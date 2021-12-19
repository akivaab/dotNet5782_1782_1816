using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public Station AddStation(int stationID, int name, Location location, int numAvailableChargingSlots)
        {
            try
            {
                DalObject.AddStation(stationID, name, numAvailableChargingSlots, location.Latitude, location.Longitude);
            }
            catch (DalApi.DO.IllegalArgumentException)
            {
                throw new IllegalArgumentException();
            }
            catch (DalApi.DO.NonUniqueIdException)
            {
                throw new NonUniqueIdException();
            }
            Station station = new(stationID, name, location, numAvailableChargingSlots, new List<DroneCharging>());
            return station;
        }
        public void UpdateStation(int stationID, int name = -1, int totalChargingSlots = -1)
        {
            try
            {
                if (name != -1)
                {
                    DalObject.UpdateStationName(stationID, name);
                }
                if (totalChargingSlots != -1)
                {
                    DalApi.DO.Station dalStation = DalObject.DisplayStation(stationID);

                    //find the amount of drones in this station
                    List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location.Latitude == dalStation.Latitude && d.Location.Longitude == dalStation.Longitude);
                    
                    int availableChargeSlots = totalChargingSlots - dronesAtStation.Count;
                    DalObject.UpdateStationChargeSlots(stationID, availableChargeSlots);
                }
            }
            catch (DalApi.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public Station DisplayStation(int stationID)
        {
            try
            {
                DalApi.DO.Station dalStation = DalObject.DisplayStation(stationID);

                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);

                //find drones at this station
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location.Latitude == stationLocation.Latitude && d.Location.Longitude == stationLocation.Longitude);

                //initialize DroneCharging entities
                List<DroneCharging> dronesCharging = new();
                foreach (DroneToList drone in dronesAtStation)
                {
                    dronesCharging.Add(new DroneCharging(drone.ID, drone.Battery));
                }

                return new Station(dalStation.ID, dalStation.Name, stationLocation, dalStation.AvailableChargeSlots, dronesCharging);
            }
            catch (DalApi.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public List<StationToList> DisplayAllStations()
        {
            List<DalApi.DO.Station> dalStations = (List<DalApi.DO.Station>)DalObject.DisplayStationsList();
            List<StationToList> stationToLists = new();

            foreach (DalApi.DO.Station dalStation in dalStations)
            {
                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location.Latitude == stationLocation.Latitude && d.Location.Longitude == stationLocation.Longitude);
                stationToLists.Add(new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count));
            }

            return stationToLists;
        }
        public List<StationToList> FindStations(Predicate<DalApi.DO.Station> predicate)
        {
            List<DalApi.DO.Station> dalStations = (List<DalApi.DO.Station>)DalObject.FindStations(predicate);
            List<StationToList> stationToLists = new();
            
            foreach (DalApi.DO.Station dalStation in dalStations)
            {
                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location.Latitude == stationLocation.Latitude && d.Location.Longitude == stationLocation.Longitude);
                stationToLists.Add(new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count));
            }
            
            return stationToLists;
        }
    }
}