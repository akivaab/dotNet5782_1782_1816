using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using DO;

namespace DalXml
{
    /// <summary>
    /// Initialization and Singleton implementation of the Data Layer.
    /// </summary>
    sealed partial class DalXml : DalApi.IDal
    {
        #region Fields
        /// <summary>
        /// Lazy and implicitly thread-safe initialization of a DalXml.
        /// </summary>
        private static readonly Lazy<DalXml> lazyDalXml = new Lazy<DalXml>(() => new DalXml());

        /// <summary>
        /// Instance of the DalXml that is first instantiated when the getter is called.
        /// </summary>
        internal static DalXml instance { get { return lazyDalXml.Value; } }

        /// <summary>
        /// Path to the directory containing the xml files.
        /// </summary>
        internal static string directory = @"Data\";

        /// <summary>
        /// Path to the xml file storing drone data.
        /// </summary>
        internal string droneXmlPath = directory + @"Drones.xml";

        /// <summary>
        /// Path to the xml file storing station data.
        /// </summary>
        internal string stationXmlPath = directory + @"Stations.xml";

        /// <summary>
        /// Path to the xml file storing customer data.
        /// </summary>
        internal string customerXmlPath = directory + @"Customers.xml";

        /// <summary>
        /// Path to the xml file storing package data.
        /// </summary>
        internal string packageXmlPath = directory + @"Packages.xml";

        /// <summary>
        /// Path to the xml file storing droneCharge data.
        /// </summary>
        internal string droneChargeXmlPath = directory + @"DroneCharges.xml";
        #endregion

        #region Constructors
        /// <summary>
        /// A constructor that ensures the existance of a directory to store xml files.
        /// Visibility is private to maintain Singleton design pattern.
        /// </summary>
        private DalXml()
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(directory + droneXmlPath))
            {
                //add using XElement
            }
            if (!File.Exists(directory + stationXmlPath))
            {
                XMLSerializer.SaveListToXMLSerializer<Station>(DalObject.DataSource.stations, directory + stationXmlPath);
            }
            if (!File.Exists(directory + customerXmlPath))
            {
                XMLSerializer.SaveListToXMLSerializer<Customer>(DalObject.DataSource.customers, directory + customerXmlPath);
            }
            if (!File.Exists(directory + packageXmlPath))
            {
                XMLSerializer.SaveListToXMLSerializer<Package>(DalObject.DataSource.packages, directory + packageXmlPath);
            }
            if (!File.Exists(directory + droneChargeXmlPath))
            {
                XMLSerializer.SaveListToXMLSerializer<DroneCharge>(DalObject.DataSource.droneCharges, directory + droneChargeXmlPath);
            }
        }
        #endregion
    }
}
