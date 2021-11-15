using System;
using System.Collections.Generic;
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
        /// <param name="dalObject"></param>
        /// <param name="location"></param>
        /// <returns>Location of the closest station</returns>
        private Location getClosestStation(IDAL.IDal dalObject, Location location)
        {
            List<IDAL.DO.Station> dalStations = (List<IDAL.DO.Station>)dalObject.DisplayStationsList();
            double min = double.MaxValue;
            Location closestStationLocation = new();
            foreach (IDAL.DO.Station station in dalStations)
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
        /// Find the location of a customer.
        /// </summary>
        /// <param name="dalObject"></param>
        /// <param name="customerID"></param>
        /// <returns>Location of a customer</returns>
        private Location getCustomerLocation(IDAL.IDal dalObject, int customerID)
        {
            List<IDAL.DO.Customer> dalCustomers = (List<IDAL.DO.Customer>)dalObject.DisplayCustomersList();
            IDAL.DO.Customer customer = dalCustomers.Find(c => c.ID == customerID);
            Location customerLocation = new(customer.Latitude, customer.Longitude);
            return customerLocation;
        }

        /// <summary>
        /// Return a random battery level between the amount necessary to deliver and return to a station and 100.
        /// </summary>
        /// <param name="dalObject"></param>
        /// <param name="droneToList"></param>
        /// <param name="package"></param>
        /// <param name="powerConsumed"></param>
        /// <returns>A random double representing battery level</returns>
        private double randomBatteryPower(IDAL.IDal dalObject, DroneToList droneToList, IDAL.DO.Package package, double powerConsumed)
        {
            List<IDAL.DO.Customer> dalCustomers = new List<IDAL.DO.Customer>(dalObject.DisplayCustomersList());
            IDAL.DO.Customer sender = dalCustomers.Find(s => s.ID == package.SenderID);
            IDAL.DO.Customer receiver = dalCustomers.Find(r => r.ID == package.ReceiverID);
            Location droneLocation = droneToList.Location;
            Location senderLocation = new Location(sender.Latitude, sender.Longitude);
            Location receiverLocation = new Location(receiver.Latitude, receiver.Longitude);
            double distanceToDeliver = getDistance(droneLocation, senderLocation);
            distanceToDeliver += getDistance(senderLocation, receiverLocation);
            distanceToDeliver += getDistance(receiverLocation, getClosestStation(dalObject, receiverLocation));
            Random random = new Random();
            return random.Next((int)Math.Ceiling(distanceToDeliver * powerConsumed), 101);
        }

        /// <summary>
        /// Gets a random customer that already received a package.
        /// </summary>
        /// <param name="dalObject"></param>
        /// <param name="dalPackages"></param>
        /// <returns>Random customer that received a package</returns>
        private IDAL.DO.Customer packageReceiver(IDAL.IDal dalObject, List<IDAL.DO.Package> dalPackages)
        {
            List<IDAL.DO.Package> deliveredPackages = dalPackages.FindAll(package => package.Delivered != DateTime.MinValue);
            Random random = new Random();
            int receiverID = deliveredPackages[random.Next(deliveredPackages.Count)].ReceiverID;
            return ((List<IDAL.DO.Customer>)dalObject.DisplayCustomersList()).Find(customer => customer.ID == receiverID);
        }
    }
}
