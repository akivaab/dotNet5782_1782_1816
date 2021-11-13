using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL
    {
        /// <summary>
        /// Calculates the distance between two locations.
        /// </summary>
        /// <param name="location1"></param>
        /// <param name="location2"></param>
        /// <returns>The distance between locations</returns>
        private double getDistance(Location location1, Location location2)
        {
            double latDist = Math.Pow(location1.Latitude - location2.Latitude, 2);
            double longDist = Math.Pow(location1.Longitude - location2.Longitude, 2);
            return Math.Sqrt(latDist + longDist);
        }

        /// <summary>
        /// Find station closest to a customer.
        /// </summary>
        /// <param name="dalObject"></param>
        /// <param name="customerID"></param>
        /// <returns>Location of the closest station</returns>
        private Location closestStationToCustomer(IDAL.IDal dalObject, int customerID)
        {
            Location customerLocation = getCustomerLocation(dalObject, customerID);
            List<IDAL.DO.Station> dalStations = (List<IDAL.DO.Station>)dalObject.DisplayStationsList();
            double min = double.MaxValue;
            Location closestStationLocation = new();
            foreach (IDAL.DO.Station station in dalStations)
            {
                Location stationLocation = new();
                stationLocation.Latitude = station.Latitude;
                stationLocation.Longitude = station.Longitude;
                double distance = getDistance(customerLocation, stationLocation);
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
            Location customerLocation = new();
            customerLocation.Latitude = customer.Latitude;
            customerLocation.Longitude = customer.Longitude;
            return customerLocation;
        }

        private double randomBatteryPower(IDAL.IDal dalObject, double powerConsumed)
        {
            //TODO
            throw new NotImplementedException();
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
