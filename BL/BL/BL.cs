using System;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    /// <summary>
    /// Initialization and Singleton implementation of the Business Layer.
    /// </summary>
    sealed partial class BL : BlApi.IBL
    {
        /// <summary>
        /// Lazy and implicitly thread-safe initialization of a BL object.
        /// </summary>
        private static readonly Lazy<BL> lazyBl = new Lazy<BL>(() => new BL());
        
        /// <summary>
        /// Instance of the BL object that is first instantiated when the getter is called.
        /// </summary>
        internal static BL instance { get { return lazyBl.Value; } }
        
        /// <summary>
        /// List of DroneToList entities.
        /// </summary>
        private List<DroneToList> Drones;
        
        /// <summary>
        /// Instance of the DalObject class.
        /// </summary>
        private DalApi.IDal DalObject;
        
        /// <summary>
        /// Array of the values related to the battery usage of drones while carrying packages of varying weights.
        /// </summary>
        private double[] PowerConsumption;
        
        /// <summary>
        /// The amount a drone battery charges per hour.
        /// </summary>
        private double ChargeRatePerHour;

        /// <summary>
        /// Connstructor of BL class, private to maintain Singleton design pattern.
        /// </summary>
        private BL()
        {
            //initialize fields
            Drones = new();
            DalObject = DalApi.DalFactory.GetDal("DalObject");
            double[] powerConsumption = (double[])DalObject.DronePowerConsumption();
            PowerConsumption = new double[4];
            Array.Copy(powerConsumption, PowerConsumption, 4);
            ChargeRatePerHour = powerConsumption[4];

            //remove problematic entities from the data layer
            dataCleanup();
            
            List<DO.Drone> dalDrones = DalObject.DisplayDronesList().ToList();

            //find all packages undelivered but with a drone assigned
            List<DO.Package> dalPackages = DalObject.FindPackages(p => p.Delivered == null && p.DroneID != null).ToList();
            
            foreach (DO.Package package in dalPackages)
            {
                DO.Drone drone = dalDrones.Find(drone => drone.ID == package.DroneID);
                dalDrones.Remove(drone);

                Location droneLocation = new();

                //if package assigned but not collected
                if (package.Assigned != null && package.Collected == null) 
                {
                    //set station closest to sender as drone location
                    droneLocation = getClosestStation(getCustomerLocation(package.SenderID)); 
                }
                //if package is collected
                else if (package.Collected != null) 
                {
                    //set sender location as drone location
                    droneLocation = getCustomerLocation(package.SenderID);
                }

                //get random battery level
                double battery = randomBatteryPower(droneLocation, package, powerConsumption[(int)package.Weight]);
                    
                DroneToList droneToList = new(drone.ID, drone.Model, (Enums.WeightCategories)drone.MaxWeight, battery, Enums.DroneStatus.delivery, droneLocation, package.ID);
                Drones.Add(droneToList);
            }

            Random random = new Random();

            //for remaining drones that are not delivering
            foreach (DO.Drone drone in dalDrones) 
            {
                DroneToList droneToList = new();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.MaxWeight = (Enums.WeightCategories)drone.MaxWeight;

                //randomly choose state of drone
                int randInt = random.Next(1, 3);
                if (randInt == 1)
                {
                    droneToList.Status = Enums.DroneStatus.maintenance;

                    //get random station as drone location
                    List<DO.Station> dalStations = DalObject.DisplayStationsList().ToList();
                    int randStation = random.Next(dalStations.Count);
                    droneToList.Location = new(dalStations[randStation].Latitude, dalStations[randStation].Longitude);
                    
                    //get random battery level 0%-20%
                    droneToList.Battery = random.Next(21);

                    //create appropriate DroneCharge entity in data layer
                    DalObject.ChargeDrone(drone.ID, dalStations[randStation].ID);
                }
                else if (randInt == 2)
                {
                    droneToList.Status = Enums.DroneStatus.available;
                    
                    //get random customer that received a package as drone location
                    DO.Customer customer = randomPackageReceiver();
                    droneToList.Location = new(customer.Latitude, customer.Longitude);
                    
                    //get random battery level
                    droneToList.Battery = random.Next((int)Math.Ceiling(powerConsumption[(int)Enums.WeightCategories.free] * getDistance(droneToList.Location, getClosestStation(droneToList.Location))), 101);
                }

                //no package assigned
                droneToList.PackageID = null; 

                Drones.Add(droneToList);
            }
        }
    }
}
