using System;
using System.Collections.Generic;
using IDAL.DO;

namespace IDAL
{
    public interface IDal
    {
        /// <summary>
        /// Add a new station to the array
        /// </summary>
        /// <param name="id">Station id</param>
        /// <param name="name">Station name</param>
        /// <param name="numChargeSlots">Number of free charging slots avaliable</param>
        /// <param name="latitude">Station latitude location</param>
        /// <param name="longitude">Station longitude location</param>
        
        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude);
        
        /// <summary>
        /// Add a new drone to the drone array
        /// </summary>
        /// <param name="id">Drone ID</param>
        /// <param name="model">Drone model</param>
        /// <param name="maxWeight">Maximum weight the drone can handle</param>
        /// <param name="status">Drone status</param>
        /// <param name="batteryLevel">Drone battery level</param>
        public void AddDrone(int id, string model, IDAL.DO.Enums.WeightCategories maxWeight);

        /// <summary> 
        /// Add a new customer to the customer array 
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="name">Customer name</param>
        /// <param name="phone">Customer phone number</param>
        /// <param name="latitude">Customer latitude location</param>
        /// <param name="longitude">Customer longitude location</param>
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude);

        /// <summary>
        /// Add a package that needs to be delivered to the package array
        /// </summary>
        /// <param name="senderID">Package sender ID</param>
        /// <param name="receiverID">Package receiver ID</param>
        /// <param name="weight">Package weight</param>
        /// <param name="priority">Priority of package delivery</param>
        /// <param name="droneID">ID of drone delivering package</param>
        /// <returns>automatic package ID, or -1 if adding a package failed</returns>
        public int AddPackage(int senderID, int receiverID, IDAL.DO.Enums.WeightCategories weight, IDAL.DO.Enums.Priorities priority, int? droneID = null);

        /// <summary>
        /// Assign a package to a drone to deliver
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>true if assigned successfully, false otherwise</returns>
        public void AssignPackage(int packageID, int droneID);

        /// <summary>
        /// Drone collects the assigned package
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>True if collected successfully, false otherwise</returns>
        public void CollectPackage(int packageID, int droneID);

        /// <summary>
        /// Drone delivers a package to the customer
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>True if delivered successfully, false otherwise</returns>
        public void DeliverPackage(int packageID, int droneID);

        /// <summary>
        /// Send a drone to charge in a base station 
        /// </summary>
        /// <param name="droneID">drone ID</param>
        /// <param name="stationID">station ID</param>
        /// <returns>True if success ,else false</returns>
        public void ChargeDrone(int droneID, int stationID);        

        /// <summary>
        /// Release drone from a charging station
        /// </summary>
        /// <param name="droneID">Drone ID</param>
        /// <param name="stationID">Station ID</param>
        /// <returns>True if released successfully, false otherwise</returns>        
        public void ReleaseDroneFromCharging(int droneID, int stationID);

        /// <summary>
        /// Update the model of the drone
        /// </summary>
        /// <param name="droneID">drone ID</param>
        /// <param name="model">drone model</param>
        public void UpdateDroneModel(int droneID, string model);

        /// <summary>
        /// Update the name of the customer
        /// </summary>
        /// <param name="customerID">customer ID</param>
        /// <param name="name">customer name</param>
        public void UpdateCustomerName(int customerID, string name);

        /// <summary>
        /// Update the phone number of the customer
        /// </summary>
        /// <param name="customerID">customer ID</param>
        /// <param name="phone">customer phone number</param>
        public void UpdateCustomerPhone(int customerID, string phone);

        /// <summary>
        /// Update the name of the station
        /// </summary>
        /// <param name="stationID">station ID</param>
        /// <param name="name">station name</param>
        public void UpdateStationName(int stationID, int name);

        /// <summary>
        /// Update the number of availbale charging slots at the station
        /// </summary>
        /// <param name="stationID">station ID</param>
        /// <param name="availableChargingSlots">number of available charging slots at the station</param>
        public void UpdateStationChargeSlots(int stationID, int availableChargingSlots);

        /// <summary>
        /// Remove a package from the list
        /// </summary>
        /// <param name="packageID">package ID</param>
        public void RemovePackage(int packageID);

        /// <summary>
        /// Modify the assignment, collection, and delivery times of a package
        /// </summary>
        /// <param name="packageID">package ID</param>
        public void ModifyPackageStatus(int packageID, DateTime? assigned, DateTime? collected, DateTime? delivered);

        /// <summary>
        /// Display a specific station to the user
        /// </summary>
        /// <param name="stationID">Station ID</param>
        /// <returns>The station with this ID</returns>
        public Station DisplayStation(int stationID);

        /// <summary>
        /// Display a specific drone to the user
        /// </summary>
        /// <param name="droneID">Drone ID</param>
        /// <returns>The drone with this ID</returns>
        public Drone DisplayDrone(int droneID);

        /// <summary>
        /// Display a specific customer to the user
        /// </summary>
        /// <param name="customerID">Customer ID</param>
        /// <return>The customer with this ID</return>
        public Customer DisplayCustomer(int customerID);

        /// <summary>
        /// Display a specific package to the user
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <return>The package with this ID</return> 
        public Package DisplayPackage(int packageID);

        /// <summary>
        /// Display all stations to the user
        /// </summary>
        /// <returns>List of all stations</returns>
        public IEnumerable<Station> DisplayStationsList();

        /// <summary>
        /// Display all drones to the user
        /// </summary>
        /// <returns>List of all drones</returns>
        public IEnumerable<Drone> DisplayDronesList();

         /// <summary>
        /// Display all customers to the user
        /// </summary>
        /// <returns>List of all customers</returns>
        public IEnumerable<Customer> DisplayCustomersList();

        /// <summary>
        /// Display all packages to the user 
        /// </summary>
        /// <returns>List of all packages</returns>
        public IEnumerable<Package> DisplayPackagesList();

        /// <summary>
        /// Display all packages according to a given predicate
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>List of all appropriate packages</returns>
        public IEnumerable<Package> FindPackages(Predicate<Package> predicate);

        /// <summary>
        /// Display all stations according to a given predicate
        /// </summary>
        /// <param name="predicate">predicate</param>
        /// <returns>List of all appropriate stations</returns>
        public IEnumerable<Station> FindStations(Predicate<Station> predicate);

        /// <summary>
        /// Represents the statistics of a drone's power consumption 
        /// </summary>
        /// <returns>array of doubles for how much power is consumed for different tasks</returns>
        public double[] DronePowerConsumption();
    }
}
