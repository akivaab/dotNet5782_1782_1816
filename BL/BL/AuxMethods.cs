﻿using System;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    /// <summary>
    /// Auxiliary methods of the Business Layer.
    /// </summary>
    public partial class BL
    {
        #region Generic Auxiliary Methods
        /// <summary>
        /// Calculates the distance in kilometers between two locations.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <returns>The distance between the locations.</returns>
        internal static double getDistance(Location location1, Location location2)
        {
            double lat1 = location1.Latitude;
            double long1 = location1.Longitude;
            double lat2 = location2.Latitude;
            double long2 = location2.Longitude;

            double equatorialEarthRadius = 6378.1370;
            double dlong = toRadians(long2 - long1);
            double dlat = toRadians(lat2 - lat1);
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(toRadians(lat1)) * Math.Cos(toRadians(lat2)) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = equatorialEarthRadius * c;

            return d;
        }

        /// <summary>
        /// Convert a value in degrees to radians.
        /// </summary>
        /// <param name="degree">The value in degrees to convert.</param>
        /// <returns>The equivalent value in radians.</returns>
        internal static double toRadians(double degree)
        {
            return degree * (Math.PI / 180);
        }

        /// <summary>
        /// Determine if an exception is system defined.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <returns>True if the exception is system-defined, false if user-defined.</returns>
        internal static bool isSystemDefinedException(Exception e)
        {
            return !(e is IllegalArgumentException ||
                e is EmptyListException ||
                e is NonUniqueIdException ||
                e is UndefinedObjectException ||
                e is UnableToChargeException ||
                e is UnableToReleaseException ||
                e is UnableToAssignException ||
                e is UnableToCollectException ||
                e is UnableToDeliverException ||
                e is UnableToRemoveException ||
                e is XMLFileLoadCreateException ||
                e is LinqQueryException ||
                e is InstanceInitializationException);
        }

        /// <summary>
        /// Return a random battery level between the amount a drone needs to deliver a package and return to a station, and 100.
        /// </summary>
        /// <param name="droneLocation">The current location of the drone.</param>
        /// <param name="package">The package being delivered.</param>
        /// <param name="powerConsumed">The amount of battery required for the drone to carry the package one kilometer.</param>
        /// <returns>A random double representing the battery level.</returns>
        /// <exception cref="UndefinedObjectException">A customer did not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="EmptyListException">There are no stations.</exception>
        /// <exception cref="LinqQueryException">Failed to get the closest station.</exception>
        private double randomBatteryPower(Location droneLocation, DO.Package package, double powerConsumed)
        {
            try
            {
                lock (dal)
                {
                    DO.Customer sender = dal.GetCustomer(package.SenderID);
                    DO.Customer receiver = dal.GetCustomer(package.ReceiverID);
                    Location senderLocation = new(sender.Latitude, sender.Longitude);
                    Location receiverLocation = new(receiver.Latitude, receiver.Longitude);

                    //distance needed to deliver is from the drone's current location to the sender, to the receiver, to the nearest station
                    double distanceToDeliver = getDistance(droneLocation, senderLocation);
                    distanceToDeliver += getDistance(senderLocation, receiverLocation);
                    distanceToDeliver += getDistance(receiverLocation, getClosestStation(receiverLocation));

                    Random random = new Random();
                    return random.Next((int)Math.Ceiling(distanceToDeliver * powerConsumed), 101);
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
        }
        #endregion

        #region Auxiliary Station Methods
        /// <summary>
        /// Find the station closest to some location.
        /// </summary>
        /// <param name="location">The location for which the method finds the nearest station.</param>
        /// <returns>The location of the station closest to the location given.</returns>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="EmptyListException">There are no stations.</exception>
        /// <exception cref="LinqQueryException">Failed to get the closest station.</exception>
        private Location getClosestStation(Location location)
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Station> dalStations = dal.GetStationsList();
                    return getClosestStation(location, dalStations);
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }

        /// <summary>
        /// Of the stations provided, find the station closest to some location.
        /// </summary>
        /// <param name="location">The location for which the method finds the nearest station.</param>
        /// <param name="stations">The collection of stations considered by the method.</param>
        /// <returns>The location of the closest station to the location given.</returns>
        /// <exception cref="EmptyListException">There are no stations.</exception>
        /// <exception cref="LinqQueryException">Failed to get the closest station.</exception>
        private Location getClosestStation(Location location, IEnumerable<DO.Station> stations)
        {
            try
            {
                lock (dal)
                {
                    if (stations.Count() == 0)
                    {
                        throw new EmptyListException("The function recieved an empty list.");
                    }

                    IEnumerable<(Location, double)> stationLocDistPairs = from DO.Station station in stations
                                                                          let stationLocation = new Location(station.Latitude, station.Longitude)
                                                                          let distance = getDistance(location, stationLocation)
                                                                          select (stationLocation, distance);

                    double minDist = stationLocDistPairs.Select(pair => pair.Item2).Min();
                    return stationLocDistPairs.Where(pair => pair.Item2 == minDist).Select(pair => pair.Item1).First();
                }
            }
            catch (Exception)
            {
                throw new LinqQueryException("Could not find a nearby station.");
            }
        }

        /// <summary>
        /// Get a collection of available stations that a drone carrying no weight is capable of reaching before running out of battery. 
        /// </summary>
        /// <param name="drone">A drone not carrying any package.</param>
        /// <returns>A collection of stations that the drone is capable of reaching.</returns>
        /// <exception cref="LinqQueryException">Failed to get reachable station.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        private IEnumerable<DO.Station> getReachableStations(DroneToList drone)
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Station> availableStations = dal.FindStations(s => s.AvailableChargeSlots > 0);

                    IEnumerable<DO.Station> reachableStations = from DO.Station station in availableStations
                                                                let stationLocation = new Location(station.Latitude, station.Longitude)
                                                                let requiredBattery = powerConsumption.ElementAt((int)Enums.WeightCategories.free) * getDistance(drone.Location, stationLocation)
                                                                where requiredBattery <= drone.Battery
                                                                select station;
                    return reachableStations;
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
            catch (Exception)
            {
                throw new LinqQueryException("Could not find a reachable station.");
            }
        }
        #endregion

        #region Auxiliary Customer Methods
        /// <summary>
        /// Find the location of a customer.
        /// </summary>
        /// <param name="customerID">ID of the customer.</param>
        /// <returns>The location of the customer.</returns>
        /// <exception cref="UndefinedObjectException">The customer given does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        private Location getCustomerLocation(int customerID)
        {
            try
            {
                lock (dal)
                {
                    DO.Customer dalCustomer = dal.GetCustomer(customerID);
                    Location customerLocation = new(dalCustomer.Latitude, dalCustomer.Longitude);
                    return customerLocation;
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
        }

        /// <summary>
        /// Get a random customer that already received a package.
        /// </summary>
        /// <returns>A random customer that received a package.</returns>
        /// <exception cref="EmptyListException">No customer received a package.</exception>
        /// <exception cref="UndefinedObjectException">The receiver obtained does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="LinqQueryException">Failed to find a package receiver.</exception>
        private DO.Customer randomPackageReceiver()
        {
            try
            {
                lock (dal)
                {
                    //get the packages already delivered
                    IEnumerable<DO.Package> deliveredPackages = dal.FindPackages(package => package.Delivered != null);

                    if (deliveredPackages.Count() == 0)
                    {
                        throw new EmptyListException("No customer has yet received a package.");
                    }

                    //randomly choose the ID of the receiver of a package
                    Random random = new Random();
                    int receiverID = deliveredPackages.ElementAt(random.Next(deliveredPackages.Count())).ReceiverID;

                    return dal.GetCustomer(receiverID);
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
            catch (Exception)
            {
                throw new LinqQueryException("Could not get a package receiver.");
            }
        }
        #endregion

        #region Auxiliary Package Methods
        /// <summary>
        /// Find the best package based on priority, weight, and distance.
        /// </summary>
        /// <param name="dalPackages">A collection of packages to choose from.</param>
        /// <param name="drone">The drone delivering the package to be chosen.</param>
        /// <returns>The ID of the best package.</returns>
        /// <exception cref="EmptyListException">There are no packages the drone can currently deliver.</exception>
        /// <exception cref="LinqQueryException">Failed to find the best package for a drone to deliver.</exception>
        private int findBestPackage(IEnumerable<DO.Package> dalPackages, DroneToList drone)
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Package> bestPackages = dalPackages;

                    //remove packages too heavy for drone to lift
                    bestPackages = from package in bestPackages
                                   where package.Weight.CompareTo((DO.Enums.WeightCategories)drone.MaxWeight) <= 0
                                   select package;
                    if (bestPackages.Count() == 0)
                    {
                        throw new EmptyListException("There are currently no packages that this drone is capable of delivering.");
                    }

                    //remove packages whose delivery will consume more battery than the drone has
                    bestPackages = from package in bestPackages
                                   let senderLocation = getCustomerLocation(package.SenderID)
                                   let receiverLocation = getCustomerLocation(package.ReceiverID)
                                   let requiredDistance = getDistance(drone.Location, senderLocation) + getDistance(senderLocation, receiverLocation) + getDistance(receiverLocation, getClosestStation(receiverLocation))
                                   let requiredBattery = powerConsumption.ElementAt((int)package.Weight) * requiredDistance
                                   where requiredBattery <= drone.Battery
                                   select package;
                    if (bestPackages.Count() == 0)
                    {
                        throw new EmptyListException("The drone needs more battery in order to deliver any package.");
                    }

                    //order packages by priority, then weight, then distance
                    bestPackages = bestPackages.OrderByDescending(p => p.Priority)
                        .ThenByDescending(p => p.Weight)
                        .ThenBy(p => getDistance(drone.Location, getCustomerLocation(p.SenderID)));

                    return bestPackages.First().ID;
                }
            }
            catch (Exception e) when (isSystemDefinedException(e))
            {
                throw new LinqQueryException("Could not find best package for delivery.");
            }
        }

        /// <summary>
        /// Find the status of a package (in terms of created, assigned, collected, delivered).
        /// </summary>
        /// <param name="dalPackage">The package whose status is being determined.</param>
        /// <returns>The status of the given package.</returns>
        private Enums.PackageStatus getPackageStatus(DO.Package dalPackage)
        {
            lock (dal)
            {
                Enums.PackageStatus status = Enums.PackageStatus.created;
                if (dalPackage.Delivered != null)
                {
                    status = Enums.PackageStatus.delivered;
                }
                else if (dalPackage.Collected != null)
                {
                    status = Enums.PackageStatus.collected;
                }
                else if (dalPackage.Assigned != null)
                {
                    status = Enums.PackageStatus.assigned;
                }
                return status;
            }
        }
        #endregion

        #region Data Layer Cleanup Methods
        /// <summary>
        /// Clean up the information randomly generated in the data layer so it all makes sense.
        /// </summary>
        /// <exception cref="UndefinedObjectException">An entity obtained does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="LinqQueryException">Failed to fix the problematic packages.</exception>
        /// <exception cref="NonUniqueIdException">There are multiple drones with the same ID.</exception>
        private void dataCleanup()
        {
            fixPackageStatusTimes();
            removeProblematicPackages();
        }

        /// <summary>
        /// Modify the information randomly generated in the data layer for the assignment, collection, and delivery times for packages so they make sense.
        /// </summary>
        /// <exception cref="UndefinedObjectException">A package obtained does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="LinqQueryException">Failed to fix the package status times.</exception>
        private void fixPackageStatusTimes()
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Package> dalPackages = dal.GetPackagesList();

                    for (int i = 0; i < dalPackages.Count(); i++)
                    {
                        DO.Package package = dalPackages.ElementAt(i);

                        //if no drone is assigned
                        if (package.DroneID == null)
                        {
                            dal.ModifyPackageStatus(package.ID, null, null, null);
                        }
                        else
                        {
                            //if the package was delivered
                            if (package.Delivered != null)
                            {
                                dal.ModifyPackageStatus(package.ID, DateTime.Now, DateTime.Now, DateTime.Now);
                            }
                            //if the package wasn't delivered but was collected
                            else if (package.Collected != null)
                            {
                                dal.ModifyPackageStatus(package.ID, DateTime.Now, DateTime.Now, null);
                            }
                            //even if the package wasn't collected, it was assigned
                            else
                            {
                                dal.ModifyPackageStatus(package.ID, DateTime.Now, null, null);
                            }
                        }
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
            catch (Exception)
            {
                throw new LinqQueryException("Could not get the packages.");
            }
        }

        /// <summary>
        /// Remove all packages generated in the data layer whose own attributes dictate that it cannot exist.
        /// </summary>
        /// <exception cref="UndefinedObjectException">An entity obtained does not exist.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        /// <exception cref="NonUniqueIdException">There are multiple drones with the same ID.</exception>
        /// <exception cref="LinqQueryException">Failed to remove the problematic packages.</exception>
        private void removeProblematicPackages()
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Package> dalPackages = dal.GetPackagesList();

                    //iterate through the packages in reverse to avoid skipping any
                    for (int i = dalPackages.Count() - 1; i >= 0; i--)
                    {
                        DO.Package package = dalPackages.ElementAt(i);

                        //remove packages whose sender is the receiver
                        if (package.SenderID == package.ReceiverID)
                        {
                            dal.RemovePackage(package.ID);
                        }
                        //if this package was assigned to a drone
                        else if (package.DroneID != null)
                        {
                            //remove packages whose weight is greater than the drone assigned to it can handle
                            if (package.Weight > dal.GetDrone((int)package.DroneID).MaxWeight)
                            {
                                dal.RemovePackage(package.ID);
                            }
                            //remove undelivered packages that were assigned to a drone already assigned to a different undelivered package
                            else if (package.Delivered == null)
                            {
                                for (int j = i - 1; j >= 0; j--)
                                {
                                    if (dalPackages.ElementAt(j).Delivered == null && package.DroneID == dalPackages.ElementAt(j).DroneID)
                                    {
                                        dal.RemovePackage(package.ID);
                                        break;
                                    }
                                }
                            }
                        }
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
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
            catch (Exception)
            {
                throw new LinqQueryException("Could not get the packages.");
            }
        }
        #endregion

        #region Simulator Assisting Methods
        /// <summary>
        /// Update the drone list from the simulator.
        /// </summary>
        /// <param name="droneID">The ID of the drone,</param>
        /// <param name="battery">The new battery level of the drone.</param>
        /// <param name="location">The new location of the drone.</param>
        internal void updateViaSimulator(int droneID, double battery, Location location)
        {
            int droneIndex = drones.FindIndex(drone => drone.ID == droneID);
            drones[droneIndex].Battery = battery;
            drones[droneIndex].Location = location;
        }
        #endregion
    }
}
