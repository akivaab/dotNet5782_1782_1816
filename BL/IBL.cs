using System;
using System.Collections.Generic;
using IBL.BO;


namespace IBL
{
    public interface IBL
    {
        /// <summary>
        /// Add a station to the system.
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="numAvailableChargingSlots"></param>
        /// <returns>The newly added station</returns>
        public Station AddStation(int stationID, int name, Location location, int numAvailableChargingSlots);
        
        /// <summary>
        /// Add a drone to the system.
        /// </summary>
        /// <param name="droneID"></param>
        /// <param name="model"></param>
        /// <param name="maxWeight">maximum weight this drone can lift</param>
        /// <param name="stationID">the station in which the drone is first charged</param>
        /// <returns>The newly added drone</returns>
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories maxWeight, int stationID);
        
        /// <summary>
        /// Add a customer to the system. 
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="location"></param>
        /// <returns>The newly added customer</returns>
        public Customer AddCustomer(int customerID, string name, string phone, Location location);

        /// <summary>
        /// Add a package to the system.
        /// </summary>
        /// <param name="senderID"></param>
        /// <param name="receiverID"></param>
        /// <param name="weight"></param>
        /// <param name="priority"></param>
        /// <returns>The newly added package</returns>
        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority);

        /// <summary>
        /// Update the model of a drone.
        /// </summary>
        /// <param name="droneID"></param>
        /// <param name="model"></param>
        public void UpdateDroneModel(int droneID, string model);

        /// <summary>
        /// Update a station's information.
        /// </summary>
        /// <param name="stationID"></param>
        /// <param name="name"></param>
        /// <param name="numChargingSlots">total number of charging slots</param>
        public void UpdateStation(int stationID, int name = -1, int numChargingSlots = -1);

        /// <summary>
        /// Update a customer's information.
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        public void UpdateCustomer(int customerID, string name = "", string phone= "");

        /// <summary>
        /// Send a drone to a station to charge.
        /// </summary>
        /// <param name="droneID"></param>
        public void SendDroneToCharge(int droneID);

        /// <summary>
        /// Release a drone from charging after some period of time.
        /// </summary>
        /// <param name="droneID"></param>
        /// <param name="chargingTimeInHours">amount of time the drone was charging for in hours</param>
        public void ReleaseFromCharge(int droneID, double chargingTimeInHours);

        /// <summary>
        /// Assign a package to a drone.
        /// </summary>
        /// <param name="droneID"></param>
        public void AssignPackage(int droneID);

        /// <summary>
        /// Have a drone collect the package assigned to it.
        /// </summary>
        /// <param name="droneID"></param>
        public void CollectPackage(int droneID);

        /// <summary>
        /// Have a drone deliver the package it collected.
        /// </summary>
        /// <param name="droneID"></param>
        public void DeliverPackage(int droneID);

        /// <summary>
        /// Finds a station by it's ID.
        /// </summary>
        /// <param name="stationID"></param>
        /// <returns>The corresponding station</returns>
        public Station DisplayStation(int stationID);

        /// <summary>
        /// Finds a station by it's ID.
        /// </summary>
        /// <param name="droneID"></param>
        /// <returns>The corresponding drone</returns>
        public Drone DisplayDrone(int droneID);

        /// <summary>
        /// Finds a customer by it's ID.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns>The corresponding customer</returns>
        public Customer DisplayCustomer(int customerID);

        /// <summary>
        /// Finds a package by it's ID.
        /// </summary>
        /// <param name="packageID"></param>
        /// <returns>The corresponding package</returns>
        public Package DisplayPackage(int packageID);

        /// <summary>
        /// Display a list of all the stations in the system.
        /// </summary>
        /// <returns>List of StationToList entities</returns>
        public List<StationToList> DisplayAllStations();

        /// <summary>
        /// Display a list of all the drones in the system.
        /// </summary>
        /// <returns>List of DroneToList entities</returns>
        public List<DroneToList> DisplayAllDrones();

        /// <summary>
        /// Display a list of all the customers in the system.
        /// </summary>
        /// <returns>List of CustomerToList entities</returns>
        public List<CustomerToList> DisplayAllCustomers();

        /// <summary>
        /// Display a list of all the packages in the system.
        /// </summary>
        /// <returns>List of PackageToList entities</returns>
        public List<PackageToList> DisplayAllPackages();

        /// <summary>
        /// Display a list of all the packages that have not yet been assigned to a drone.
        /// </summary>
        /// <returns>List of PackageToList entities</returns>
        public List<PackageToList> DisplayAllUnassignedPackages();

        /// <summary>
        /// Display a list of all stations that have available charging slots.
        /// </summary>
        /// <returns>List of StationToList entities</returns>
        public List<StationToList> DisplayFreeStations();
    }
}
