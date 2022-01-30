using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using DO;

namespace DalXml
{
    /// <summary>
    /// Auxiliary methods for working with XML files.
    /// </summary>
    partial class DalXml
    {
        #region XML Initialization
        /// <summary>
        /// Initialize a directory of XML files (if they don't exist yet).
        /// </summary>
        /// <returns>True if XML files were initialized, false otherwise.</returns>
        private bool initializeXMLFiles()
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception)
            {
                throw new XMLFileLoadCreateException("Failed to create directory.");
            }

            if (!File.Exists(droneXmlPath) || !File.Exists(stationXmlPath) || !File.Exists(customerXmlPath) || !File.Exists(packageXmlPath) || !File.Exists(droneChargeXmlPath))
            {
                saveDronesList(DalObject.DataSource.drones);
                saveListToXMLSerializer<Station>(DalObject.DataSource.stations, stationXmlPath);
                saveListToXMLSerializer<Customer>(DalObject.DataSource.customers, customerXmlPath);
                saveListToXMLSerializer<Package>(DalObject.DataSource.packages, packageXmlPath);
                saveListToXMLSerializer<DroneCharge>(DalObject.DataSource.droneCharges, droneChargeXmlPath);

                int packageIDConfig = loadListFromXMLSerializer<Package>(packageXmlPath).Last().ID + 1;
                XElement configRoot = new XElement("Config",
                                                    new XElement("packageID", packageIDConfig),
                                                    new XElement("free", 0.01),
                                                    new XElement("lightWeight", 0.05),
                                                    new XElement("midWeight", 0.1),
                                                    new XElement("heavyWeight", 0.15),
                                                    new XElement("chargingRate", 20.0));
                saveElementToXML(configRoot, configXmlPath);
                return true;
            }
            return false;
        }
        #endregion

        #region Save/Load with XMLSerializer
        /// <summary>
        /// Serialize a list to XML.
        /// </summary>
        /// <typeparam name="T">The entity being serialized.</typeparam>
        /// <param name="list">A list of T entities.</param>
        /// <param name="filePath">The path to the XML file being written upon.</param>
        private static void saveListToXMLSerializer<T>(List<T> list, string filePath)
        {
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Create);
                XmlSerializer x = new XmlSerializer(list.GetType());
                x.Serialize(file, list);
                file.Close();
            }
            catch (Exception)
            {
                throw new XMLFileLoadCreateException($"Failed to save XML file: {filePath}");
            }
        }

        /// <summary>
        /// Deserialize a list from XML.
        /// </summary>
        /// <typeparam name="T">The entity being deserialized.</typeparam>
        /// <param name="filePath">The path to the XML file being read from.</param>
        /// <returns>A list of T entities.</returns>
        private static List<T> loadListFromXMLSerializer<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    List<T> list;
                    XmlSerializer x = new XmlSerializer(typeof(List<T>));
                    FileStream file = new FileStream(filePath, FileMode.Open);
                    list = (List<T>)x.Deserialize(file);
                    file.Close();
                    return list;
                }
                else
                {
                    return new List<T>();
                }
            }
            catch (Exception)
            {
                throw new XMLFileLoadCreateException($"Failed to load XML file: {filePath}");
            }
        }
        #endregion

        #region Save/Load with XElement (Drone)
        /// <summary>
        /// Save a root XElement to an XML file.
        /// </summary>
        /// <param name="root">The root element.</param>
        /// <param name="filePath">The path to the XML file.</param>
        private void saveElementToXML(XElement root, string filePath)
        {
            try
            {
                root.Save(filePath);
            }
            catch (Exception)
            {
                throw new XMLFileLoadCreateException($"Failed to save XML file: {droneXmlPath}");
            }
        }

        /// <summary>
        /// Load the root XElement from an XML file.
        /// </summary>
        /// <param name="filePath">The path to the XML file.</param>
        /// <returns>The root XElement.</returns>
        private XElement loadElementFromXML(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return XElement.Load(filePath);
                }
            }
            catch (Exception)
            {
                throw new XMLFileLoadCreateException($"Failed to load XML file: {filePath}");
            }
            return null;
        }

        /// <summary>
        /// Sava a list of drones to an XML file.
        /// </summary>
        /// <param name="drones">A list of drones.</param>
        private void saveDronesList(List<Drone> drones)
        {
            try
            {
                XElement droneRoot = new XElement("Drones",
                    from d in drones
                    select new XElement("Drone",
                                        new XElement("ID", d.ID),
                                        new XElement("Model", d.Model),
                                        new XElement("MaxWeight", d.MaxWeight),
                                        new XElement("Active", d.Active)));
                saveElementToXML(droneRoot, droneXmlPath);
            }
            catch (Exception)
            {
                throw new XMLFileLoadCreateException($"Failed to save XML file: {droneXmlPath}");
            }
        }

        [Obsolete]
        /// <summary>
        /// Load a list of drones from an XML file.
        /// </summary>
        /// <returns>The list of drones.</returns>
        private List<Drone> loadDronesList()
        {
            try
            {
                XElement droneRoot = loadElementFromXML(droneXmlPath);
                return (from drone in droneRoot.Elements()
                        select new Drone()
                        {
                            ID = int.Parse(drone.Element("ID").Value),
                            Model = drone.Element("Model").Value,
                            MaxWeight = (Enums.WeightCategories)Enum.Parse(typeof(Enums.WeightCategories), drone.Element("MaxWeight").Value),
                            Active = bool.Parse(drone.Element("Active").Value)
                        }).ToList();
            }
            catch (Exception)
            {
                throw new XMLFileLoadCreateException($"Failed to load XML file: {droneXmlPath}");
            }
        }
        #endregion

        #region Query XML (Drone)
        /// <summary>
        /// Check if there is an active drone with a given ID in an XML file.
        /// </summary>
        /// <param name="droneRoot">The root element of an XML file.</param>
        /// <param name="droneID">The ID of the drone being searched for.</param>
        /// <returns>True if the drone exists, false otherwise.</returns>
        private bool activeDroneExists(XElement droneRoot, int droneID)
        {
            try
            {
                extractDrone(droneRoot, droneID);
                return true;
            }
            catch (UndefinedObjectException)
            {
                return false;
            }
        }

        /// <summary>
        /// Extract a drone child element from an XML file.
        /// </summary>
        /// <param name="droneRoot">The root element of an XML file.</param>
        /// <param name="droneID">The ID of the drone being searched for.</param>
        /// <returns>The drone child element.</returns>
        private XElement extractDrone(XElement droneRoot, int droneID)
        {
            try
            {
                XElement drone = (from d in droneRoot.Elements()
                                where int.Parse(d.Element("ID").Value) == droneID && bool.Parse(d.Element("Active").Value)
                                select d).SingleOrDefault(); /*.DefaultIfEmpty(null).Single();*/

                if (drone == null) /*default(XElement)*/
                {
                    throw new UndefinedObjectException("There is no drone with the given ID.");
                }

                return drone;
            }
            catch (ArgumentNullException)
            {
                throw new UndefinedObjectException("The XML file of drones is null.");
            }
            catch (InvalidOperationException)
            {
                throw new NonUniqueIdException("Multiple drones have the same ID.");
            }
        }
        #endregion
    }
}
