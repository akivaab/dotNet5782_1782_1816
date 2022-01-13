using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DO;

namespace DalXml
{
    partial class DalXml : DalApi.IDal
    {
        #region Add Methods
        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight)
        {
            XElement droneRoot = loadElementFromXML<Drone>(droneXmlPath);

            if (activeDroneExists(droneRoot, id))
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }

            Drone drone = new();
            drone.ID = id;
            drone.Model = model;
            drone.MaxWeight = maxWeight;
            drone.Active = true;
            droneRoot.Add(drone);

            saveElementToXML<Drone>(droneRoot, droneXmlPath);
        }
        #endregion

        #region Update Methods
        public void ChargeDrone(int droneID, int stationID)
        {
            XElement droneRoot = loadElementFromXML<Drone>(droneXmlPath);

            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);
            int stationIndex = stations.FindIndex(station => station.ID == stationID && station.Active);

            if (!activeDroneExists(droneRoot, droneID) || stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (stationIndex == -1 ? "station" : "drone") + " with the given ID.");
            }

            Station station = stations[stationIndex];
            station.AvailableChargeSlots--;
            stations[stationIndex] = station;
            saveListToXMLSerializer<Station>(stations, stationXmlPath);

            List<DroneCharge> droneCharges = loadListFromXMLSerializer<DroneCharge>(droneChargeXmlPath);
            DroneCharge droneCharge = new DroneCharge();
            droneCharge.DroneID = droneID;
            droneCharge.StationID = stationID;
            droneCharge.BeganCharge = DateTime.Now;
            droneCharge.Active = true;
            droneCharges.Add(droneCharge);
            saveListToXMLSerializer<DroneCharge>(droneCharges, droneChargeXmlPath);
        }

        public void ReleaseDroneFromCharging(int droneID, int stationID)
        {
            XElement droneRoot = loadElementFromXML<Drone>(droneXmlPath);

            List<Station> stations = loadListFromXMLSerializer<Station>(stationXmlPath);
            List<DroneCharge> droneCharges = loadListFromXMLSerializer<DroneCharge>(droneChargeXmlPath);

            int stationIndex = stations.FindIndex(station => station.ID == stationID);
            int droneChargeIndex = droneCharges.FindIndex(droneCharge => droneCharge.DroneID == droneID && droneCharge.StationID == stationID && droneCharge.Active);

            if (!activeDroneExists(droneRoot, droneID) || stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (stationIndex == -1 ? "station" : "drone") + " with the given ID.");
            }
            if (droneChargeIndex == -1)
            {
                throw new UndefinedObjectException("The given drone is not charging in the given station.");
            }

            Station station = stations[stationIndex];
            station.AvailableChargeSlots++;
            stations[stationIndex] = station;
            saveListToXMLSerializer<Station>(stations, stationXmlPath);

            DroneCharge droneCharge = droneCharges[droneChargeIndex];
            droneCharge.Active = false;
            droneCharges[droneChargeIndex] = droneCharge;
            saveListToXMLSerializer<DroneCharge>(droneCharges, droneChargeXmlPath);
        }

        public void UpdateDroneModel(int droneID, string model)
        {
            List<Drone> drones = loadDronesList();

            int droneIndex = drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            Drone drone = drones[droneIndex];
            drone.Model = model;
            drones[droneIndex] = drone;
        }
        #endregion
    }
}
