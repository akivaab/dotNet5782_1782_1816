using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DO;

namespace DalObject
{
    /// <summary>
    /// Station-related functionality of the Data Layer.
    /// </summary>
    partial class DalObject : DalApi.IDal
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            if (DataSource.stations.Exists(station => station.ID == id && station.Active))
            {
                throw new NonUniqueIdException("The given station ID is not unique.");
            }
            
            Station station = new();
            station.ID = id;
            station.Name = name;
            station.AvailableChargeSlots = numChargeSlots;
            station.Latitude = latitude;
            station.Longitude = longitude;
            station.Active = true;
            DataSource.stations.Add(station);
        }
        #endregion

        #region Update Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStationName(int stationID, int name)
        {
            int stationIndex = DataSource.stations.FindIndex(station => station.ID == stationID && station.Active);
            
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }

            Station station = DataSource.stations[stationIndex];
            station.Name = name;
            DataSource.stations[stationIndex] = station;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStationChargeSlots(int stationID, int availableChargingSlots)
        {
            int stationIndex = DataSource.stations.FindIndex(station => station.ID == stationID && station.Active);
            
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }

            Station station = DataSource.stations[stationIndex];
            station.AvailableChargeSlots = availableChargingSlots;
            DataSource.stations[stationIndex] = station;
        }
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveStation(int stationID)
        {
            int stationIndex = DataSource.stations.FindIndex(station => station.ID == stationID && station.Active);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID");
            }

            Station station = DataSource.stations[stationIndex];
            station.Active = false;
            DataSource.stations[stationIndex] = station;
        }
        #endregion

        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStation(int stationID)
        {
            int stationIndex = DataSource.stations.FindIndex(station => station.ID == stationID && station.Active);
            
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }
            
            return DataSource.stations[stationIndex];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> GetStationsList()
        {
            return from station in DataSource.stations
                   where station.Active
                   select station;
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> FindStations(Predicate<Station> predicate)
        {
            return from station in DataSource.stations
                   where predicate(station) && station.Active
                   select station;
        }
        #endregion
    }
}
