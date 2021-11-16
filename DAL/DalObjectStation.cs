using System.Collections.Generic;
using IDAL.DO;

namespace DalObject
{
    public partial class DalObject : IDAL.IDal
    {
        /// <summary>
        /// Constructor adds initial values to the entity arrays
        /// </summary>
        public DalObject()
        {
            DataSource.Initialize();
        }

        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90 || longitude < 0 || longitude > 180)
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
            /*
            List<Station> stations = new();
            for (int i = 0; i < DataSource.Stations.Count; i++)
            {
                stations.Add(DataSource.Stations[i]);
            }
            return stations;
            */
            return DataSource.Stations;
        }

        public IEnumerable<Station> DisplayUnoccupiedStationsList()
        {
            List<Station> stations = new();
            for (int i = 0; i < DataSource.Stations.Count; i++)
            {
                if (DataSource.Stations[i].AvailableChargeSlots > 0)
                {
                    stations.Add(DataSource.Stations[i]);
                }
            }
            return stations;
        }
    }
}
