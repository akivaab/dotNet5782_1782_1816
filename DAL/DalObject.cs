using System;
using IDAL.DO;

namespace DalObject
{
    public class DalObject
    {
        /// <summary>
        /// Constructor adds initial values to the entity arrays
        /// </summary>
        public DalObject()
        {
            DataSource.Initialize();
        }

        /// <summary>
        /// Add a new station to the array
        /// </summary>
        /// <param name="id">Station id</param>
        /// <param name="name">Station name</param>
        /// <param name="numChargeSlots">Number of free charging slots avaliable</param>
        /// <param name="latitude">Station latitude location</param>
        /// <param name="longitude">Station longitude location</param>
        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            if (DataSource.Stations.Count < 5)
            {
                Station station = new();
                station.ID = id;
                station.Name = name;
                station.NumChargeSlots = numChargeSlots;
                station.Latitude = latitude;
                station.Longitude = longitude;
                DataSource.Stations.Add(station);
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max numbers of stations reached.");
            }
        }

        /// <summary>
        /// Add a new drone to the drone array
        /// </summary>
        /// <param name="id">Drone ID</param>
        /// <param name="model">Drone model</param>
        /// <param name="maxWeight">Maximum weight the drone can handle</param>
        /// <param name="status">Drone status</param>
        /// <param name="batteryLevel">Drone battery level</param>
        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight)
        {
            if (DataSource.Drones.Count < 10)
            {
                Drone drone = new();
                drone.ID = id;
                drone.Model = model;
                drone.MaxWeight = maxWeight;
                DataSource.Drones.Add(drone);
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max numbers of drones reached.");
            }
        }

        /// <summary> 
        /// Add a new customer to the customer array 
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="name">Customer name</param>
        /// <param name="phone">Customer phone number</param>
        /// <param name="latitude">Customer latitude location</param>
        /// <param name="longitude">Customer longitude location</param>
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            if (DataSource.Customers.Count < 100)
            {
                Customer customer = new();
                customer.ID = id;
                customer.Name = name;
                customer.Phone = phone;
                customer.Latitude = latitude;
                customer.Longitude = longitude;
                DataSource.Customers.Add(customer);
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max number of customers reached.");
            }
        }

        /// <summary>
        /// Add a package that needs to be delivered to the package array
        /// </summary>
        /// <param name="senderID">Package sender ID</param>
        /// <param name="receiverID">Package receiver ID</param>
        /// <param name="weight">Package weight</param>
        /// <param name="priority">Priority of package delivery</param>
        /// <param name="droneID">ID of drone delivering package</param>
        /// <returns>automatic package ID, or -1 if adding a package failed</returns>
        public int AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority, int droneID = 0)
        {
            if (DataSource.Packages.Count < 1000)
            {
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
                Console.WriteLine("Success");
                return DataSource.Config.PackageID;
            }
            else
            {
                Console.WriteLine("ERROR: Max number of packages reached.");
                return -1;
            }
        }

        /// <summary>
        /// Assign a package to a drone to deliver
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>true if assigned successfully, false otherwise</returns>
        public bool AssignPackage(int packageID, int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    for (int j = 0; j < DataSource.Config.NextPackage; j++)
                    {
                        if (DataSource.Packages[j].ID == packageID)
                        {
                            DataSource.Packages[j].DroneID = droneID;
                            DataSource.Packages[j].Scheduled = DateTime.Now;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Drone collects the assigned package
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>True if collected successfully, false otherwise</returns>
        public bool CollectPackage(int packageID, int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    if (DataSource.Drones[i].Status == Enums.DroneStatuses.free)
                    {
                        for (int j = 0; j < DataSource.Config.NextPackage; j++)
                        {
                            if (DataSource.Packages[j].ID == packageID)
                            {
                                DataSource.Packages[j].PickedUp = DateTime.Now;
                                DataSource.Drones[i].Status = Enums.DroneStatuses.delivery;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Drone delivers a package to the customer
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>True if delivered successfully, false otherwise</returns>
        public bool DeliverPackage(int packageID, int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    for (int j = 0; j < DataSource.Config.NextPackage; j++)
                    {
                        if (DataSource.Packages[j].ID == packageID)
                        {
                            DataSource.Packages[j].Delivered = DateTime.Now;
                            DataSource.Packages[j].DroneID = 0;
                            DataSource.Drones[i].Status = Enums.DroneStatuses.free;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Send a drone to charge in a base station 
        /// </summary>
        /// <param name="droneID">drone ID</param>
        /// <param name="stationID">station ID</param>
        /// <returns>True if success ,else false</returns>
        public bool ChargeDrone(int droneID, int stationID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    for (int j = 0; j < DataSource.Config.NextStation; j++)
                    {
                        if (DataSource.Stations[j].ID == stationID)
                        {
                            DataSource.Drones[i].Status = Enums.DroneStatuses.maintenance;
                            DataSource.Stations[j].NumChargeSlots--;
                            DroneCharge droneCharge = new DroneCharge();
                            droneCharge.DroneID = droneID;
                            droneCharge.StationID = stationID;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Release drone from a charging station
        /// </summary>
        /// <param name="droneID">Drone ID</param>
        /// <param name="stationID">Station ID</param>
        /// <returns>True if released successfully, false otherwise</returns>        
        public bool ReleaseDroneFromCharging(int droneID, int stationID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    for (int j = 0; j < DataSource.Config.NextStation; j++)
                    {
                        if (DataSource.Stations[j].ID == stationID)
                        {
                            DataSource.Drones[i].Status = Enums.DroneStatuses.free;
                            DataSource.Stations[j].NumChargeSlots++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Display a specific station to the user
        /// </summary>
        /// <param name="stationID">Station ID</param>
        public void DisplayStation(int stationID)
        {
            for (int i = 0; i < DataSource.Config.NextStation; i++)
            {
                if (DataSource.Stations[i].ID == stationID)
                {
                    Console.WriteLine(DataSource.Stations[i]);
                }
            }
        }

        /// <summary>
        /// Display a specific drone to the user
        /// </summary>
        /// <param name="droneID">Drone ID</param>
        public void DisplayDrone(int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    Console.WriteLine(DataSource.Drones[i]);
                }
            }
        }

        /// <summary>
        /// Display a specific customer to the user
        /// </summary>
        /// <param name="customerID">Customer ID</param>
        public void DisplayCustomer(int customerID)
        {
            for (int i = 0; i < DataSource.Config.NextCustomer; i++)
            {
                if (DataSource.Customers[i].ID == customerID)
                {
                    Console.WriteLine(DataSource.Customers[i]);
                }
            }
        }

        /// <summary>
        /// Display a specific package to the user
        /// </summary>
        /// <param name="packageID">Package ID</param>
        public void DisplayPackage(int packageID)
        {
            for (int i = 0; i < DataSource.Config.NextPackage; i++)
            {
                if (DataSource.Packages[i].ID == packageID)
                {
                    Console.WriteLine(DataSource.Packages[i]);
                }
            }
        }

        /// <summary>
        /// Display all stations to the user
        /// </summary>
        public void DisplayStationsList()
        {
            for (int i = 0; i < DataSource.Config.NextStation; i++)
            {
                Console.WriteLine(DataSource.Stations[i]);
            }
        }

        /// <summary>
        /// Display all drones to the user
        /// </summary>
        public void DisplayDronesList()
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                Console.WriteLine(DataSource.Drones[i]);
            }
        }

        /// <summary>
        /// Display all customers to the user
        /// </summary>
        public void DisplayCustomersList()
        {
            for (int i = 0; i < DataSource.Config.NextCustomer; i++)
            {
                Console.WriteLine(DataSource.Customers[i]);
            }
        }

        /// <summary>
        /// Display all packages to the user 
        /// </summary>
        public void DisplayPackagesList()
        {
            for (int i = 0; i < DataSource.Config.NextPackage; i++)
            {
                Console.WriteLine(DataSource.Packages[i]);
            }
        }

        /// <summary>
        /// Display all packages not assigned to a drone
        /// </summary>
        public void DisplayUnassignedPackagesList()
        {
            for (int i = 0; i < DataSource.Config.NextPackage; i++)
            {
                if (DataSource.Packages[i].DroneID == 0)
                {
                    Console.WriteLine(DataSource.Packages[i]);
                }
            }
        }

        /// <summary>
        /// Display all stations with available charge slots
        /// </summary>
        public void DisplayUnoccupiedStationsList()
        {
            for (int i = 0; i < DataSource.Config.NextStation; i++)
            {
                if (DataSource.Stations[i].NumChargeSlots > 0)
                {
                    Console.WriteLine(DataSource.Stations[i]);
                }
            }
        }
    }
}
