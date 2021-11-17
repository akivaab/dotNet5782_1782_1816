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

            List<IDAL.DO.Drone> dalDrones = new(DalObject.DisplayDronesList());

            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            foreach (IDAL.DO.Package package in dalPackages)
            {
                if (package.Delivered == DateTime.MinValue && package.DroneID != 0) //undelivered, drone assigned
                {
                    IDAL.DO.Drone drone = dalDrones.Find(drone => drone.ID == package.DroneID);
                    dalDrones.Remove(drone);
                    Location droneLocation = new();
                    if (package.Assigned != DateTime.MinValue && package.Collected == DateTime.MinValue) //assigned but not collected
                    {
                        droneLocation = getClosestStation(getCustomerLocation(package.SenderID)); //station closest to sender
                    }
                    else if (package.Collected != DateTime.MinValue)
                    {
                        droneLocation = getCustomerLocation(package.SenderID); //sender location
                    }
                    double battery = randomBatteryPower(droneLocation, package, powerConsumption[(int)package.Weight]);
                    DroneToList droneToList = new(drone.ID, drone.Model, (Enums.WeightCategories)drone.MaxWeight, battery, Enums.DroneStatus.delivery, droneLocation, package.ID);
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
                    List<IDAL.DO.Station> dataLayerStations = new(DalObject.DisplayStationsList());
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
            Station station = new(stationID, name, location, numAvailableChargingSlots, new List<DroneCharging>());
            DalObject.AddStation(stationID, name, numAvailableChargingSlots, location.Latitude, location.Longitude);
            return station;
        }
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories weight, int stationID)
        {
            Random random = new Random();
            IDAL.DO.Station dalStation = DalObject.DisplayStation(stationID);
            double battery = random.Next(20, 41);
            Location droneLocation = new Location(dalStation.Latitude, dalStation.Longitude);
            Drone drone = new(droneID, model, weight, battery, Enums.DroneStatus.maintenance, null, droneLocation);
            DalObject.AddDrone(droneID, model, (IDAL.DO.Enums.WeightCategories)weight);
            Drones.Add(new DroneToList(droneID, model, weight, battery, Enums.DroneStatus.maintenance, droneLocation, -1));
            return drone;
        }
        public Customer AddCustomer(int customerID, string name, string phone, Location location)
        {
            Customer customer = new(customerID, name, phone, location, new List<PackageForCustomer>(), new List<PackageForCustomer>());
            DalObject.AddCustomer(customerID, name, phone, location.Latitude, location.Longitude);
            return customer;
        }
        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            IDAL.DO.Customer sender = DalObject.DisplayCustomer(senderID);
            IDAL.DO.Customer receiver = DalObject.DisplayCustomer(receiverID);
            CustomerForPackage packageSender = new(sender.ID, sender.Name);
            CustomerForPackage packageReceiver = new(receiver.ID, receiver.Name);
            
            int packageID = DalObject.AddPackage(senderID, receiverID, (IDAL.DO.Enums.WeightCategories)weight, (IDAL.DO.Enums.Priorities)priority);
            Package package = new(packageID, packageSender, packageReceiver, weight, priority, null, DateTime.Now, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
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
        public void UpdateStation(int stationID, int name = -1, int totalChargingSlots = -1)
        {
            List<IDAL.DO.Station> dalStationList = (List<IDAL.DO.Station>)DalObject.DisplayStationsList();
            int stationIndex = dalStationList.FindIndex(s => s.ID == stationID);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            IDAL.DO.Station dalStation = dalStationList[stationIndex];
            dalStationList.RemoveAt(stationIndex);
            if (name != -1)
            {
                dalStation.Name = name;
            }
            if (totalChargingSlots != -1)
            {
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == new Location(dalStation.Latitude, dalStation.Longitude));
                dalStation.AvailableChargeSlots = totalChargingSlots - dronesAtStation.Count;
            }
            dalStationList.Insert(stationIndex, dalStation);
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
                throw new UnableToRelease();
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
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayUnassignedPackagesList());
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
            IDAL.DO.Package dalPackage = DalObject.DisplayPackage(Drones[droneIndex].PackageID);
            if (dalPackage.Assigned == DateTime.MinValue || dalPackage.Collected != DateTime.MinValue)
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
            IDAL.DO.Package dalPackage = DalObject.DisplayPackage(Drones[droneIndex].PackageID);
            if (dalPackage.Collected == DateTime.MinValue || dalPackage.Delivered != DateTime.MinValue)
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
            IDAL.DO.Station dalStation = DalObject.DisplayStation(stationID);
            Location stationLocation = new Location(dalStation.Latitude, dalStation.Longitude);
            
            //find drones at this station
            List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == stationLocation);
            
            //initialize DroneCharging entities
            List<DroneCharging> dronesCharging = new();
            foreach (DroneToList drone in dronesAtStation)
            {
                dronesCharging.Add(new DroneCharging(drone.ID, drone.Battery));
            }
            
            return new Station(dalStation.ID, dalStation.Name, stationLocation, dalStation.AvailableChargeSlots, dronesCharging);
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
        public Customer DisplayCustomer(int customerID)
        {
            IDAL.DO.Customer dalCustomer = DalObject.DisplayCustomer(customerID);
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            List<IDAL.DO.Package> dalPackagesToSend = dalPackages.FindAll(p => p.SenderID == customerID);
            List<IDAL.DO.Package> dalPackagesToReceive = dalPackages.FindAll(p => p.ReceiverID == customerID);
            List<PackageForCustomer> packagesToSend = new();
            List<PackageForCustomer> packagesToReceive = new();
            foreach (IDAL.DO.Package dalPackage in dalPackagesToSend)
            {
                IDAL.DO.Customer receiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                CustomerForPackage receiverForPackage = new(receiver.ID, receiver.Name);
                packagesToSend.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), receiverForPackage));
            }
            foreach (IDAL.DO.Package dalPackage in dalPackagesToReceive)
            {
                IDAL.DO.Customer sender = DalObject.DisplayCustomer(dalPackage.SenderID);
                CustomerForPackage senderForPackage = new(sender.ID, sender.Name);
                packagesToReceive.Add(new PackageForCustomer(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage), senderForPackage));
            }
            Customer customer = new(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, new Location(dalCustomer.Latitude, dalCustomer.Longitude), packagesToSend, packagesToReceive);
            return customer;
        }
        public Package DisplayPackage(int packageID)
        {
            IDAL.DO.Package dalPackage = DalObject.DisplayPackage(packageID);

            IDAL.DO.Customer dalPackageSender = DalObject.DisplayCustomer(dalPackage.SenderID);
            IDAL.DO.Customer dalPackageReceiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
            CustomerForPackage senderForPackage = new(dalPackageSender.ID, dalPackageSender.Name);
            CustomerForPackage receiverForPackage = new(dalPackageReceiver.ID, dalPackageReceiver.Name);

            int droneIndex = Drones.FindIndex(d => d.ID == dalPackage.DroneID);
            DroneDelivering droneDelivering = droneIndex != -1 ? new(Drones[droneIndex].ID, Drones[droneIndex].Battery, Drones[droneIndex].Location) : null;

            Package package = new(dalPackage.ID, senderForPackage, receiverForPackage, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, droneDelivering, dalPackage.Requested, dalPackage.Assigned, dalPackage.Collected, dalPackage.Delivered);
            return package;
        }
        public List<StationToList> DisplayAllStations()
        {
            List<IDAL.DO.Station> dalStations = new(DalObject.DisplayStationsList());
            List<StationToList> stationToLists = new();
            foreach (IDAL.DO.Station dalStation in dalStations)
            {
                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == stationLocation);
                stationToLists.Add(new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count));
            }
            return stationToLists;
        }
        public List<DroneToList> DisplayAllDrones()
        {
            return Drones;
        }
        public List<CustomerToList> DisplayAllCustomers()
        {
            List<IDAL.DO.Customer> dalCustomers = new(DalObject.DisplayCustomersList());
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            List<CustomerToList> customerToLists = new();
            int numDeliveredPackagesSent = 0;
            int numUndeliveredPackagesSent = 0;
            int numPackagesReceived = 0;
            int numPackagesExpected = 0;
            foreach (IDAL.DO.Customer dalCustomer in dalCustomers)
            {
                numDeliveredPackagesSent = dalPackages.FindAll(p => p.SenderID == dalCustomer.ID && p.Delivered != DateTime.MinValue).Count;
                numUndeliveredPackagesSent = dalPackages.FindAll(p => p.SenderID == dalCustomer.ID && p.Collected != DateTime.MinValue && p.Delivered == DateTime.MinValue).Count;
                numPackagesReceived = dalPackages.FindAll(p => p.ReceiverID == dalCustomer.ID && p.Delivered != DateTime.MinValue).Count;
                numPackagesExpected = dalPackages.FindAll(p => p.ReceiverID == dalCustomer.ID && p.Delivered == DateTime.MinValue).Count;
                customerToLists.Add(new CustomerToList(dalCustomer.ID, dalCustomer.Name, dalCustomer.Phone, numDeliveredPackagesSent, numUndeliveredPackagesSent, numPackagesReceived, numPackagesExpected));
            }
            return customerToLists;
        }
        public List<PackageToList> DisplayAllPackages()
        {
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            List<PackageToList> packageToLists = new();
            foreach (IDAL.DO.Package dalPackage in dalPackages)
            {
                string senderName = DalObject.DisplayCustomer(dalPackage.SenderID).Name;
                string receiverName = DalObject.DisplayCustomer(dalPackage.ReceiverID).Name;
                packageToLists.Add(new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage)));
            }
            return packageToLists;
        }
        public List<PackageToList> DisplayAllUnassignedPackages()
        {
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayUnassignedPackagesList());
            List<PackageToList> packageToLists = new();
            foreach (IDAL.DO.Package dalPackage in dalPackages)
            {
                string senderName = DalObject.DisplayCustomer(dalPackage.SenderID).Name;
                string receiverName = DalObject.DisplayCustomer(dalPackage.ReceiverID).Name;
                packageToLists.Add(new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, Enums.PackageStatus.created));
            }
            return packageToLists;
        }
        public List<StationToList> DisplayFreeStations()
        {
            List<IDAL.DO.Station> dalStations = new(DalObject.DisplayFreeStationsList());
            List<StationToList> stationToLists = new();
            foreach (IDAL.DO.Station dalStation in dalStations)
            {
                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == stationLocation);
                stationToLists.Add(new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count));
            }
            return stationToLists;
        }
    }
}
