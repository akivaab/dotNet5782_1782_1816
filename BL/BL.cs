using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public List<DroneToList> drones;
        public IDAL.IDal dalObject;
        public BL()
        {
            dalObject = new DalObject.DalObject();
            double[] powerConsumption = dalObject.DronePowerConsumption();
            List<IDAL.DO.Drone> dalDrones = new List<IDAL.DO.Drone>(dalObject.DisplayDronesList());

            List<IDAL.DO.Package> dalPackages = new List<IDAL.DO.Package>(dalObject.DisplayPackagesList());
            foreach (IDAL.DO.Package package in dalPackages)
            {
                if (package.Delivered == DateTime.MinValue && package.DroneID != 0) //undelivered, drone assigned
                {
                    IDAL.DO.Drone drone = dalDrones.Find(drone => drone.ID == package.DroneID);
                    dalDrones.Remove(drone);
                    DroneToList droneToList = new();
                    droneToList.ID = drone.ID;
                    droneToList.Model = drone.Model;
                    droneToList.Weight = (Enums.WeightCategories)drone.MaxWeight;
                    droneToList.Status = Enums.DroneStatus.delivery;
                    if (package.Assigned != DateTime.MinValue && package.Collected == DateTime.MinValue) //assigned but not collected
                    {
                        droneToList.Location = closestStationToCustomer(dalObject, package.SenderID); //station closest to sender
                    }
                    else if (package.Collected != DateTime.MinValue)
                    {
                        droneToList.Location = getCustomerLocation(dalObject, package.SenderID); //sender location
                    }
                    droneToList.Battery = randomBatteryPower(dalObject, powerConsumption[(int)package.Weight]);//NOT IMPLEMENTED
                    droneToList.PackageID = package.ID;
                    drones.Add(droneToList);
                }
            }
            Random random = new Random();
            foreach (IDAL.DO.Drone drone in dalDrones) //remaining drones not delivering
            {
                DroneToList droneToList = new();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.Weight = (Enums.WeightCategories)drone.MaxWeight;
                int randInt = random.Next(1, 3);
                if (randInt == 1)
                {
                    droneToList.Status = Enums.DroneStatus.maintenance;
                    List<IDAL.DO.Station> dataLayerStations = new List<IDAL.DO.Station>(dalObject.DisplayStationsList());
                    int randStation = random.Next(dataLayerStations.Count);
                    double stationLatitude = dataLayerStations[randStation].Latitude;
                    double stationLongitude = dataLayerStations[randStation].Longitude;
                    droneToList.Location.Latitude = stationLatitude;
                    droneToList.Location.Longitude = stationLongitude;
                    droneToList.Battery = random.Next(20);
                }
                else if (randInt == 2)
                {
                    droneToList.Status = Enums.DroneStatus.available;
                    IDAL.DO.Customer customer = packageReceiver(dalObject, dalPackages);
                    double customerLatitude = customer.Latitude;
                    double customerLongitude = customer.Longitude;
                    droneToList.Location.Latitude = customerLatitude;
                    droneToList.Location.Longitude = customerLongitude;
                    droneToList.Battery = randomBatteryPower(dalObject, powerConsumption[0]);//NOT IMPLEMENTED
                }
                droneToList.PackageID = 0;
                drones.Add(droneToList);
            }
        }
        public Station AddStation(int stationID, int name, Location location, int numAvailableChargingSlots)
        {
            Station station = new();
            station.ID = stationID;
            station.Name = name;
            station.Location = location;
            station.AvailableChargeSlots = numAvailableChargingSlots;
            station.DronesCharging = new List<DroneCharging>();

            dalObject.AddStation(stationID, name, numAvailableChargingSlots, location.Latitude, location.Longitude);
            return station;
        }
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories weight, int stationID)
        {
            Drone drone = new();
            drone.ID = droneID;
            drone.Model = model;
            drone.Weight = weight;
            Random random = new Random();
            drone.Battery = random.Next(20, 41);
            drone.Status = Enums.DroneStatus.maintenance;
            IDAL.DO.Station station = ((List<IDAL.DO.Station>)dalObject.DisplayStationsList()).Find(s => s.ID == stationID);
            Location location = new();
            location.Latitude = station.Latitude;
            location.Longitude = station.Longitude;
            drone.Location = location;

            dalObject.AddDrone(droneID, model, (IDAL.DO.Enums.WeightCategories)weight);
            return drone;
        }
        public Customer AddCustomer(int customerID, string name, string phone, Location location)
        {
            Customer customer = new();
            customer.ID = customerID;
            customer.Name = name;
            customer.Phone = phone;
            customer.Location = location;

            dalObject.AddCustomer(customerID, name, phone, location.Latitude, location.Longitude);
            return customer;
        }
        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            Package package = new();
            List<IDAL.DO.Customer> customers = new List<IDAL.DO.Customer>(dalObject.DisplayCustomersList());
            IDAL.DO.Customer sender = customers.Find(s => s.ID == senderID);
            IDAL.DO.Customer receiver = customers.Find(r => r.ID == receiverID);
            CustomerForPackage packageSender = new();
            packageSender.ID = sender.ID;
            packageSender.Name = sender.Name;
            CustomerForPackage packageReceiver = new();
            packageReceiver.ID = receiver.ID;
            packageReceiver.Name = receiver.Name;
            package.Sender = packageSender;
            package.Receiver = packageReceiver;
            package.Weight = weight;
            package.Priority = priority;
            package.DroneDelivering = null;
            package.RequestTime = DateTime.Now;
            package.AssigningTime = DateTime.MinValue;
            package.CollectingTime = DateTime.MinValue;
            package.DeliveringTime = DateTime.MinValue;

            dalObject.AddPackage(senderID, receiverID, (IDAL.DO.Enums.WeightCategories)weight, (IDAL.DO.Enums.Priorities)priority);
            return package;
        }
        public void UpdateDroneModel(int droneID, string model)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            List<IDAL.DO.Drone> dalDroneList = (List<IDAL.DO.Drone>)dalObject.DisplayDronesList();
            int dalDroneIndex = dalDroneList.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1 || dalDroneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            drones[droneIndex].Model = model;
            IDAL.DO.Drone droneToModify = dalDroneList[dalDroneIndex];
            dalDroneList.RemoveAt(droneIndex);
            droneToModify.Model = model;
            dalDroneList.Insert(droneIndex, droneToModify);
        }
        public void UpdateStation(int stationID, int name = -1, int numChargingSlots = -1)
        {
            List<IDAL.DO.Station> dalStationList = (List<IDAL.DO.Station>)dalObject.DisplayStationsList();
            int stationIndex = dalStationList.FindIndex(s => s.ID == stationID);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            IDAL.DO.Station station = dalStationList[stationIndex];
            dalStationList.RemoveAt(stationIndex);
            if (name != -1)
            {
                station.Name = name;
            }
            if (numChargingSlots != -1)
            {
                station.NumChargeSlots = numChargingSlots;
            }
            dalStationList.Insert(stationIndex, station);
        }
        public void UpdateCustomer(int customerID, string name = "", string phone = "")
        {
            List<IDAL.DO.Customer> dalCustomerList = (List<IDAL.DO.Customer>)dalObject.DisplayCustomersList();
            int customerIndex = dalCustomerList.FindIndex(c => c.ID == customerID);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            IDAL.DO.Customer customer = dalCustomerList[customerIndex];
            dalCustomerList.RemoveAt(customerIndex);
            if (name != "")
            {
                customer.Name = name;
            }
            if (phone != "")
            {
                customer.Phone = phone;
            }
            dalCustomerList.Insert(customerIndex, customer);
        }
        public void SendDroneToCharge(int droneID)
        {
            throw new NotImplementedException();
        }
        public void ReleaseFromCharge(int droneID, DateTime chargingTime)
        {
            throw new NotImplementedException();
        }
        public void AssignPackage(int droneID)
        {
            throw new NotImplementedException();
        }
        public void CollectPackage(int droneID)
        {
            throw new NotImplementedException();
        }
        public void DeliverPackage(int droneID)
        {
            throw new NotImplementedException();
        }
        public void DisplayStation(int stationID)
        {
            throw new NotImplementedException();
        }
        public void DisplayDrone(int droneID)
        {
            throw new NotImplementedException();
        }
        public void DisplayCustomer(int customerID)
        {
            throw new NotImplementedException();
        }
        public void DisplayPackage(int packageID)
        {
            throw new NotImplementedException();
        }
        public void DisplayAllStations()
        {
            throw new NotImplementedException();
        }
        public void DisplayAllDrones()
        {
            throw new NotImplementedException();
        }
        public void DisplayAllCustomers()
        {
            throw new NotImplementedException();
        }
        public void DisplayAllPackages()
        {
            throw new NotImplementedException();
        }
        public void DisplayAllUnassignedPackages()
        {
            throw new NotImplementedException();
        }
        public void DisplayFreeStations()
        {
            throw new NotImplementedException();
        }

    }
}
