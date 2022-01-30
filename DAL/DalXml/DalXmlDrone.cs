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
            XElement droneRoot = loadElementFromXML(droneXmlPath);

            if (activeDroneExists(droneRoot, id))
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }

            droneRoot.Add(new XElement("Drone",
                                        new XElement("ID", id),
                                        new XElement("Model", model),
                                        new XElement("MaxWeight", maxWeight),
                                        new XElement("Active", true)));
            saveElementToXML(droneRoot, droneXmlPath);
        }
        #endregion

        #region Update Methods
        public void ChargeDrone(int droneID, int stationID)
        {
            XElement droneRoot = loadElementFromXML(droneXmlPath);

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
            XElement droneRoot = loadElementFromXML(droneXmlPath);

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
            XElement droneRoot = loadElementFromXML(droneXmlPath);

            XElement drone = extractDrone(droneRoot, droneID);
            drone.Element("Model").Value = model;

            saveElementToXML(droneRoot, droneXmlPath);
        }
        #endregion

        #region Remove Methods
        public void RemoveDrone(int droneID)
        {
            XElement droneRoot = loadElementFromXML(droneXmlPath);

            XElement drone = extractDrone(droneRoot, droneID);
            drone.Element("Active").Value = false.ToString();

            saveElementToXML(droneRoot, droneXmlPath);
        }
        #endregion

        #region Getter Methods
        public Drone GetDrone(int droneID)
        {
            XElement droneRoot = loadElementFromXML(droneXmlPath);
            XElement drone = extractDrone(droneRoot, droneID);

            return new Drone()
            {
                ID = droneID,
                Model = drone.Element("Model").Value,
                MaxWeight = (Enums.WeightCategories)Enum.Parse(typeof(Enums.WeightCategories), drone.Element("MaxWeight").Value),
                Active = true
            };
        }

        public IEnumerable<Drone> GetDronesList()
        {
            XElement droneRoot = loadElementFromXML(droneXmlPath);
            return from drone in droneRoot.Elements()
                   where bool.Parse(drone.Element("Active").Value)
                   select new Drone()
                   {
                       ID = int.Parse(drone.Element("ID").Value),
                       Model = drone.Element("Model").Value,
                       MaxWeight = (Enums.WeightCategories)Enum.Parse(typeof(Enums.WeightCategories), drone.Element("MaxWeight").Value),
                       Active = true
                   };
        }

        public IEnumerable<double> DronePowerConsumption()
        {
            double[] powerConsumptionValues = new double[5];
            XElement configRoot = loadElementFromXML(configXmlPath);

            powerConsumptionValues[0] = double.Parse(configRoot.Element("free").Value);
            powerConsumptionValues[1] = double.Parse(configRoot.Element("lightWeight").Value);
            powerConsumptionValues[2] = double.Parse(configRoot.Element("midWeight").Value);
            powerConsumptionValues[3] = double.Parse(configRoot.Element("heavyWeight").Value);
            powerConsumptionValues[4] = double.Parse(configRoot.Element("chargingRate").Value);
            return powerConsumptionValues;
        }
        #endregion
    }
}
