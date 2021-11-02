using System.Collections.Generic;
using IDAL.DO;

namespace DalObject
{
    public partial class DalObject : IDal.IDal
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
            if (DataSource.Stations.Count >= 5)
            {
                throw new ExceededLimitException();
            }
            if (DataSource.Stations.FindIndex(station => station.ID == id) != -1)
            {
                throw new NonUniqueIdException();
            }
            Station station = new();
            station.ID = id;
            station.Name = name;
            station.NumChargeSlots = numChargeSlots;
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
            List<Station> stations = new();
            for (int i = 0; i < DataSource.Stations.Count; i++)
            {
                stations.Add(DataSource.Stations[i]);
            }
            return stations;
        }

        public IEnumerable<Station> DisplayUnoccupiedStationsList()
        {
            List<Station> stations = new();
            for (int i = 0; i < DataSource.Stations.Count; i++)
            {
                if (DataSource.Stations[i].NumChargeSlots > 0)
                {
                    stations.Add(DataSource.Stations[i]);
                }
            }
            return stations;
        }
    }
}
