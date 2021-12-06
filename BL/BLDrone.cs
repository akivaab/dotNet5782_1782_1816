using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories maxWeight, int stationID)
        {
            if (Drones.FindIndex(d => d.ID == droneID) != -1)
            {
                throw new NonUniqueIdException();
            }

            try
            {
                //these statements may throw exceptions:
                IDAL.DO.Station dalStation = DalObject.DisplayStation(stationID);
                if (dalStation.AvailableChargeSlots <= 0)
                {
                    throw new UnableToChargeException();
                }

                DalObject.AddDrone(droneID, model, (IDAL.DO.Enums.WeightCategories)maxWeight);
                DalObject.ChargeDrone(droneID, stationID);  //the drone starts out charging in a station

                //add to Drones, the list of DroneToList entities
                Location droneLocation = new Location(dalStation.Latitude, dalStation.Longitude);
                Random random = new Random();
                double battery = random.Next(20, 41);
                Drones.Add(new DroneToList(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, droneLocation, null));
                
                Drone drone = new(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, null, droneLocation);
                return drone;
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
            catch (IDAL.DO.NonUniqueIdException)
            {
                throw new NonUniqueIdException();
            }
        }
        public void UpdateDroneModel(int droneID, string model)
        {
            //update in the list of DroneToLists
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            Drones[droneIndex].Model = model;

            //update in the data layer
            try
            {
                DalObject.UpdateDroneModel(droneID, model);
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public void SendDroneToCharge(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }

            List<IDAL.DO.Station> reachableStations = getReachableStations(Drones[droneIndex]);
            if (Drones[droneIndex].Status != Enums.DroneStatus.available || reachableStations.Count == 0)
            {
                throw new UnableToChargeException();
            }

            try 
            { 
                Location closestStationLocation = getClosestStation(Drones[droneIndex].Location, reachableStations);
                List<IDAL.DO.Station> dalStations = new(DalObject.DisplayStationsList());
                IDAL.DO.Station dalStation = dalStations.Find(s => s.Latitude == closestStationLocation.Latitude && s.Longitude == closestStationLocation.Longitude);
                DalObject.ChargeDrone(droneID, dalStation.ID);

                Drones[droneIndex].Battery = Math.Max(Drones[droneIndex].Battery - (PowerConsumption[(int)Enums.WeightCategories.free] * getDistance(Drones[droneIndex].Location, closestStationLocation)), 0);
                Drones[droneIndex].Location = closestStationLocation;
                Drones[droneIndex].Status = Enums.DroneStatus.maintenance;
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public void ReleaseFromCharge(int droneID, double chargingTimeInHours)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }

            //check drone status
            if (Drones[droneIndex].Status != Enums.DroneStatus.maintenance)
            {
                throw new UnableToReleaseException();
            }

            try 
            { 
                List<IDAL.DO.Station> dalStations = (List<IDAL.DO.Station>)DalObject.FindStations(s => s.Latitude == Drones[droneIndex].Location.Latitude && s.Longitude == Drones[droneIndex].Location.Longitude);
                DalObject.ReleaseDroneFromCharging(droneID, dalStations[0].ID);

                Drones[droneIndex].Battery = Math.Min(Drones[droneIndex].Battery + (ChargeRatePerHour * chargingTimeInHours), 100);
                Drones[droneIndex].Status = Enums.DroneStatus.available;
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public Drone DisplayDrone(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            DroneToList droneToList = Drones[droneIndex];

            try
            {
                PackageInTransfer packageInTransfer;
                if (droneToList.PackageID != null && droneToList.Status == Enums.DroneStatus.delivery) //this drone is delivering a package
                {
                    IDAL.DO.Package dalPackage = DalObject.DisplayPackage((int)droneToList.PackageID);

                    IDAL.DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
                    IDAL.DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                    
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
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public List<DroneToList> DisplayAllDrones()
        {
            return Drones;
        }

        public List<DroneToList> FindDrones(Predicate<DroneToList> predicate)
        {
            List<DroneToList> filteredDroneToLists = new();
            foreach (DroneToList drone in Drones)
            {
                if (predicate(drone) == true)
                {
                    filteredDroneToLists.Add(drone);
                }
            }
            return filteredDroneToLists;
        }

        public DateTime GetTimeChargeBegan(int droneID)
        {
            try
            {
                return DalObject.GetTimeChargeBegan(droneID);
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
    }
}