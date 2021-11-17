using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories maxWeight, int stationID)
        {
            Random random = new Random();
            IDAL.DO.Station dalStation = DalObject.DisplayStation(stationID);
            double battery = random.Next(20, 41);
            Location droneLocation = new Location(dalStation.Latitude, dalStation.Longitude);
            Drone drone = new(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, null, droneLocation);
            DalObject.AddDrone(droneID, model, (IDAL.DO.Enums.WeightCategories)maxWeight);
            Drones.Add(new DroneToList(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, droneLocation, -1));
            return drone;
        }
        public void UpdateDroneModel(int droneID, string model)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            List<IDAL.DO.Drone> dalDroneList = (List<IDAL.DO.Drone>)DalObject.DisplayDronesList();
            int dalDroneIndex = dalDroneList.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1 || dalDroneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            Drones[droneIndex].Model = model;
            IDAL.DO.Drone droneToModify = dalDroneList[dalDroneIndex];
            dalDroneList.RemoveAt(droneIndex);
            droneToModify.Model = model;
            dalDroneList.Insert(droneIndex, droneToModify);
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
            Location closestStationLocation = getClosestStation(Drones[droneIndex].Location, reachableStations);
            List<IDAL.DO.Station> dalStations = new(DalObject.DisplayStationsList());
            int stationIndex = dalStations.FindIndex(s => new Location(s.Latitude, s.Longitude) == closestStationLocation);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            Drones[droneIndex].Battery -= PowerConsumption[0] * getDistance(Drones[droneIndex].Location, closestStationLocation);
            Drones[droneIndex].Location = closestStationLocation;
            Drones[droneIndex].Status = Enums.DroneStatus.maintenance;
            DalObject.ChargeDrone(droneID, dalStations[stationIndex].ID);
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
            Drones[droneIndex].Battery = ChargeRatePerHour * chargingTime;
            Drones[droneIndex].Status = Enums.DroneStatus.available;

            List<IDAL.DO.Station> dalStationList = new List<IDAL.DO.Station>(DalObject.DisplayStationsList());
            int stationIndex = dalStationList.FindIndex(s => new Location(s.Latitude, s.Longitude) == Drones[droneIndex].Location);
            if (stationIndex == -1)
            {
                throw new Exception();
            }
            DalObject.ReleaseDroneFromCharging(droneID, dalStationList[stationIndex].ID);
        }
        public Drone DisplayDrone(int droneID) //what if no package is assigned?
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            DroneToList droneToList = Drones[droneIndex];

            IDAL.DO.Package dalPackage = DalObject.DisplayPackage(droneToList.PackageID);
            bool status = dalPackage.Collected != DateTime.MinValue && dalPackage.Delivered == DateTime.MinValue ? true : false;

            IDAL.DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
            IDAL.DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
            CustomerForPackage packageSender = new(sender.ID, sender.Name);
            CustomerForPackage packageReceiver = new(receiver.ID, receiver.Name);
            Location collectLocation = new(sender.Latitude, sender.Longitude);
            Location deliveryLocation = new(receiver.Latitude, receiver.Longitude);
            PackageInTransfer packageInTransfer = new(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, status, packageSender, packageReceiver, collectLocation, deliveryLocation, getDistance(collectLocation, deliveryLocation));

            Drone drone = new(droneToList.ID, droneToList.Model, droneToList.MaxWeight, droneToList.Battery, droneToList.Status, packageInTransfer, droneToList.Location);
            return drone;
        }
        public List<DroneToList> DisplayAllDrones()
        {
            return Drones;
        }
    }
}