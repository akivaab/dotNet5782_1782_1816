using System;
using System.Collections.Generic;
using IDAL.DO;

namespace DalObject
{
    public class DalObject : IDal.IDal
    {
        /// <summary>
        /// Constructor adds initial values to the entity arrays
        /// </summary>
        public DalObject()
        {
            DataSource.Initialize();
        }

        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            if (DataSource.Stations.Count >= 5)
            {
                throw new ExceededLimitException();
            }
            if (DataSource.Stations.FindIndex(station => station.ID == id) != -1)
            {
                throw new NonUniqueIdException();
            }
            Station station = new();
            station.ID = id;
            station.Name = name;
            station.NumChargeSlots = numChargeSlots;
            station.Latitude = latitude;
            station.Longitude = longitude;
            DataSource.Stations.Add(station);
        }

        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight)
        {
            if (DataSource.Drones.Count >= 10)
            {
                throw new ExceededLimitException();
            }
            if (DataSource.Drones.FindIndex(drone => drone.ID == id) != -1)
            {
                throw new NonUniqueIdException();
            }
            Drone drone = new();
            drone.ID = id;
            drone.Model = model;
            drone.MaxWeight = maxWeight;
            DataSource.Drones.Add(drone);
        }

        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            if (DataSource.Customers.Count >= 100)
            {
                throw new ExceededLimitException();
            }
            if (DataSource.Customers.FindIndex(customer => customer.ID == id) != -1)
            {
                throw new NonUniqueIdException();
            }
            Customer customer = new();
            customer.ID = id;
            customer.Name = name;
            customer.Phone = phone;
            customer.Latitude = latitude;
            customer.Longitude = longitude;
            DataSource.Customers.Add(customer);
        }

        public int AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority, int droneID = 0)
        {
            if (DataSource.Packages.Count >= 1000)
            {
                throw new ExceededLimitException();
            }
            if (DataSource.Customers.FindIndex(customer => customer.ID == senderID) == -1 ||
                DataSource.Customers.FindIndex(customer => customer.ID == receiverID) == -1 ||
                (droneID != 0 && DataSource.Drones.FindIndex(drone => drone.ID == droneID) == -1))
            {
                throw new UndefinedObjectException();
            }
            Package package = new();
            package.ID = DataSource.Config.PackageID;
            package.SenderID = senderID;
            package.ReceiverID = receiverID;
            package.Weight = weight;
            package.Priority = priority;
            package.DroneID = droneID;
            package.Requested = DateTime.Now;
            DataSource.Config.PackageID++;
            DataSource.Packages.Add(package);
            return DataSource.Config.PackageID;
        }

        public void AssignPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            Package package = DataSource.Packages[packageIndex];
            package.DroneID = droneID;
            package.Scheduled = DateTime.Now;
            DataSource.Packages[packageIndex] = package;
        }

        public void CollectPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            Package package = DataSource.Packages[packageIndex];
            package.PickedUp = DateTime.Now;
            DataSource.Packages[packageIndex] = package;
            //Drone drone = DataSource.Drones[droneIndex];
            //drone.Status = Enums.DroneStatuses.delivery;
            //DataSource.Drones[droneIndex] = drone;
        }

        public void DeliverPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            Package package = DataSource.Packages[packageIndex];
            package.Delivered = DateTime.Now;
            package.DroneID = 0;
            DataSource.Packages[packageIndex] = package;
            //Drone drone = DataSource.Drones[droneIndex];
            //drone.Status = Enums.DroneStatuses.free;
            //DataSource.Drones[droneIndex] = drone;
        }

        public void ChargeDrone(int droneID, int stationID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int stationIndex = DataSource.Stations.FindIndex(station => station.ID == stationID);
            if (droneIndex == -1 || stationIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            //Drone drone = DataSource.Drones[droneIndex];
            //drone.Status = Enums.DroneStatuses.maintenance;
            //DataSource.Drones[droneIndex] = drone;
            Station station = DataSource.Stations[stationIndex];
            station.NumChargeSlots--;
            DataSource.Stations[stationIndex] = station;
            DroneCharge droneCharge = new DroneCharge();
            droneCharge.DroneID = droneID;
            droneCharge.StationID = stationID;
        }

        public void ReleaseDroneFromCharging(int droneID, int stationID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int stationIndex = DataSource.Stations.FindIndex(station => station.ID == stationID);
            if (droneIndex == -1 || stationIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            //Drone drone = DataSource.Drones[droneIndex];
            //drone.Status = Enums.DroneStatuses.free;
            //DataSource.Drones[droneIndex] = drone;
            Station station = DataSource.Stations[stationIndex];
            station.NumChargeSlots++;
            DataSource.Stations[stationIndex] = station;
        }

        public Station DisplayStation(int stationID)
        {
            int stationIndex = DataSource.Stations.FindIndex(station => station.ID == stationID);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            return DataSource.Stations[stationIndex];
        }

        public Drone DisplayDrone(int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            return DataSource.Drones[droneIndex];
        }

        public Customer DisplayCustomer(int customerID)
        {
            int customerIndex = DataSource.Customers.FindIndex(customer => customer.ID == customerID);
            if (customerIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            return DataSource.Customers[customerIndex];
        }

        public Package DisplayPackage(int packageID)
        {
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            return DataSource.Packages[packageIndex];
        }

        public IEnumerable<Station> DisplayStationsList()
        {
            List<Station> stations = new();
            for (int i = 0; i < DataSource.Stations.Count; i++)
            {
                stations.Add(DataSource.Stations[i]);
            }
            return stations;
        }

        public IEnumerable<Drone> DisplayDronesList()
        {
            List<Drone> drones = new();
            for (int i = 0; i < DataSource.Drones.Count; i++)
            {
                drones.Add(DataSource.Drones[i]);
            }
            return drones;
        }

        public IEnumerable<Customer> DisplayCustomersList()
        {
            List<Customer> customers = new();
            for (int i = 0; i < DataSource.Customers.Count; i++)
            {
                customers.Add(DataSource.Customers[i]);
            }
            return customers;
        }

        public IEnumerable<Package> DisplayPackagesList()
        {
            List<Package> packages = new();
            for (int i = 0; i < DataSource.Packages.Count; i++)
            {
                packages.Add(DataSource.Packages[i]);
            }
            return packages;
        }

        public IEnumerable<Package> DisplayUnassignedPackagesList()
        {
            List<Package> packages = new();
            for (int i = 0; i < DataSource.Packages.Count; i++)
            {
                if (DataSource.Packages[i].DroneID == 0)
                {
                    packages.Add(DataSource.Packages[i]);
                }
            }
            return packages;
        }

        public IEnumerable<Station> DisplayUnoccupiedStationsList()
        {
            List<Station> stations = new();
            for (int i = 0; i < DataSource.Stations.Count; i++)
            {
                if (DataSource.Stations[i].NumChargeSlots > 0)
                {
                    stations.Add(DataSource.Stations[i]);
                }
            }
            return stations;
        }

        public double[] PowerConsumption()
        {
            double[] powerConsumptionValues = new double[5];
            powerConsumptionValues[0] = DataSource.Config.Free;
            powerConsumptionValues[1] = DataSource.Config.LightWeight;
            powerConsumptionValues[2] = DataSource.Config.MidWeight;
            powerConsumptionValues[3] = DataSource.Config.HeavyWeight;
            powerConsumptionValues[4] = DataSource.Config.ChargingRate;
            return powerConsumptionValues;
        }
    }
}
