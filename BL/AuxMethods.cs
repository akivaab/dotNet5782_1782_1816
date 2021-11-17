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
            IDAL.DO.Customer dalCustomer = DalObject.DisplayCustomer(customerID);
            Location customerLocation = new(dalCustomer.Latitude, dalCustomer.Longitude);
            return customerLocation;
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
            IDAL.DO.Customer sender = DalObject.DisplayCustomer(package.SenderID);
            IDAL.DO.Customer receiver = DalObject.DisplayCustomer(package.ReceiverID);
            Location senderLocation = new Location(sender.Latitude, sender.Longitude);
            Location receiverLocation = new Location(receiver.Latitude, receiver.Longitude);

            double distanceToDeliver = getDistance(droneLocation, senderLocation);
            distanceToDeliver += getDistance(senderLocation, receiverLocation);
            distanceToDeliver += getDistance(receiverLocation, getClosestStation(receiverLocation));
            
            Random random = new Random();
            return random.Next((int)Math.Ceiling(distanceToDeliver * powerConsumed), 101);
        }

        /// <summary>
        /// Gets a random customer that already received a package.
        /// </summary>
        /// <param name="dalPackages"></param>
        /// <returns>Random customer that received a package</returns>
        private IDAL.DO.Customer randomPackageReceiver(List<IDAL.DO.Package> dalPackages)
        {
            List<IDAL.DO.Package> deliveredPackages = dalPackages.FindAll(package => package.Delivered != DateTime.MinValue);
            Random random = new Random();
            int receiverID = deliveredPackages[random.Next(deliveredPackages.Count)].ReceiverID;
            return DalObject.DisplayCustomer(receiverID);
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

            //remove packages to heavy for drone to lift
            bestPackages.RemoveAll(p => p.Weight.CompareTo(drone.MaxWeight) > 0);
            
            //remove packages whose delivery will consume more battery than the drone has
            bestPackages.RemoveAll(p =>
            {
                Location senderLocation = getCustomerLocation(p.SenderID);
                Location receiverLocation = getCustomerLocation(p.ReceiverID);
               
                double requiredDistance = getDistance(drone.Location, senderLocation);
                requiredDistance += getDistance(senderLocation, receiverLocation);
                requiredDistance += getDistance(receiverLocation, getClosestStation(receiverLocation));
                
                double requiredBattery = PowerConsumption[(int)p.Weight] * requiredDistance;
                return requiredBattery > drone.Battery;
            });

            bestPackages = (List<IDAL.DO.Package>)bestPackages.OrderByDescending(p => p.Priority)
                .ThenByDescending(p => p.Weight)
                .ThenBy(p => getDistance(drone.Location, getCustomerLocation(p.SenderID)));
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
    }
}
