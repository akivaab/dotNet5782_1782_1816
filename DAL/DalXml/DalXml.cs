using System;
using DO;

namespace DalXml
{
    /// <summary>
    /// Initialization and Singleton implementation of the Data Layer.
    /// </summary>
    sealed partial class DalXml : DalApi.IDal
    {
        #region Fields and Properties
        /// <summary>
        /// Lazy and implicitly thread-safe initialization of a DalXml.
        /// </summary>
        private static readonly Lazy<DalXml> lazyDalXml = new Lazy<DalXml>(() => new DalXml());

        /// <summary>
        /// Instance of the DalXml that is first instantiated when the getter is called.
        /// </summary>
        internal static DalXml instance => lazyDalXml.Value;

        /// <summary>
        /// Path to the directory containing the XML files.
        /// </summary>
        private static readonly string directory = @"Data\";

        /// <summary>
        /// Path to the XML file storing configuration data. 
        /// </summary>
        private static readonly string configXmlPath = directory + @"config.xml";

        /// <summary>
        /// Path to the XML file storing drone data.
        /// </summary>
        private static readonly string droneXmlPath = directory + @"Drones.xml";

        /// <summary>
        /// Path to the XML file storing station data.
        /// </summary>
        private static readonly string stationXmlPath = directory + @"Stations.xml";

        /// <summary>
        /// Path to the XML file storing customer data.
        /// </summary>
        private static readonly string customerXmlPath = directory + @"Customers.xml";

        /// <summary>
        /// Path to the XML file storing package data.
        /// </summary>
        private static readonly string packageXmlPath = directory + @"Packages.xml";

        /// <summary>
        /// Path to the XML file storing droneCharge data.
        /// </summary>
        private static readonly string droneChargeXmlPath = directory + @"DroneCharges.xml";

        public bool DataCleanupRequired { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// A constructor that ensures the existance of a directory of xml files.
        /// Private to maintain Singleton design pattern.
        /// </summary>
        /// <exception cref="XMLFileLoadCreateException">Failed to initialize directory or files.</exception>
        private DalXml()
        {
            bool initializedNewXMLFiles = initializeXMLFiles();
            DataCleanupRequired = initializedNewXMLFiles;
        }
        #endregion
    }
}
