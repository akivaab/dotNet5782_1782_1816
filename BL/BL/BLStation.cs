using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BO;

namespace BL
{
    /// <summary>
    /// Station-related functionality of the Business Layer.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
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
                lock (dal)
                {
                    dal.AddStation(stationID, name, numAvailableChargingSlots, location.Latitude, location.Longitude);
                }
                Station station = new(stationID, name, location, numAvailableChargingSlots, new List<DroneCharging>());
                return station;
            }
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Update Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(int stationID, int name = -1, int totalChargingSlots = -1)
        {
            try
            {
                lock (dal)
                {
                    if (name != -1)
                    {
                        dal.UpdateStationName(stationID, name);
                    }
                    if (totalChargingSlots != -1)
                    {
                        DO.Station dalStation = dal.GetStation(stationID);

                        //find the amount of drones in this station
                        IEnumerable<DO.DroneCharge> dronesAtStation = dal.FindDroneCharges(dc => dc.StationID == stationID);

                        int availableChargeSlots = totalChargingSlots - dronesAtStation.Count();
                        if (availableChargeSlots < 0)
                        {
                            throw new IllegalArgumentException("There cannot be less charging slots than drones charging.");
                        }

                        dal.UpdateStationChargeSlots(stationID, availableChargeSlots);
                    }
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveStation(int stationID)
        {
            try
            {
                if (FindStations(s => s.ID == stationID).Single().NumOccupiedChargeSlots > 0)
                {
                    throw new UnableToRemoveException("The station has drones charging in it.");
                }

                lock (dal)
                {
                    dal.RemoveStation(stationID);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
            catch (Exception e) when (isSystemDefinedException(e))
            {
                throw new LinqQueryException("There is not a single matching station.");
            }
        }
        #endregion

        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStation(int stationID)
        {
            try
            {
                lock (dal)
                {
                    DO.Station dalStation = dal.GetStation(stationID);

                    Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);

                    //find drones at this station
                    IEnumerable<DO.DroneCharge> dronesAtStation = dal.FindDroneCharges(dc => dc.StationID == stationID);

                    //initialize DroneCharging entities
                    IEnumerable<DroneCharging> dronesCharging = from DO.DroneCharge droneCharge in dronesAtStation
                                                                let drone = drones.Find(d => d.ID == droneCharge.DroneID)
                                                                select new DroneCharging(drone.ID, drone.Battery);

                    return new Station(dalStation.ID, dalStation.Name, stationLocation, dalStation.AvailableChargeSlots, dronesCharging);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationToList> GetStationsList()
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Station> dalStations = dal.GetStationsList();
                    IEnumerable<StationToList> stationToLists = from DO.Station dalStation in dalStations
                                                                let dronesAtStation = dal.FindDroneCharges(dc => dc.StationID == dalStation.ID)
                                                                select new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count());
                    return stationToLists;
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<StationToList> FindStations(Predicate<DO.Station> predicate)
        {
            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Station> dalStations = dal.FindStations(predicate);
                    IEnumerable<StationToList> stationToLists = from DO.Station dalStation in dalStations
                                                                let dronesAtStation = dal.FindDroneCharges(dc => dc.StationID == dalStation.ID)
                                                                select new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count());
                    return stationToLists;
                }
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion
    }
}