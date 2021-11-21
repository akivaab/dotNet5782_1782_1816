using System;
using System.Collections.Generic;
using System.Linq;
using IBL.BO;

namespace IBL
{
    public partial class BL
    {
        /// <summary>
        /// Calculates the distance in kilometers between two locations.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns>The distance in kilometers between locations</returns>
        private double getDistance(Location location1, Location location2)
        {
            double lat1 = location1.Latitude;
            double long1 = location1.Longitude;
            double lat2 = location2.Latitude;
            double long2 = location2.Longitude;

            double equatorialEarthRadius = 6378.1370;
            double degreeToRadian = (Math.PI / 180);
            double dlong = (long2 - long1) * degreeToRadian;
            double dlat = (lat2 - lat1) * degreeToRadian;
            double a = Math.Pow(Math.Sin(dlat / 2D), 2D) + Math.Cos(lat1 * degreeToRadian) * Math.Cos(lat2 * degreeToRadian) * Math.Pow(Math.Sin(dlong / 2D), 2D);
            double c = 2D * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1D - a));
            double d = equatorialEarthRadius * c;

            return d;
        }

        /// <summary>
        /// Find station closest to some location.
        /// </summary>
        /// <param name="location"></param>
        /// <returns>Location of the closest station</returns>
        private Location getClosestStation(Location location)
        {
            List<IDAL.DO.Station> dalStations = new(DalObject.DisplayStationsList());
            return getClosestStation(location, dalStations);
        }

        /// <summary>
        /// Of the stations provided, find the station closest to some location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="stations"></param>
        /// <returns>Location of the closest station</returns>
        private Location getClosestStation(Location location, List<IDAL.DO.Station> stations)
        {
            double min = double.MaxValue;
            Location closestStationLocation = new();

            foreach (IDAL.DO.Station station in stations)
            {
                Location stationLocation = new(station.Latitude, station.Longitude);
                double distance = getDistance(location, stationLocation);
                if (distance < min)
                {
                    closestStationLocation = stationLocation;
                    min = distance;
                }
            }

            return closestStationLocation;
        }

        /// <summary>
        /// Gets a list of stations that a drone carrying no weight is capable of reaching before running out of battery. 
        /// </summary>
        /// <param name="drone"></param>
        /// <returns>List of reachable stations</returns>
        private List<IDAL.DO.Station> getReachableStations(DroneToList drone)
        {
            List<IDAL.DO.Station> availableStations = new(DalObject.DisplayFreeStationsList());
            List<IDAL.DO.Station> reachableStations = new();

            foreach (IDAL.DO.Station station in availableStations)
            {
                Location stationLocation = new(station.Latitude, station.Longitude);
                double requiredBattery = PowerConsumption[(int)Enums.WeightCategories.free] * getDistance(drone.Location, stationLocation);
                if (requiredBattery <= drone.Battery)
                {
                    reachableStations.Add(station);
                }
            }

            return reachableStations;
        }

        /// <summary>
        /// Find the location of a customer.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns>Location of a customer</returns>
        private Location getCustomerLocation(int customerID)
        {
            try
            {
                IDAL.DO.Customer dalCustomer = DalObject.DisplayCustomer(customerID);
                Location customerLocation = new(dalCustomer.Latitude, dalCustomer.Longitude);
                return customerLocation;
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }

        /// <summary>
        /// Return a random battery level between the amount necessary to deliver and return to a station and 100.
        /// </summary>
        /// <param name="droneLocation"></param>
        /// <param name="package"></param>
        /// <param name="powerConsumed"></param>
        /// <returns>A random double representing battery level</returns>
        private double randomBatteryPower(Location droneLocation, IDAL.DO.Package package, double powerConsumed)
        {
            try
            {
                IDAL.DO.Customer sender = DalObject.DisplayCustomer(package.SenderID);
                IDAL.DO.Customer receiver = DalObject.DisplayCustomer(package.ReceiverID);
                Location senderLocation = new Location(sender.Latitude, sender.Longitude);
                Location receiverLocation = new Location(receiver.Latitude, receiver.Longitude);

                //distance needed to deliver is from the drone's current location to the sender, to the receiver, to the nearest station
                double distanceToDeliver = getDistance(droneLocation, senderLocation);
                distanceToDeliver += getDistance(senderLocation, receiverLocation);
                distanceToDeliver += getDistance(receiverLocation, getClosestStation(receiverLocation));

                Random random = new Random();
                return random.Next((int)Math.Ceiling(distanceToDeliver * powerConsumed), 101);
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }

        /// <summary>
        /// Gets a random customer that already received a package.
        /// </summary>
        /// <param name="dalPackages"></param>
        /// <returns>Random customer that received a package</returns>
        private IDAL.DO.Customer randomPackageReceiver(List<IDAL.DO.Package> dalPackages)
        {
            try
            {
                //get the packages already delivered
                List<IDAL.DO.Package> deliveredPackages = dalPackages.FindAll(package => package.Delivered != DateTime.MinValue);

                //randomly choose the ID of the receiver of a package
                Random random = new Random();
                int receiverID = deliveredPackages[random.Next(deliveredPackages.Count)].ReceiverID;

                return DalObject.DisplayCustomer(receiverID);
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }

        /// <summary>
        /// Finds the best package based on priority, weight, and distance.
        /// </summary>
        /// <param name="dalPackages"></param>
        /// <param name="drone"></param>
        /// <returns>ID of the best package</returns>
        private int findBestPackage(List<IDAL.DO.Package> dalPackages, DroneToList drone)
        {
            List<IDAL.DO.Package> bestPackages = new(dalPackages);

            //remove packages too heavy for drone to lift
            bestPackages.RemoveAll(p => p.Weight.CompareTo((IDAL.DO.Enums.WeightCategories)drone.MaxWeight) > 0);
            
            //remove packages whose delivery will consume more battery than the drone has
            bestPackages.RemoveAll(p =>
            {
                Location senderLocation = getCustomerLocation(p.SenderID);
                Location receiverLocation = getCustomerLocation(p.ReceiverID);

                //distance needed to deliver is from the drone's current location to the sender, to the receiver, to the nearest station
                double requiredDistance = getDistance(drone.Location, senderLocation);
                requiredDistance += getDistance(senderLocation, receiverLocation);
                requiredDistance += getDistance(receiverLocation, getClosestStation(receiverLocation));
                
                double requiredBattery = PowerConsumption[(int)p.Weight] * requiredDistance;
                return requiredBattery > drone.Battery;
            });

            //order packages by priority, then weight, then distance
            bestPackages = (List<IDAL.DO.Package>)bestPackages.OrderByDescending(p => p.Priority)
                .ThenByDescending(p => p.Weight)
                .ThenBy(p => getDistance(drone.Location, getCustomerLocation(p.SenderID)))
                .ToList();
            
            return bestPackages[0].ID;
        }

        /// <summary>
        /// Finds the status of a package (in terms of created, assigned, collected, delivered).
        /// </summary>
        /// <param name="dalPackage"></param>
        /// <returns>The status of the package</returns>
        private Enums.PackageStatus getPackageStatus(IDAL.DO.Package dalPackage)
        {
            Enums.PackageStatus status = Enums.PackageStatus.created;
            if (dalPackage.Delivered != DateTime.MinValue)
            {
                status = Enums.PackageStatus.delivered;
            }
            else if (dalPackage.Collected != DateTime.MinValue)
            {
                status = Enums.PackageStatus.collected;
            }
            else if (dalPackage.Assigned != DateTime.MinValue)
            {
                status = Enums.PackageStatus.assigned;
            }
            return status;
        }

        /// <summary>
        /// Clean up the information randomly generated in the data layer so it makes sense.
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
                List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());

                for (int i = 0; i < dalPackages.Count; i++)
                {
                    //if no drone is assigned
                    if (dalPackages[i].DroneID == 0)
                    {
                        DalObject.ModifyPackageStatus(dalPackages[i].ID, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
                    }
                    else
                    {
                        //if the package was delivered
                        if (dalPackages[i].Delivered != DateTime.MinValue)
                        {
                            DalObject.ModifyPackageStatus(dalPackages[i].ID, DateTime.Now, DateTime.Now, DateTime.Now);
                        }
                        //if the package wasn't delivered but was collected
                        else if (dalPackages[i].Collected != DateTime.MinValue)
                        {
                            DalObject.ModifyPackageStatus(dalPackages[i].ID, DateTime.Now, DateTime.Now, DateTime.MinValue);
                        }
                        //if the package wasn't collected but was assigned
                        else if (dalPackages[i].Assigned != DateTime.MinValue)
                        {
                            DalObject.ModifyPackageStatus(dalPackages[i].ID, DateTime.Now, DateTime.MinValue, DateTime.MinValue);
                        }
                    }
                }
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }

        /// <summary>
        /// Remove all packages generated in the data layer whose own attributes dictate that it cannot exist.
        /// </summary>
        private void removeProblematicPackages()
        {
            try
            {
                List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());

                //iterate through the packages in reverse to avoid skipping any
                for (int i = dalPackages.Count - 1; i >= 0; i--)
                {
                    //remove packages whose sender is the receiver
                    if (dalPackages[i].SenderID == dalPackages[i].ReceiverID)
                    {
                        DalObject.RemovePackage(dalPackages[i].ID);
                    }
                    //if this package was assigned to a drone
                    else if (dalPackages[i].DroneID != 0)
                    {
                        //remove packages whose weight is greater than the drone assigned to it can handle
                        if (dalPackages[i].Weight > DalObject.DisplayDrone(dalPackages[i].DroneID).MaxWeight)
                        {
                            DalObject.RemovePackage(dalPackages[i].ID);
                        }
                        //remove undelivered packages that were assigned to a drone already assigned to a different undelivered package
                        else if (dalPackages[i].Delivered == DateTime.MinValue)
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if (dalPackages[j].Delivered == DateTime.MinValue && dalPackages[i].DroneID == dalPackages[j].DroneID)
                                {
                                    DalObject.RemovePackage(dalPackages[i].ID);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
    }
}
