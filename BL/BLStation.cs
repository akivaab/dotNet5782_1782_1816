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
            catch (IDAL.DO.IllegalArgumentException)
            {
                throw new IllegalArgumentException();
            }
            catch (IDAL.DO.NonUniqueIdException)
            {
                throw new NonUniqueIdException();
            }
            Station station = new(stationID, name, location, numAvailableChargingSlots, new List<DroneCharging>());
            return station;
        }
        public void UpdateStation(int stationID, int name = -1, int totalChargingSlots = -1)
        {
            List<IDAL.DO.Station> dalStationList = (List<IDAL.DO.Station>)DalObject.DisplayStationsList();
            int stationIndex = dalStationList.FindIndex(s => s.ID == stationID);
            if (stationIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            IDAL.DO.Station dalStation = dalStationList[stationIndex];

            dalStationList.RemoveAt(stationIndex);
            if (name != -1)
            {
                dalStation.Name = name;
            }
            if (totalChargingSlots != -1)
            {
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == new Location(dalStation.Latitude, dalStation.Longitude));
                dalStation.AvailableChargeSlots = totalChargingSlots - dronesAtStation.Count;
            }
            dalStationList.Insert(stationIndex, dalStation);
        }
        public Station DisplayStation(int stationID)
        {
            try
            {
                IDAL.DO.Station dalStation = DalObject.DisplayStation(stationID);

                Location stationLocation = new Location(dalStation.Latitude, dalStation.Longitude);

                //find drones at this station
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == stationLocation);

                //initialize DroneCharging entities
                List<DroneCharging> dronesCharging = new();
                foreach (DroneToList drone in dronesAtStation)
                {
                    dronesCharging.Add(new DroneCharging(drone.ID, drone.Battery));
                }

                return new Station(dalStation.ID, dalStation.Name, stationLocation, dalStation.AvailableChargeSlots, dronesCharging);
            }
            catch (IDAL.DO.UndefinedObjectException)
            {
                throw new UndefinedObjectException();
            }
        }
        public List<StationToList> DisplayAllStations()
        {
            List<IDAL.DO.Station> dalStations = new(DalObject.DisplayStationsList());
            List<StationToList> stationToLists = new();
            foreach (IDAL.DO.Station dalStation in dalStations)
            {
                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == stationLocation);
                stationToLists.Add(new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count));
            }
            return stationToLists;
        }
        public List<StationToList> DisplayFreeStations()
        {
            List<IDAL.DO.Station> dalStations = new(DalObject.DisplayFreeStationsList());
            List<StationToList> stationToLists = new();
            foreach (IDAL.DO.Station dalStation in dalStations)
            {
                Location stationLocation = new(dalStation.Latitude, dalStation.Longitude);
                List<DroneToList> dronesAtStation = Drones.FindAll(d => d.Location == stationLocation);
                stationToLists.Add(new StationToList(dalStation.ID, dalStation.Name, dalStation.AvailableChargeSlots, dronesAtStation.Count));
            }
            return stationToLists;
        }
    }
}