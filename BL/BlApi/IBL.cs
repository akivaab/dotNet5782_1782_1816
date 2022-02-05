using System;
using System.Collections.Generic;
using BO;

namespace BlApi
{
    /// <summary>
    /// Interface for the BL class, containing all functions the PL could call.
    /// </summary>
    public interface IBL
    {
        #region Add Methods
        /// <summary>
        /// Add a station to the system.
        /// </summary>
        /// <param name="stationID">The ID of the station being added.</param>
        /// <param name="name">The name of the station being added.</param>
        /// <param name="location">The location of the station being added.</param>
        /// <param name="numAvailableChargingSlots">The number of available charging slots in the station being added.</param>
        /// <returns>The newly added station.</returns>
        /// <exception cref="IllegalArgumentException">An argument passed is invalid.</exception>
        /// <exception cref="NonUniqueIdException">The given ID is not unique.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Station AddStation(int stationID, int name, Location location, int numAvailableChargingSlots);

        /// <summary>
        /// Add a drone to the system.
        /// </summary>
        /// <param name="droneID">The ID of the drone being added.</param>
        /// <param name="model">The model of the drone being added.</param>
        /// <param name="maxWeight">The maximum weight the drone being added can lift.</param>
        /// <param name="stationID">The station in which the drone being added is first charged.</param>
        /// <returns>The newly added drone.</returns>
        /// <exception cref="NonUniqueIdException">The given ID is not unique.</exception>
        /// <exception cref="UnableToChargeException">No available charge slots to place drone.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories maxWeight, int stationID);

        /// <summary>
        /// Add a customer to the system. 
        /// </summary>
        /// <param name="customerID">The ID of the customer being added.</param>
        /// <param name="name">The name of the customer being added.</param>
        /// <param name="phone">The phone number of the customer being added.</param>
        /// <param name="location">The location of the customer being added.</param>
        /// <returns>The newly added customer.</returns>
        /// <exception cref="IllegalArgumentException">An argument passed is invalid.</exception>
        /// <exception cref="NonUniqueIdException">The given ID is not unique.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Customer AddCustomer(int customerID, string name, string phone, Location location);

        /// <summary>
        /// Add a package to the system.
        /// </summary>
        /// <param name="senderID">The ID of the customer sending the package being added.</param>
        /// <param name="receiverID">The ID of the customer receiving the package being added.</param>
        /// <param name="weight">The weight of the package being added.</param>
        /// <param name="priority">The prioroty of the package being added.</param>
        /// <returns>The newly added package.</returns>
        /// <exception cref="IllegalArgumentException">An argument passed is invalid.</exception>
        /// <exception cref="UndefinedObjectException">Either the sender or receiver given do not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority);
        #endregion

        #region Update Methods
        /// <summary>
        /// Update the model of a drone.
        /// </summary>
        /// <param name="droneID">The ID drone of the drone being updated.</param>
        /// <param name="model">The updated model of the drone.</param>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void UpdateDroneModel(int droneID, string model);

        /// <summary>
        /// Update a station's information.
        /// </summary>
        /// <param name="stationID">The ID of the station being updated.</param>
        /// <param name="name">The updated name of the station.</param>
        /// <param name="numChargingSlots">The updated total number of charging slots in the station.</param>
        /// <exception cref="IllegalArgumentException">There cannot be less charging slots than charging drones.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void UpdateStation(int stationID, int name = -1, int numChargingSlots = -1);

        /// <summary>
        /// Update a customer's information.
        /// </summary>
        /// <param name="customerID">The ID of the customer being updated.</param>
        /// <param name="name">The updated name of the customer.</param>
        /// <param name="phone">The updated phone number of the customer.</param>
        /// <exception cref="IllegalArgumentException">An argument passed is invalid.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void UpdateCustomer(int customerID, string name = "", string phone= "");

        /// <summary>
        /// Update a customer's password.
        /// </summary>
        /// <param name="customerID">The ID of the customer being updated.</param>
        /// <param name="password">The new password.</param>     
        /// <exception cref="IllegalArgumentException">An argument passed is invalid.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void UpdateCustomerPassword(int customerID, string password);

        /// <summary>
        /// Send a drone to a station to charge.
        /// </summary>
        /// <param name="droneID">The ID of the drone being sent to charge.</param>
        /// <exception cref="UnableToChargeException">Cannot send the drone to charge.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void SendDroneToCharge(int droneID);

        /// <summary>
        /// Release a drone from charging after some period of time.
        /// </summary>
        /// <param name="droneID">The ID of the drone being released.</param>
        /// <param name="chargingTimeInHours">The amount of time the drone was charging for in hours.</param>
        /// <exception cref="UnableToReleaseException">Cannot release the drone.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void ReleaseFromCharge(int droneID, double chargingTimeInHours);

        /// <summary>
        /// Assign a package to a drone.
        /// </summary>
        /// <param name="droneID">The ID of the drone being assigned a package.</param>
        /// <exception cref="UnableToAssignException">Cannot assign the drone to a package.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void AssignPackage(int droneID);

        /// <summary>
        /// Have a drone collect the package assigned to it.
        /// </summary>
        /// <param name="droneID">The ID of the drone being sent to collect its package.</param>
        /// <exception cref="UnableToCollectException">The drone cannot collect its package.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void CollectPackage(int droneID);

