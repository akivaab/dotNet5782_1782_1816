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
        /// Path to the directory containing the XML files.
        /// </summary>
        private static string directory = @"Data\";

        /// <summary>
        /// Path to the XML file storing configuration data. 
        /// </summary>
        private string configXmlPath = directory + @"config.xml";

        /// <summary>
        /// Path to the XML file storing drone data.
        /// </summary>
        private string droneXmlPath = directory + @"Drones.xml";

        /// <summary>
        /// Path to the XML file storing station data.
        /// </summary>
        private string stationXmlPath = directory + @"Stations.xml";

        /// <summary>
        /// Path to the XML file storing customer data.
        /// </summary>
        private string customerXmlPath = directory + @"Customers.xml";

        /// <summary>
        /// Path to the XML file storing package data.
        /// </summary>
        private string packageXmlPath = directory + @"Packages.xml";

        /// <summary>
        /// Path to the XML file storing droneCharge data.
        /// </summary>
        private string droneChargeXmlPath = directory + @"DroneCharges.xml";
        #endregion

        #region Constructors
        /// <summary>
        /// A constructor that ensures the existance of a directory of xml files.
        /// Visibility is private to maintain Singleton design pattern.
        /// </summary>
        private DalXml()
        {
            initializeXMLFiles();
        }
        #endregion
    }
}
