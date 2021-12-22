using System;
using System.Collections.Generic;
using System.Linq;
using DO;

namespace DalObject
{
    partial class DalObject : DalApi.IDal
    {
        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            if (latitude < -1 || latitude > 1 || longitude < 0 || longitude > 2) //limited coordinate field
            {
                throw new IllegalArgumentException("The given latitude and/or longitude is out of our coordinate field range.");
            }
            if (DataSource.Stations.FindIndex(station => station.ID == id) != -1)
            {
                throw new NonUniqueIdException("The given station ID is not unique.");
            }
            
            Station station = new();
            station.ID = id;
            station.Name = name;
            station.AvailableChargeSlots = numChargeSlots;
            station.Latitude = latitude;
            station.Longitude = longitude;
            DataSource.Stations.Add(station);
        }

        public void UpdateStationName(int stationID, int name)
        {
            int stationIndex = DataSource.Stations.FindIndex(s => s.ID == stationID);
            
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }

            Station station = DataSource.Stations[stationIndex];
            station.Name = name;
            DataSource.Stations[stationIndex] = station;
        }

        public void UpdateStationChargeSlots(int stationID, int availableChargingSlots)
        {
            int stationIndex = DataSource.Stations.FindIndex(s => s.ID == stationID);
            
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }

            Station station = DataSource.Stations[stationIndex];
            station.AvailableChargeSlots = availableChargingSlots;
            DataSource.Stations[stationIndex] = station;
        }

        public Station DisplayStation(int stationID)
        {
            int stationIndex = DataSource.Stations.FindIndex(station => station.ID == stationID);
            
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }
            
            return DataSource.Stations[stationIndex];
        }

        public IEnumerable<Station> DisplayStationsList()
        {
            return from station in DataSource.Stations
                   select station;
        }

        public IEnumerable<Station> FindStations(Predicate<Station> predicate)
        {
            return from station in DataSource.Stations
                   where predicate(station)
                   select station;
        }
    }
}