        /// <summary>
        /// Have a drone deliver the package it collected.
        /// </summary>
        /// <param name="droneID">The ID of the drone being sent to deliver its package.</param>
        /// <exception cref="UnableToDeliverException">The drone cannot deliver its package.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void DeliverPackage(int droneID);
        #endregion

        #region Remove Methods
        /// <summary>
        /// Remove a station from the system.
        /// </summary>
        /// <param name="stationID">The station ID.</param>
        /// <exception cref="UnableToRemoveException">Cannot remove the station.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void RemoveStation(int stationID);

        /// <summary>
        /// Remove a drone from the system.
        /// </summary>
        /// <param name="droneID">The drone ID.</param>
        /// <exception cref="UnableToRemoveException">Cannot remove the drone.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void RemoveDrone(int droneID);

        /// <summary>
        /// Remove a customer from the system.
        /// </summary>
        /// <param name="customerID">The customer ID.</param>
        /// <exception cref="UnableToRemoveException">Cannot remove the customer.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void RemoveCustomer(int customerID);

        /// <summary>
        /// Remove a package from the system.
        /// </summary>
        /// <param name="packageID">The package ID.</param>
        /// <exception cref="UnableToRemoveException">Cannot remove the package.</exception>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public void RemovePackage(int packageID);
        #endregion

        #region Getter Methods - Single Entity
        /// <summary>
        /// Find a station by its ID.
        /// </summary>
        /// <param name="stationID">The ID of the station to be found.</param>
        /// <returns>The corresponding station.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Station GetStation(int stationID);

        /// <summary>
        /// Find a drone by it's ID.
        /// </summary>
        /// <param name="droneID">The ID of the drone to be found.</param>
        /// <returns>The corresponding drone.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Drone GetDrone(int droneID);

        /// <summary>
        /// Find a customer by it's ID.
        /// </summary>
        /// <param name="customerID">The ID of the customer to be found.</param>
        /// <returns>The corresponding customer.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Customer GetCustomer(int customerID);

        /// <summary>
        /// Find a package by it's ID.
        /// </summary>
        /// <param name="packageID">The ID of the package to be found.</param>
        /// <returns>The corresponding package.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public Package GetPackage(int packageID);

        /// <summary>
        /// Get the time a drone began charging in a station.
        /// </summary>
        /// <param name="droneID">The ID of the drone.</param>
        /// <returns>The DateTime the drone began charging.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public DateTime GetTimeChargeBegan(int droneID);

        /// <summary>
        /// Get a customer's password.
        /// </summary>
        /// <param name="customerID">The ID of the customer.</param>
        /// <returns>The customer's password.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public string GetCustomerPassword(int customerID);
        #endregion

        #region Getter Methods - Entity Collection
        /// <summary>
        /// Get a collection of all the stations in the system.
        /// </summary>
        /// <returns>A collection of StationToList entities.</returns>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public IEnumerable<StationToList> GetStationsList();

        /// <summary>
        /// Get a collection of all the drones in the system.
        /// </summary>
        /// <returns>A collection of DroneToList entities.</returns>
        public IEnumerable<DroneToList> GetDronesList();

        /// <summary>
        /// Get a collection of all the customers in the system.
        /// </summary>
        /// <returns>A collection of CustomerToList entities.</returns>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public IEnumerable<CustomerToList> GetCustomersList();

        /// <summary>
        /// Get a collection of all the packages in the system.
        /// </summary>
        /// <returns>A collection of PackageToList entities.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public IEnumerable<PackageToList> GetPackagesList();
        #endregion

        #region Find Methods
        /// <summary>
        /// Get a collection of all the packages according to a certain predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the packages.</param>
        /// <returns>A collection of PackageToList entities.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public IEnumerable<PackageToList> FindPackages(Predicate<DO.Package> predicate);

        /// <summary>
        /// Get a collection of all stations according to a certain predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the stations.</param>
        /// <returns>A collection of StationToList entities</returns>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public IEnumerable<StationToList> FindStations(Predicate<DO.Station> predicate);

        /// <summary>
        /// Get a collection of all drones according to a certain predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the drones.</param>
        /// <returns>A collection of DroneToList entities.</returns>
        public IEnumerable<DroneToList> FindDrones(Predicate<DroneToList> predicate);

        /// <summary>
        /// Get a collection of all customers according to a certain predicate.
        /// </summary>
        /// <param name="predicate">The predicate used to filter the customers.</param>
        /// <returns>A collection of CustomerToList entities.</returns>
        /// <exception cref="UndefinedObjectException">Some entity called does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        public IEnumerable<CustomerToList> FindCustomers(Predicate<DO.Customer> predicate);
        #endregion

        /// <summary>
        /// Activate a running simulation of a drone performing its duties.
        /// </summary>
        /// <param name="droneID">The ID of the drone being simulated.</param>
        /// <param name="updateDisplay">Delegate for updating the display.</param>
        /// <param name="stop">Function marking when to stop the simulation.</param>
        public void ActivateSimulator(int droneID, Action<int, IEnumerable<string>> updateDisplay, Func<bool> stop);
    }
}
