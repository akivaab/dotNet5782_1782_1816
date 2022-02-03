using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DO;

namespace DalXml
{
    /// <summary>
    /// Station-related functionality of the Data Layer.
    /// </summary>
    partial class DalXml : DalApi.IDal
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);

            if (stations.Exists(station => station.ID == id && station.Active))
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
            stations.Add(station);

            saveListToXMLSerializer<Station>(stations, stationXmlPath);
        }
        #endregion

        #region Update Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStationName(int stationID, int name)
        {
            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);

            int stationIndex = stations.FindIndex(station => station.ID == stationID && station.Active);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }

            Station station = stations[stationIndex];
            station.Name = name;
            stations[stationIndex] = station;

            saveListToXMLSerializer<Station>(stations, stationXmlPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStationChargeSlots(int stationID, int availableChargingSlots)
        {
            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);

            int stationIndex = stations.FindIndex(station => station.ID == stationID && station.Active);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }

            Station station = stations[stationIndex];
            station.AvailableChargeSlots = availableChargingSlots;
            stations[stationIndex] = station;

            saveListToXMLSerializer<Station>(stations, stationXmlPath);
        }
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveStation(int stationID)
        {
            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);

            int stationIndex = stations.FindIndex(station => station.ID == stationID && station.Active);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID");
            }

            Station station = stations[stationIndex];
            station.Active = false;
            stations[stationIndex] = station;

            saveListToXMLSerializer<Station>(stations, stationXmlPath);
        }
        #endregion

        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStation(int stationID)
        {
            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);

            int stationIndex = stations.FindIndex(station => station.ID == stationID && station.Active);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no station with the given ID.");
            }

            return stations[stationIndex].Clone();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> GetStationsList()
        {
            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);

            return from station in stations
                   where station.Active
                   select station.Clone();
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> FindStations(Predicate<Station> predicate)
        {
            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);

            return from station in stations
                   where predicate(station) && station.Active
                   select station.Clone();
        }
        #endregion
    }
}
