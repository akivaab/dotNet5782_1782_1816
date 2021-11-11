using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL
    {
        private double getDistance(Location location1, Location location2)
        {
            //TODO: euclidean distance
        }
        private Location closestStation(IDAL.IDal dalObject, int customerID)
        {
            Location closestCustomerLocation = getCustomerLocation(dalObject, customerID);
            List<IDAL.DO.Station> stations = (List<IDAL.DO.Station>)dalObject.DisplayStationsList();
            double min = double.MaxValue;
            Location closestStationLocation = new();
            foreach (IDAL.DO.Station station in stations)
            {
                Location stationLocation = new();
                stationLocation.Latitude = station.Latitude;
                stationLocation.Longitude = station.Longitude;
                double distance = getDistance(closestCustomerLocation, stationLocation);
                if (distance < min)
                {
                    closestStationLocation = stationLocation;
                    min = distance;
                }
            }
            return closestStationLocation;
            
        }
        private Location getCustomerLocation(IDAL.IDal dalObject, int customerID)
        {
            List<IDAL.DO.Customer> customerList = (List<IDAL.DO.Customer>)dalObject.DisplayCustomersList();
            IDAL.DO.Customer customer = customerList.Find(c => c.ID == customerID);
            Location customerLocation = new();
            customerLocation.Latitude = customer.Latitude;
            customerLocation.Longitude = customer.Longitude;
            return customerLocation;
        }
    }
}
