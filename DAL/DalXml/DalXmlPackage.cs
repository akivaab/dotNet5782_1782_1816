using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DO;

namespace DalXml
{
    /// <summary>
    /// Package-related functionality of the Data Layer.
    /// </summary>
    partial class DalXml : DalApi.IDal
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            List<Customer> customers = loadListFromXMLSerializer<Customer>(customerXmlPath);
            bool senderExists = customers.Exists(customer => customer.ID == senderID && customer.Active);
            bool receiverExists = customers.Exists(customer => customer.ID == receiverID && customer.Active);
            if (!(senderExists && receiverExists))
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            XElement configRoot = loadElementFromXML(configXmlPath);
            int packageID = int.Parse(configRoot.Element("packageID").Value);

            Package package = new();
            package.ID = packageID;
            package.SenderID = senderID;
            package.ReceiverID = receiverID;
            package.Weight = weight;
            package.Priority = priority;
            package.DroneID = null;
            package.Requested = DateTime.Now;
            package.Assigned = null;
            package.Collected = null;
            package.Delivered = null;
            package.Active = true;

            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            packages.Add(package);
            saveListToXMLSerializer<Package>(packages, packageXmlPath);

            //increment package ID in config file
            configRoot.Element("packageID").Value = (packageID + 1).ToString();
            saveElementToXML(configRoot, configXmlPath);

            return package.ID;
        }
        #endregion

        #region Update Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AssignPackage(int packageID, int droneID)
        {
            XElement droneRoot = loadElementFromXML(droneXmlPath);

            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            int packageIndex = packages.FindIndex(package => package.ID == packageID && package.Active);
            if (!activeDroneExists(droneRoot, droneID) || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (packageIndex == -1 ? "package" : "drone") + " with the given ID.");
            }

            Package package = packages[packageIndex];
            package.DroneID = droneID;
            package.Assigned = DateTime.Now;
            packages[packageIndex] = package;

            saveListToXMLSerializer<Package>(packages, packageXmlPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CollectPackage(int packageID, int droneID)
        {
            XElement droneRoot = loadElementFromXML(droneXmlPath);

            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            int packageIndex = packages.FindIndex(package => package.ID == packageID && package.Active);
            if (!activeDroneExists(droneRoot, droneID) || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (packageIndex == -1 ? "package" : "drone") + " with the given ID.");
            }

            Package package = packages[packageIndex];
            package.Collected = DateTime.Now;
            packages[packageIndex] = package;

            saveListToXMLSerializer<Package>(packages, packageXmlPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverPackage(int packageID, int droneID)
        {
            XElement droneRoot = loadElementFromXML(droneXmlPath);

            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            int packageIndex = packages.FindIndex(package => package.ID == packageID && package.Active);

            if (!activeDroneExists(droneRoot, droneID) || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (packageIndex == -1 ? "package" : "drone") + " with the given ID.");
            }

            Package package = packages[packageIndex];
            package.Delivered = DateTime.Now;
            packages[packageIndex] = package;

            saveListToXMLSerializer<Package>(packages, packageXmlPath);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ModifyPackageStatus(int packageID, DateTime? assigned, DateTime? collected, DateTime? delivered)
        {
            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            int packageIndex = packages.FindIndex(package => package.ID == packageID && package.Active);
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }

            Package package = packages[packageIndex];
            package.Assigned = assigned;
            package.Collected = collected;
            package.Delivered = delivered;
            packages[packageIndex] = package;

            saveListToXMLSerializer<Package>(packages, packageXmlPath);
        }
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemovePackage(int packageID)
        {
            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            int packageIndex = packages.FindIndex(package => package.ID == packageID && package.Active);
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }

            Package package = packages[packageIndex];
            package.Active = false;
            packages[packageIndex] = package;

            saveListToXMLSerializer<Package>(packages, packageXmlPath);
        }
        #endregion

        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Package GetPackage(int packageID)
        {
            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            int packageIndex = packages.FindIndex(package => package.ID == packageID && package.Active);
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }

            return packages[packageIndex].Clone();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> GetPackagesList()
        {
            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            return from package in packages
                   where package.Active
                   select package.Clone();
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> FindPackages(Predicate<Package> predicate)
        {
            List<Package> packages = loadListFromXMLSerializer<Package>(packageXmlPath);
            return from package in packages
                   where predicate(package) && package.Active
                   select package.Clone();
        }
        #endregion
    }
}
