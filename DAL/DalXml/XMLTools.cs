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
        private void initializeXMLFiles()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(droneXmlPath))
            {
                saveDronesList(DalObject.DataSource.drones);
            }
            if (!File.Exists(stationXmlPath))
            {
                saveListToXMLSerializer<Station>(DalObject.DataSource.stations, stationXmlPath);
            }
            if (!File.Exists(customerXmlPath))
            {
                saveListToXMLSerializer<Customer>(DalObject.DataSource.customers, customerXmlPath);
            }
            if (!File.Exists(packageXmlPath))
            {
                saveListToXMLSerializer<Package>(DalObject.DataSource.packages, packageXmlPath);
            }
            if (!File.Exists(droneChargeXmlPath))
            {
                saveListToXMLSerializer<DroneCharge>(DalObject.DataSource.droneCharges, droneChargeXmlPath);
            }
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
            catch (Exception ex)
            {
                //throw new DO.XMLFileLoadCreateException(filePath, $"fail to create xml file: {filePath}", ex);
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
            catch (Exception ex)
            {
                //throw new DO.XMLFileLoadCreateException(filePath, $"fail to load xml file: {filePath}", ex);
            }
            return null;
        }
        #endregion

        #region Save/Load with XElement (Drone)
        /// <summary>
        /// Save a Drone root element to an XML file.
        /// </summary>
        private void saveDrones()
        {
            try
            {
                droneRoot.Save(droneXmlPath);
            }
            catch
            {
                //throw new exception
            }
        }

        /// <summary>
        /// Sava a list of drones to an XML file.
        /// </summary>
        /// <param name="drones">A list of drones.</param>
        private void saveDronesList(List<Drone> drones)
        {
            try
            {
                droneRoot = new XElement("drones",
                    from d in drones
                    select new XElement("drone",
                                        new XElement("id", d.ID),
                                        new XElement("model", d.Model),
                                        new XElement("maxWeight", d.MaxWeight),
                                        new XElement("active", d.Active)));
                saveDrones();
            }
            catch
            {
                //throw new exception
            }
        }

        /// <summary>
        /// Load a Drone root element from an XML file.
        /// </summary>
        private void loadDrones()
        {
            try
            {
                droneRoot = XElement.Load(droneXmlPath);
            }
            catch
            {
                //throw new exception  
            }
        }
        #endregion
    }
}
