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
        #region Fields and Properties
        /// <summary>
        /// Lazy and implicitly thread-safe initialization of a BL object.
        /// </summary>
        private static readonly Lazy<BL> lazyBl = new Lazy<BL>(() => new BL());

        /// <summary>
        /// Instance of the BL object that is first instantiated when the getter is called.
        /// </summary>
        internal static BL instance => lazyBl.Value;

        /// <summary>
        /// List of DroneToList entities.
        /// </summary>
        private List<DroneToList> drones;

        /// <summary>
        /// Instance of the DalObject or DalXml class.
        /// </summary>
        internal DalApi.IDal dal;

        /// <summary>
        /// Collection of the values related to the battery usage of drones while carrying packages of varying weights.
        /// </summary>
        private IEnumerable<double> powerConsumption;

        /// <summary>
        /// The amount a drone battery charges per hour.
        /// </summary>
        private double chargeRatePerSecond;
        #endregion

        #region Constructor (and its Helper Methods)
        /// <summary>
        /// Constructor of BL class, private to maintain Singleton design pattern.
        /// </summary>
        /// <exception cref="UndefinedObjectException">An entity obtained does not exist.</exception>
        /// <exception cref="EmptyListException">There are no stations.</exception>
        /// <exception cref="NonUniqueIdException">There are multiple drones with the same ID.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load XML.</exception>
        /// <exception cref="IllegalArgumentException">The string does not match a given instance.</exception>
        /// <exception cref="InstanceInitializationException">Either the DalObject or DalXml instance was not created properly.</exception>
        /// <exception cref="LinqQueryException">Failed to initialize.</exception>
        private BL()
        {
            try
            {
                //initialize fields
                drones = new();
                dal = DalApi.DalFactory.GetDal("DalXml");
                lock (dal)
                {
                    powerConsumption = dal.DronePowerConsumption().Take(4);
                    chargeRatePerSecond = dal.DronePowerConsumption().Last();

                    //remove problematic entities from the data layer
                    if (dal.DataCleanupRequired)
                    {
                        dataCleanup();
                    }

                    IEnumerable<DO.Drone> dalDrones = dal.GetDronesList();
                    IEnumerable<DO.Drone> unassignedDalDrones = addAssignedDrones(dalDrones);
                    addUnassignedDrones(unassignedDalDrones);
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
            catch (DO.IllegalArgumentException e)
            {
                throw new IllegalArgumentException(e.Message, e);
            }
            catch (DO.InstanceInitializationException e)
            {
                throw new InstanceInitializationException(e.Message, e);
            }
            catch (Exception e) when (isSystemDefinedException(e))
            {
                throw new LinqQueryException("Failed to initialize.");
            }
        }

        /// <summary>
        /// Add assigned drones to the BL Drone List.
        /// </summary>
        /// <param name="dalDrones">Collection of drones from the DAL layer.</param>
        /// <returns>A collection of drones from the DAL layer excluding those assigned a package.</returns>
        /// <exception cref="UndefinedObjectException">An entity obtained does not exist.</exception>
        /// <exception cref="EmptyListException">There are no stations.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="LinqQueryException">Failed to initialize.</exception>
        private IEnumerable<DO.Drone> addAssignedDrones(IEnumerable<DO.Drone> dalDrones)
        {
            try
            {
                lock (dal)
                {
                    //copy to avoid changing input
                    List<DO.Drone> dalDronesCopy = new(dalDrones);

                    //find all packages undelivered but with a drone assigned
                    IEnumerable<DO.Package> dalPackages = dal.FindPackages(p => p.Delivered == null && p.DroneID != null);

                    foreach (DO.Package package in dalPackages)
                    {
                        DO.Drone drone = dalDronesCopy.Find(drone => drone.ID == package.DroneID);
                        dalDronesCopy.Remove(drone);

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
                        double battery = randomBatteryPower(droneLocation, package, powerConsumption.ElementAt((int)package.Weight));

                        DroneToList droneToList = new(drone.ID, drone.Model, (Enums.WeightCategories)drone.MaxWeight, battery, Enums.DroneStatus.delivery, droneLocation, package.ID);
                        drones.Add(droneToList);
                    }
                    return dalDronesCopy;
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
            catch (Exception e) when (isSystemDefinedException(e))
            {
                throw new LinqQueryException("Failed to initialize.");
            }
        }

        /// <summary>
        /// Add unassigned drones to the BL Drone List.
        /// </summary>
        /// <param name="dalDrones">List of drones from the DAL layer.</param>
        /// <exception cref="UndefinedObjectException">An entity obtained does not exist.</exception>
        /// <exception cref="NonUniqueIdException">There are multiple drones with the same ID.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="LinqQueryException">Failed to initialize.</exception>
        private void addUnassignedDrones(IEnumerable<DO.Drone> dalDrones)
        {
            lock (dal)
            {
                if (dal.DataCleanupRequired)
                {
                    randomInitialization(dalDrones);
                }
                else
                {
                    xmlInitialization(dalDrones);
                }
            }
        }

        /// <summary>
        /// Randomly initialize the drones being added to the BL Drone List.
        /// </summary>
        /// <param name="dalDrones">List of drones from the DAL layer.</param>
        /// <exception cref="UndefinedObjectException">An entity obtained does not exist.</exception>
        /// <exception cref="NonUniqueIdException">There are multiple drones with the same ID.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="LinqQueryException">Failed to initialize.</exception>
        private void randomInitialization(IEnumerable<DO.Drone> dalDrones)
        {
            Random random = new Random();

            try
            {
                lock (dal)
                {
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
                            IEnumerable<DO.Station> dalStations = dal.GetStationsList();
                            int randStation = random.Next(dalStations.Count());
                            DO.Station dalStation = dalStations.ElementAt(randStation);
                            droneToList.Location = new(dalStation.Latitude, dalStation.Longitude);

                            //get random battery level 0%-20%
                            droneToList.Battery = random.Next(21);

                            //create appropriate DroneCharge entity in data layer
                            dal.ChargeDrone(drone.ID, dalStation.ID);
                        }
                        else if (randInt == 2)
                        {
                            droneToList.Status = Enums.DroneStatus.available;

                            //get random customer that received a package as drone location
                            DO.Customer customer = randomPackageReceiver();
                            droneToList.Location = new(customer.Latitude, customer.Longitude);

                            //get random battery level
                            droneToList.Battery = random.Next((int)Math.Ceiling(powerConsumption.ElementAt((int)Enums.WeightCategories.free) * getDistance(droneToList.Location, getClosestStation(droneToList.Location))), 101);
                        }

                        //no package assigned
                        droneToList.PackageID = null;
                        drones.Add(droneToList);
                    }
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
            catch (Exception e) when (isSystemDefinedException(e))
            {
                throw new LinqQueryException("Could not initialize.");
            }
        }

        /// <summary>
        /// Initialize the drones being added to the BL Drone List based on data from XML files.
        /// </summary>
        /// <param name="dalDrones">List of drones from the DAL layer.</param>
        /// <exception cref="UndefinedObjectException">An entity obtained does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="LinqQueryException">Failed to initialize.</exception>
        private void xmlInitialization(IEnumerable<DO.Drone> dalDrones)
        {
            Random random = new Random();

            try
            {
                lock (dal)
                {
                    //for remaining drones that are not delivering
                    foreach (DO.Drone drone in dalDrones)
                    {
                        DroneToList droneToList = new();
                        droneToList.ID = drone.ID;
                        droneToList.Model = drone.Model;
                        droneToList.MaxWeight = (Enums.WeightCategories)drone.MaxWeight;

                        //reinstate the appropriate entity data based on the XML files
                        DO.DroneCharge dalDroneCharge = dal.FindDroneCharges(dc => dc.DroneID == drone.ID).SingleOrDefault();
                        if (dalDroneCharge.Equals(default(DO.DroneCharge)))
                        {
                            droneToList.Status = Enums.DroneStatus.available;

                            //get random customer that received a package as drone location (cannot be determined with DalXml data) 
                            DO.Customer customer = randomPackageReceiver();
                            droneToList.Location = new(customer.Latitude, customer.Longitude);

                            //get random battery level (cannot be determined with DalXml data)
                            droneToList.Battery = random.Next((int)Math.Ceiling(powerConsumption.ElementAt((int)Enums.WeightCategories.free) * getDistance(droneToList.Location, getClosestStation(droneToList.Location))), 101);
                        }
                        else
                        {
                            droneToList.Status = Enums.DroneStatus.maintenance;

                            DO.Station stationChargingIn = dal.GetStation(dalDroneCharge.StationID);
                            droneToList.Location = new(stationChargingIn.Latitude, stationChargingIn.Longitude);

                            //get random battery level 0%-20% (cannot be determined with DalXml data)
                            droneToList.Battery = random.Next(21);
                        }

                        //no package assigned
                        droneToList.PackageID = null;
                        drones.Add(droneToList);
                    }
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
            catch (Exception e) when (isSystemDefinedException(e))
            {
                throw new LinqQueryException("Could not initialize.");
            }
        }
        #endregion

        #region Simulator
        public void ActivateSimulator(int droneID, Action<int, IEnumerable<string>> updateDisplay, Func<bool> stop)
        {
            new Simulator(this, droneID, updateDisplay, stop);
        }
        #endregion
    }
}
