using System;
using System.Collections.Generic;

namespace DalApi
{
    /// <summary>
    /// Interface for the DalObject and DalXml classes, containing all functions the BL could call.
    /// </summary>
    public interface IDal
    {
        #region Add Methods

        /// <summary>
        /// Add a new station to the system.
        /// </summary>
        /// <param name="id">The station ID.</param>
        /// <param name="name">The station name.</param>
        /// <param name="numChargeSlots">The number of charging slots avaliable.</param>
        /// <param name="latitude">The station latitude coordinates.</param>
        /// <param name="longitude">The station longitude coordinates.</param>

        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude);
        
        /// <summary>
        /// Add a new drone to the system.
        /// </summary>
        /// <param name="id">The drone ID.</param>
        /// <param name="model">The drone model.</param>
        /// <param name="maxWeight">The maximum weight the drone can handle.</param>
        /// <param name="status">The drone status.</param>
        /// <param name="batteryLevel">The battery level of the drone.</param>
        public void AddDrone(int id, string model, DO.Enums.WeightCategories maxWeight);

        /// <summary> 
        /// Add a new customer to the system.
        /// </summary>
        /// <param name="id">The customer's ID.</param>
        /// <param name="name">The customer's name.</param>
        /// <param name="phone">The customer's phone number.</param>
        /// <param name="latitude">The customer's latitude coordinates.</param>
        /// <param name="longitude">The customer's longitude coordinate.</param>
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude);

        /// <summary>
        /// Add a new package to the system. 
        /// </summary>
        /// <param name="senderID">The ID of the package sender.</param>
        /// <param name="receiverID">The ID of the package receiver.</param>
        /// <param name="weight">The weight of the package.</param>
        /// <param name="priority">The priority of the package.</param>
        /// <param name="droneID">The ID of the drone assigned to the package.</param>
        /// <returns>automatic package ID, or -1 if adding a package failed</returns>
        public int AddPackage(int senderID, int receiverID, DO.Enums.WeightCategories weight, DO.Enums.Priorities priority, int? droneID = null);

        #endregion

        #region Update Methods

        /// <summary>
        /// Assign a package to a drone.
        /// </summary>
        /// <param name="packageID">The package ID.</param>
        /// <param name="droneID">The drone ID.</param>
        public void AssignPackage(int packageID, int droneID);

        /// <summary>
        /// Have a drone collect a package.
        /// </summary>
        /// <param name="packageID">The package ID.</param>
        /// <param name="droneID">The drone ID.</param>
        public void CollectPackage(int packageID, int droneID);

        /// <summary>
        /// Modify the assignment, collection, and delivery times of a package.
        /// </summary>
        /// <param name="packageID">The apckage ID.</param>
        /// <param name="assigned">The new assignment time.</param>
        /// <param name="collected">The new collection time.</param>
        /// <param name="delivered">The new delivery time.</param>
        public void ModifyPackageStatus(int packageID, DateTime? assigned, DateTime? collected, DateTime? delivered);

        /// <summary>
        /// Have a drone deliver a package.
        /// </summary>
        /// <param name="packageID">The package ID.</param>
        /// <param name="droneID">The drone ID.</param>
        public void DeliverPackage(int packageID, int droneID);

        /// <summary>
        /// Send a drone to charge in a station.
        /// </summary>
        /// <param name="droneID">The drone ID.</param>
        /// <param name="stationID">The station ID.</param>
        public void ChargeDrone(int droneID, int stationID);

        /// <summary>
        /// Release a drone from a charging station.
        /// </summary>
        /// <param name="droneID">The drone ID.</param>
        /// <param name="stationID">The station ID.</param>
        public void ReleaseDroneFromCharging(int droneID, int stationID);

        /// <summary>
        /// Update the model of a drone.
        /// </summary>
        /// <param name="droneID">The drone ID.</param>
        /// <param name="model">The new drone model.</param>
        public void UpdateDroneModel(int droneID, string model);

        /// <summary>
        /// Update the name of a customer.
        /// </summary>
        /// <param name="customerID">The customer ID.</param>
        /// <param name="name">The new customer name.</param>
        public void UpdateCustomerName(int customerID, string name);

        /// <summary>
        /// Update the phone number of a customer.
        /// </summary>
        /// <param name="customerID">The customer ID.</param>
        /// <param name="phone">The customer's new phone number</param>
        public void UpdateCustomerPhone(int customerID, string phone);

        /// <summary>
        /// Update the password of a customer.
        /// </summary>
        /// <param name="customerID">The customer ID.</param>
        /// <param name="password">The customer's new password</param>
        public void UpdateCustomerPassword(int customerID, string password);

        /// <summary>
        /// Update the name of a station.
        /// </summary>
        /// <param name="stationID">The station ID.</param>
        /// <param name="name">The new station name.</param>
        public void UpdateStationName(int stationID, int name);

        /// <summary>
        /// Update the number of available charging slots at a station.
        /// </summary>
        /// <param name="stationID">The station ID.</param>
        /// <param name="availableChargingSlots">The number of available charging slots at the station.</param>
        public void UpdateStationChargeSlots(int stationID, int availableChargingSlots);

        #endregion

        #region Remove Methods

        /// <summary>
        /// Remove a station from the system.
        /// </summary>
        /// <param name="stationID">The station ID.</param>
        public void RemoveStation(int stationID);

        /// <summary>
        /// Remove a drone from the system.
        /// </summary>
        /// <param name="droneID">The drone ID.</param>
        public void RemoveDrone(int droneID);

        /// <summary>
        /// Remove a customer from the system.
        /// </summary>
        /// <param name="customerID">The customer ID.</param>
        public void RemoveCustomer(int customerID);

        /// <summary>
        /// Remove a package from the system.
        /// </summary>
        /// <param name="packageID"></param>
        public void RemovePackage(int packageID);

        #endregion

        #region Getter Methods - Single Entity

        /// <summary>
        /// Get a single station.
        /// </summary>
        /// <param name="stationID">The Station ID.</param>
        /// <returns>The station with this ID.</returns>
        public DO.Station GetStation(int stationID);

        /// <summary>
        /// Get a single drone.
        /// </summary>
        /// <param name="droneID">The Drone ID.</param>
        /// <returns>The drone with this ID.</returns>
        public DO.Drone GetDrone(int droneID);

        /// <summary>
        /// Get a single customer.
        /// </summary>
        /// <param name="customerID">The customer ID.</param>
        /// <return>The customer with this ID.</return>
        public DO.Customer GetCustomer(int customerID);

        /// <summary>
        /// Get a single package.
        /// </summary>
        /// <param name="packageID">The package ID.</param>
        /// <return>The package with this ID.</return> 
        public DO.Package GetPackage(int packageID);

        /// <summary>
        /// Get the time a drone began charging in a station.
        /// </summary>
        /// <param name="droneID">The drone ID.</param>
        /// <returns>DateTime the drone began charging.</returns>
        public DateTime GetTimeChargeBegan(int droneID);

        /// <summary>
        /// Get the customer password.
        /// </summary>
        /// <param name="CustomerID">The customer ID.</param>
        /// <returns>The customer password.</returns>
        public string GetCustomerPassword(int customerID);

        #endregion

        #region Getter Methods - Entity Collection

        /// <summary>
        /// Get all stations.
        /// </summary>
        /// <returns>List of all stations</returns>
        public IEnumerable<DO.Station> GetStationsList();

        /// <summary>
        /// Get all drones in the system.
        /// </summary>
        /// <returns>A collection of all drones.</returns>
        public IEnumerable<DO.Drone> GetDronesList();

        /// <summary>
        /// Get all customers in the system.
        /// </summary>
        /// <returns>A collection of all customers.</returns>
        public IEnumerable<DO.Customer> GetCustomersList();

        /// <summary>
        /// Get all packages in the system.
        /// </summary>
        /// <returns>A collection of all packages.</returns>
        public IEnumerable<DO.Package> GetPackagesList();

        /// <summary>
        /// Represents the statistics of a drone's power consumption.
        /// </summary>
        /// <returns>A collection of doubles for how much power is consumed for different tasks.</returns>
        public IEnumerable<double> DronePowerConsumption();

        #endregion

        #region Find Methods

        /// <summary>
        /// Find all packages according to a given predicate.
        /// </summary>
        /// <param name="predicate">Predicate used as a search parameter.</param>
        /// <returns>A collection of all appropriate packages.</returns>
        public IEnumerable<DO.Package> FindPackages(Predicate<DO.Package> predicate);

        /// <summary>
        /// Find all stations according to a given predicate.
        /// </summary>
        /// <param name="predicate">Predicate used as a search parameter.</param>
        /// <returns>A collection of all appropriate stations.</returns>
        public IEnumerable<DO.Station> FindStations(Predicate<DO.Station> predicate);

        /// <summary>
        /// Find all customers according to a given predicate.
        /// </summary>
        /// <param name="predicate">Predicate used as a search parameter.</param>
        /// <returns>A collection of all appropriate customers.</returns>
        public IEnumerable<DO.Customer> FindCustomers(Predicate<DO.Customer> predicate);

        #endregion
    }
}
