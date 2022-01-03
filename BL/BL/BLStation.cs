using BO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    /// <summary>
    /// Station-related functionality of the Business Layer.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        #region Add Methods

        public Station AddStation(int stationID, int name, Location location, int numAvailableChargingSlots)
        {
            if (numAvailableChargingSlots < 0)
            {
                throw new IllegalArgumentException("There cannot be a negative number of available charging slots.");
            }
            if (location.Latitude < -1 || location.Latitude > 1 || location.Longitude < 0 || location.Longitude > 2) //limited coordinate field
            {
                throw new IllegalArgumentException("The given latitude and/or longitude is out of our coordinate field range.");
            }

            try
            {
                dalObject.AddStation(stationID, name, numAvailableChargingSlots, location.Latitude, location.Longitude);
            }
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
            Station station = new(stationID, name, location, numAvailableChargingSlots, new List<DroneCharging>());
            return station;
        }

        #endregion

        #region Update Methods

        public void UpdateStation(int stationID, int name = -1, int totalChargingSlots = -1)
        {
            try
            {
                if (name != -1)
                {
                    dalObject.UpdateStationName(stationID, name);
                }
                if (totalChargingSlots != -1)
                {
                    DO.Station dalStation = dalObject.GetStation(stationID);

                    //find the amount of drones in this station
                    IEnumerable<DroneToList> dronesAtStation = drones.FindAll(d => d.Location.Latitude == dalStation.Latitude && d.Location.Longitude == dalStation.Longitude && d.Status == Enums.DroneStatus.maintenance);
                    
                    int availableChargeSlots = totalChargingSlots - dronesAtStation.Count();
                    if (availableChargeSlots < 0)
                    {
                        throw new IllegalArgumentException("There cannot be less charging slots than drones charging.");
                    }

                    dalObject.UpdateStationChargeSlots(stationID, availableChargeSlots);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        #endregion

        #region Remove Methods

        public void RemoveStation(int stationID)
        {
            try
            {
                dalObject.RemoveStation(stationID);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        #endregion

        #region Getter Methods

        public Station GetStation(int stationID)
        {
            try
            {
                DO.Station dalStation = dalObject.GetStation(stationID);

                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);

                //find drones at this station
                IEnumerable<DroneToList> dronesAtStation = drones.FindAll(d => d.Location.Latitude == stationLocation.Latitude && d.Location.Longitude == stationLocation.Longitude && d.Status == Enums.DroneStatus.maintenance);

                //initialize DroneCharging entities
                IEnumerable<DroneCharging> dronesCharging = from DroneToList drone in dronesAtStation
                                                            select new DroneCharging(drone.ID, drone.Battery);
                
                return new Station(dalStation.ID, dalStation.Name, stationLocation, dalStation.AvailableChargeSlots, dronesCharging);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public IEnumerable<StationToList> GetStationsList()
        {
            IEnumerable<DO.Station> dalStations = dalObject.GetStationsList();
            IEnumerable<StationToList> stationToLists = from DO.Station dalStation in dalStations
                                                        let stationLocation = new Location(dalStation.Latitude, dalStation.Longitude)
                                                        let dronesAtStation = drones.FindAll(d => d.Location.Latitude == stationLocation.Latitude && d.Location.Longitude == stationLocation.Longitude && d.Status == Enums.DroneStatus.maintenance)
                                                        select new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count);
            return stationToLists;
        }

        #endregion

        #region Find Methods

        public IEnumerable<StationToList> FindStations(Predicate<DO.Station> predicate)
        {
            IEnumerable<DO.Station> dalStations = dalObject.FindStations(predicate);
            IEnumerable<StationToList> stationToLists = from DO.Station dalStation in dalStations
                                                        let stationLocation = new Location(dalStation.Latitude, dalStation.Longitude)
                                                        let dronesAtStation = drones.FindAll(d => d.Location.Latitude == stationLocation.Latitude && d.Location.Longitude == stationLocation.Longitude && d.Status == Enums.DroneStatus.maintenance)
                                                        select new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count);
            return stationToLists;
        }

        #endregion
    }
}