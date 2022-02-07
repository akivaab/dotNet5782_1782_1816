using System;
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
        private double getDistance(Location location1, Location location2)
        {
            double lat1 = location1.Latitude;
            double long1 = location1.Longitude;
            double lat2 = location2.Latitude;
            double long2 = location2.Longitude;

            double equatorialEarthRadius = 6378.1370;
            double degreeToRadian = Math.PI / 180;
            double dlong = (long2 - long1) * degreeToRadian;
            double dlat = (lat2 - lat1) * degreeToRadian;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * degreeToRadian) * Math.Cos(lat2 * degreeToRadian) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = equatorialEarthRadius * c;

            return d;
        }

        /// <summary>
        /// Return a random battery level between the amount a drone needs to deliver a package and return to a station, and 100.
        /// </summary>
        /// <param name="droneLocation">The current location of the drone.</param>
        /// <param name="package">The package being delivered.</param>
        /// <param name="powerConsumed">The amount of battery required for the drone to carry the package one kilometer.</param>
        /// <returns>A random double representing the battery level.</returns>
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

        #region Auxiliary Drone Methods
        /// <summary>
        /// Charge the drone, as reflected in the battery.
        /// </summary>
        /// <param name="droneID">The ID of the drone charging.</param>
        /// <param name="chargingTimeInSeconds">The amount of time in seconds the drone has charged.</param>
        internal void chargeDrone(int droneID, double chargingTimeInSeconds)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            drones[droneIndex].Battery = Math.Min(drones[droneIndex].Battery + (chargeRatePerSecond * chargingTimeInSeconds), 100);
        }

        /// <summary>
        /// Release the drone from the charging station.
        /// </summary>
        /// <param name="droneID">The ID of the drone being releaseed.</param>
        internal void releaseDrone(int droneID)
        {
            try
            {
                int droneIndex = drones.FindIndex(d => d.ID == droneID);
                if (droneIndex == -1)
                {
                    throw new UndefinedObjectException("There is no drone with the given ID.");
                }

                lock (dal)
                {
                    DO.DroneCharge dalDroneCharge = dal.FindDroneCharges(dc => dc.DroneID == droneID).Single();
                    dal.ReleaseDroneFromCharging(droneID, dalDroneCharge.StationID);
                }
                drones[droneIndex].Status = Enums.DroneStatus.available;
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
        private Location getClosestStation(Location location, IEnumerable<DO.Station> stations)
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

        /// <summary>
        /// Get a collection of available stations that a drone carrying no weight is capable of reaching before running out of battery. 
        /// </summary>
        /// <param name="drone">A drone not carrying any package.</param>
        /// <returns>A collection of stations that the drone is capable of reaching.</returns>
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
        }
        #endregion

        #region Auxiliary Customer Methods
        /// <summary>
        /// Find the location of a customer.
        /// </summary>
        /// <param name="customerID">ID of the customer.</param>
        /// <returns>The location of the customer.</returns>
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
        }
        #endregion

        #region Auxiliary Package Methods
        /// <summary>
        /// Find the best package based on priority, weight, and distance.
        /// </summary>
        /// <param name="dalPackages">A collection of packages to choose from.</param>
        /// <param name="drone">The drone delivering the package to be chosen.</param>
        /// <returns>The ID of the best package.</returns>
        private int findBestPackage(IEnumerable<DO.Package> dalPackages, DroneToList drone)
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
        private void dataCleanup()
        {
            fixPackageStatusTimes();
            removeProblematicPackages();
        }

        /// <summary>
        /// Modify the information randomly generated in the data layer for the assignment, collection, and delivery times for packages so they make sense.
        /// </summary>
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
        }

        /// <summary>
        /// Remove all packages generated in the data layer whose own attributes dictate that it cannot exist.
        /// </summary>
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
        }
        #endregion
    }
}
