using System;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    /// <summary>
    /// Drone-related functionality of the Business Layer.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories maxWeight, int stationID)
        {
            if (Drones.FindIndex(d => d.ID == droneID) != -1)
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }

            try
            {
                //these statements may throw exceptions:
                DO.Station dalStation = DalObject.DisplayStation(stationID);
                if (dalStation.AvailableChargeSlots <= 0)
                {
                    throw new UnableToChargeException("There are no available charge slots for the drone in the given station.");
                }

                DalObject.AddDrone(droneID, model, (DO.Enums.WeightCategories)maxWeight);
                DalObject.ChargeDrone(droneID, stationID);  //the drone starts out charging in a station

                //add to Drones, the list of DroneToList entities
                Location droneLocation = new Location(dalStation.Latitude, dalStation.Longitude);
                Random random = new Random();
                double battery = random.Next(20, 41);
                Drones.Add(new DroneToList(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, droneLocation, null));
                
                Drone drone = new(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, null, droneLocation);
                return drone;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
        }

        public void UpdateDroneModel(int droneID, string model)
        {
            //update in the list of DroneToLists
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }
            Drones[droneIndex].Model = model;

            //update in the data layer
            try
            {
                DalObject.UpdateDroneModel(droneID, model);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public void SendDroneToCharge(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (Drones[droneIndex].Status != Enums.DroneStatus.available)
            {
                throw new UnableToChargeException("The drone cannot currently be sent to charge.");
            }

            IEnumerable<DO.Station> reachableStations = getReachableStations(Drones[droneIndex]);
            if (reachableStations.Count() == 0)
            {
                throw new UnableToChargeException("The drone does not have enough battery to reach a station.");
            }

            try 
            { 
                Location closestStationLocation = getClosestStation(Drones[droneIndex].Location, reachableStations);
                IEnumerable<DO.Station> dalStations = DalObject.DisplayStationsList();
                DO.Station dalStation = dalStations.Where(s => s.Latitude == closestStationLocation.Latitude && s.Longitude == closestStationLocation.Longitude).First();
                
                DalObject.ChargeDrone(droneID, dalStation.ID);

                Drones[droneIndex].Battery = Math.Max(Drones[droneIndex].Battery - (PowerConsumption[(int)Enums.WeightCategories.free] * getDistance(Drones[droneIndex].Location, closestStationLocation)), 0);
                Drones[droneIndex].Location = closestStationLocation;
                Drones[droneIndex].Status = Enums.DroneStatus.maintenance;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public void ReleaseFromCharge(int droneID, double chargingTimeInHours)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            //check drone status
            if (Drones[droneIndex].Status != Enums.DroneStatus.maintenance)
            {
                throw new UnableToReleaseException("The drone is not currently charging and so cannot be released.");
            }

            try 
            { 
                IEnumerable<DO.Station> dalStations = DalObject.FindStations(s => s.Latitude == Drones[droneIndex].Location.Latitude && s.Longitude == Drones[droneIndex].Location.Longitude);
                DalObject.ReleaseDroneFromCharging(droneID, dalStations.First().ID);

                Drones[droneIndex].Battery = Math.Min(Drones[droneIndex].Battery + (ChargeRatePerHour * chargingTimeInHours), 100);
                Drones[droneIndex].Status = Enums.DroneStatus.available;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public Drone DisplayDrone(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }
            DroneToList droneToList = Drones[droneIndex];

            try
            {
                PackageInTransfer packageInTransfer;
                if (droneToList.PackageID != null && droneToList.Status == Enums.DroneStatus.delivery) //this drone is delivering a package
                {
                    DO.Package dalPackage = DalObject.DisplayPackage((int)droneToList.PackageID);

                    DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
                    DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                    
                    CustomerForPackage packageSender = new(sender.ID, sender.Name);
                    CustomerForPackage packageReceiver = new(receiver.ID, receiver.Name);
                    
                    Location collectLocation = new(sender.Latitude, sender.Longitude);
                    Location deliveryLocation = new(receiver.Latitude, receiver.Longitude);

                    bool status = dalPackage.Collected != null && dalPackage.Delivered == null ? true : false;
                    packageInTransfer = new(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, status, packageSender, packageReceiver, collectLocation, deliveryLocation, getDistance(collectLocation, deliveryLocation));
                }
                else
                {
                    packageInTransfer = null;
                }

                Drone drone = new(droneToList.ID, droneToList.Model, droneToList.MaxWeight, droneToList.Battery, droneToList.Status, packageInTransfer, droneToList.Location);
                return drone;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public IEnumerable<DroneToList> DisplayAllDrones()
        {
            return Drones;
        }

        public IEnumerable<DroneToList> FindDrones(Predicate<DroneToList> predicate)
        {
            return from DroneToList drone in Drones
                   where predicate(drone)
                   select drone;
        }

        public DateTime GetTimeChargeBegan(int droneID)
        {
            try
            {
                return DalObject.GetTimeChargeBegan(droneID);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
    }
}