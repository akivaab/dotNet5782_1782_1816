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
                DalObject.AddDrone(droneID, model, (IDAL.DO.Enums.WeightCategories)maxWeight);
               
                //add to Drones, the list of DroneToList entities
                Location droneLocation = new Location(dalStation.Latitude, dalStation.Longitude);
                Random random = new Random();
                double battery = random.Next(20, 41);
                Drones.Add(new DroneToList(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, droneLocation, -1));
                
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
                IDAL.DO.Station dalStation = dalStations.Find(s => new Location(s.Latitude, s.Longitude) == closestStationLocation);
                DalObject.ChargeDrone(droneID, dalStation.ID);

                Drones[droneIndex].Battery -= PowerConsumption[(int)Enums.WeightCategories.free] * getDistance(Drones[droneIndex].Location, closestStationLocation);
                Drones[droneIndex].Location = closestStationLocation;
                Drones[droneIndex].Status = Enums.DroneStatus.maintenance;
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public void ReleaseFromCharge(int droneID, double chargingTime)
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
                List<IDAL.DO.Station> dalStationList = new(DalObject.DisplayStationsList());
                IDAL.DO.Station dalStation = dalStationList.Find(s => s.Latitude == Drones[droneIndex].Location.Latitude && s.Longitude == Drones[droneIndex].Location.Longitude);
                DalObject.ReleaseDroneFromCharging(droneID, dalStation.ID);

                Drones[droneIndex].Battery = ChargeRatePerHour * chargingTime;
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
                if (droneToList.PackageID != -1 && droneToList.Status == Enums.DroneStatus.delivery) //this drone is delivering a package
                {
                    IDAL.DO.Package dalPackage = DalObject.DisplayPackage(droneToList.PackageID);

                    IDAL.DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
                    IDAL.DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                    
                    CustomerForPackage packageSender = new(sender.ID, sender.Name);
                    CustomerForPackage packageReceiver = new(receiver.ID, receiver.Name);
                    
                    Location collectLocation = new(sender.Latitude, sender.Longitude);
                    Location deliveryLocation = new(receiver.Latitude, receiver.Longitude);

                    bool status = dalPackage.Collected != DateTime.MinValue && dalPackage.Delivered == DateTime.MinValue ? true : false;
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
    }
}