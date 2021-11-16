using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public List<DroneToList> Drones;
        public IDAL.IDal DalObject;
        public double[] PowerConsumption;
        public double ChargeRatePerHour;
        public BL()
        {
            DalObject = new DalObject.DalObject();
            double[] powerConsumption = DalObject.DronePowerConsumption();
            Array.Copy(powerConsumption, PowerConsumption, 4);
            ChargeRatePerHour = powerConsumption[4];

            List<IDAL.DO.Drone> dalDrones = new List<IDAL.DO.Drone>(DalObject.DisplayDronesList());

            List<IDAL.DO.Package> dalPackages = new List<IDAL.DO.Package>(DalObject.DisplayPackagesList());
            foreach (IDAL.DO.Package package in dalPackages)
            {
                if (package.Delivered == DateTime.MinValue && package.DroneID != 0) //undelivered, drone assigned
                {
                    IDAL.DO.Drone drone = dalDrones.Find(drone => drone.ID == package.DroneID);
                    dalDrones.Remove(drone);
                    DroneToList droneToList = new();
                    droneToList.ID = drone.ID;
                    droneToList.Model = drone.Model;
                    droneToList.MaxWeight = (Enums.WeightCategories)drone.MaxWeight;
                    droneToList.Status = Enums.DroneStatus.delivery;
                    if (package.Assigned != DateTime.MinValue && package.Collected == DateTime.MinValue) //assigned but not collected
                    {
                        droneToList.Location = getClosestStation(getCustomerLocation(package.SenderID)); //station closest to sender
                    }
                    else if (package.Collected != DateTime.MinValue)
                    {
                        droneToList.Location = getCustomerLocation(package.SenderID); //sender location
                    }
                    droneToList.Battery = randomBatteryPower(droneToList, package, powerConsumption[(int)package.Weight]);
                    droneToList.PackageID = package.ID;
                    Drones.Add(droneToList);
                }
            }
            Random random = new Random();
            foreach (IDAL.DO.Drone drone in dalDrones) //remaining drones not delivering
            {
                DroneToList droneToList = new();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.MaxWeight = (Enums.WeightCategories)drone.MaxWeight;
                int randInt = random.Next(1, 3);
                if (randInt == 1)
                {
                    droneToList.Status = Enums.DroneStatus.maintenance;
                    List<IDAL.DO.Station> dataLayerStations = new List<IDAL.DO.Station>(DalObject.DisplayStationsList());
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
                    IDAL.DO.Customer customer = packageReceiver(dalPackages);
                    droneToList.Location = new Location(customer.Latitude, customer.Longitude);
                    droneToList.Battery = random.Next((int)Math.Ceiling(powerConsumption[(int)Enums.WeightCategories.free] * getDistance(droneToList.Location, getClosestStation(droneToList.Location))), 100);
                }
                droneToList.PackageID = -1;
                Drones.Add(droneToList);
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

            DalObject.AddStation(stationID, name, numAvailableChargingSlots, location.Latitude, location.Longitude);
            return station;
        }
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories weight, int stationID)
        {
            Drone drone = new();
            drone.ID = droneID;
            drone.Model = model;
            drone.MaxWeight = weight;
            Random random = new Random();
            drone.Battery = random.Next(20, 41);
            drone.Status = Enums.DroneStatus.maintenance;
            IDAL.DO.Station station = ((List<IDAL.DO.Station>)DalObject.DisplayStationsList()).Find(s => s.ID == stationID);
            Location location = new();
            location.Latitude = station.Latitude;
            location.Longitude = station.Longitude;
            drone.Location = location;

            DalObject.AddDrone(droneID, model, (IDAL.DO.Enums.WeightCategories)weight);
            return drone;
        }
        public Customer AddCustomer(int customerID, string name, string phone, Location location)
        {
            Customer customer = new();
            customer.ID = customerID;
            customer.Name = name;
            customer.Phone = phone;
            customer.Location = location;

            DalObject.AddCustomer(customerID, name, phone, location.Latitude, location.Longitude);
            return customer;
        }
        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            Package package = new();
            List<IDAL.DO.Customer> customers = new List<IDAL.DO.Customer>(DalObject.DisplayCustomersList());
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

            DalObject.AddPackage(senderID, receiverID, (IDAL.DO.Enums.WeightCategories)weight, (IDAL.DO.Enums.Priorities)priority);
            return package;
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
        public void UpdateStation(int stationID, int name = -1, int numChargingSlots = -1)
        {
            List<IDAL.DO.Station> dalStationList = (List<IDAL.DO.Station>)DalObject.DisplayStationsList();
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
            List<IDAL.DO.Customer> dalCustomerList = (List<IDAL.DO.Customer>)DalObject.DisplayCustomersList();
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
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            List<IDAL.DO.Station> reachableStations = getReachableStations(Drones[droneIndex]);
            if (Drones[droneIndex].Status != Enums.DroneStatus.available || reachableStations.Count == 0)
            {
                throw new UnableToCharge();
            }
            Location closestStationLocation = getClosestStation(Drones[droneIndex].Location, reachableStations);
            List<IDAL.DO.Station> dalStations = new List<IDAL.DO.Station>(DalObject.DisplayStationsList());
            int stationIndex = dalStations.FindIndex(s => s.Latitude == closestStationLocation.Latitude && s.Longitude == closestStationLocation.Longitude);
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
                throw new UnableToRelease();
            }
            Drones[droneIndex].Battery = ChargeRatePerHour * chargingTime;
            Drones[droneIndex].Status = Enums.DroneStatus.available;

            List<IDAL.DO.Station> dalStationList = new List<IDAL.DO.Station>(DalObject.DisplayStationsList());
            int stationIndex = dalStationList.FindIndex(s => s.Latitude == Drones[droneIndex].Location.Latitude && s.Longitude == Drones[droneIndex].Location.Longitude);
            if (stationIndex == -1)
            {
                throw new Exception();
            }
            DalObject.ReleaseDroneFromCharging(droneID, dalStationList[stationIndex].ID);
        }
        public void AssignPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            if (Drones[droneIndex].Status != Enums.DroneStatus.available)
            {
                throw new Exception();
            }
            List<IDAL.DO.Package> dalPackages = new List<IDAL.DO.Package>(DalObject.DisplayUnassignedPackagesList());
            int bestPackageID = findBestPackage(dalPackages, Drones[droneIndex]);
            if (bestPackageID == -1)
            {
                throw new Exception();
            }
            Drones[droneIndex].Status = Enums.DroneStatus.delivery;
            Drones[droneIndex].PackageID = bestPackageID;
            DalObject.AssignPackage(bestPackageID, droneID);
        }
        public void CollectPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            if (Drones[droneIndex].PackageID == -1)
            {
                throw new Exception();
            }
            List<IDAL.DO.Package> dalPackages = new List<IDAL.DO.Package>(DalObject.DisplayPackagesList());
            int dalPackageIndex = dalPackages.FindIndex(p => p.ID == Drones[droneIndex].PackageID);
            IDAL.DO.Package dalPackage = dalPackages[dalPackageIndex];
            if (dalPackageIndex == -1 || dalPackage.Assigned == DateTime.MinValue || dalPackage.Collected != DateTime.MinValue)
            {
                throw new Exception();
            }
            Location senderLocation = getCustomerLocation(dalPackage.SenderID);
            Drones[droneIndex].Battery -= PowerConsumption[0] * getDistance(Drones[droneIndex].Location, senderLocation);
            Drones[droneIndex].Location = senderLocation;
            DalObject.CollectPackage(dalPackage.ID, droneID);
        }
        public void DeliverPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            if (Drones[droneIndex].PackageID == -1)
            {
                throw new Exception();
            }
            List<IDAL.DO.Package> dalPackages = new List<IDAL.DO.Package>(DalObject.DisplayPackagesList());
            int dalPackageIndex = dalPackages.FindIndex(p => p.ID == Drones[droneIndex].PackageID);
            IDAL.DO.Package dalPackage = dalPackages[dalPackageIndex];
            if (dalPackageIndex == -1 || dalPackage.Collected == DateTime.MinValue || dalPackage.Delivered != DateTime.MinValue)
            {
                throw new Exception();
            }
            Location receiverLocation = getCustomerLocation(dalPackage.ReceiverID);
            Drones[droneIndex].Battery -= PowerConsumption[(int)dalPackage.Weight] * getDistance(Drones[droneIndex].Location, receiverLocation);
            Drones[droneIndex].Location = receiverLocation;
            Drones[droneIndex].Status = Enums.DroneStatus.available;
            DalObject.DeliverPackage(dalPackage.ID, droneID);
        }
        public Station DisplayStation(int stationID)
        {
            throw new NotImplementedException();
        }
        public Drone DisplayDrone(int droneID)
        {
            throw new NotImplementedException();
        }
        public Customer DisplayCustomer(int customerID)
        {
            throw new NotImplementedException();
        }
        public Package DisplayPackage(int packageID)
        {
            throw new NotImplementedException();
        }
        public List<StationToList> DisplayAllStations()
        {
            throw new NotImplementedException();
        }
        public List<DroneToList> DisplayAllDrones()
        {
            throw new NotImplementedException();
        }
        public List<CustomerToList> DisplayAllCustomers()
        {
            throw new NotImplementedException();
        }
        public List<PackageToList> DisplayAllPackages()
        {
            throw new NotImplementedException();
        }
        public List<PackageToList> DisplayAllUnassignedPackages()
        {
            throw new NotImplementedException();
        }
        public List<StationToList> DisplayFreeStations()
        {
            throw new NotImplementedException();
        }
    }
}
