using System;
using System.Collections.Generic;
using DO;

namespace DalObject
{
    sealed partial class DalObject : DalApi.IDal
    {
        private static readonly Lazy<DalObject> lazyDalObject = new Lazy<DalObject>(() => new DalObject());

        public static DalObject Instance { get { return lazyDalObject.Value; } }
        /// <summary>
        /// Constructor adds initial values to the entity arrays
        /// </summary>
        private DalObject()
        {
            DataSource.Initialize();
        }

        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            if (latitude < -1 || latitude > 1 || longitude < 0 || longitude > 2) //limited coordinate field
            {
                throw new IllegalArgumentException();
            }
            if (DataSource.Stations.FindIndex(station => station.ID == id) != -1)
            {
                throw new NonUniqueIdException();
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
                throw new UndefinedObjectException();
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
                throw new UndefinedObjectException();
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
                throw new UndefinedObjectException();
            }
            return DataSource.Stations[stationIndex];
        }

        public IEnumerable<Station> DisplayStationsList()
        {
            List<Station> stations = new();
            foreach (Station station in DataSource.Stations)
            {
                stations.Add(station);
            }
            return stations;
        }

        public IEnumerable<Station> FindStations(Predicate<Station> predicate)
        {
            List<Station> stations = new();
            foreach (Station station in DataSource.Stations)
            {
                if (predicate(station) == true)
                {
                    stations.Add(station);
                }
            }
            return stations;
        }
    }
}
