using System;
using System.Collections.Generic;
using System.Linq;
using DO;

namespace DalObject
{
    /// <summary>
    /// Package-related functionality of the Data Layer.
    /// </summary>
    partial class DalObject : DalApi.IDal
    {
        #region Add Methods

        public int AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority, int? droneID = null)
        {
            if (DataSource.customers.FindIndex(customer => customer.ID == senderID) == -1 ||
                DataSource.customers.FindIndex(customer => customer.ID == receiverID) == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }
            if (droneID != null && DataSource.drones.FindIndex(drone => drone.ID == droneID) == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            Package package = new();
            package.ID = DataSource.Config.packageID;
            package.SenderID = senderID;
            package.ReceiverID = receiverID;
            package.Weight = weight;
            package.Priority = priority;
            package.DroneID = droneID;
            package.Requested = DateTime.Now;
            package.Assigned = null;
            package.Collected = null;
            package.Delivered = null;
            DataSource.Config.packageID++;
            DataSource.packages.Add(package);
            return DataSource.Config.packageID;
        }

        #endregion

        #region Update Methods

        public void AssignPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID);
            
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "package") + " with the given ID.");
            }
            
            Package package = DataSource.packages[packageIndex];
            package.DroneID = droneID;
            package.Assigned = DateTime.Now;
            DataSource.packages[packageIndex] = package;
        }

        public void CollectPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID);
            
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "package") + " with the given ID.");
            }
            
            Package package = DataSource.packages[packageIndex];
            package.Collected = DateTime.Now;
            DataSource.packages[packageIndex] = package;
        }

        public void DeliverPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID);
            
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "package") + " with the given ID.");
            }
            
            Package package = DataSource.packages[packageIndex];
            package.Delivered = DateTime.Now;
            package.DroneID = 0;
            DataSource.packages[packageIndex] = package;
        }

        public void ModifyPackageStatus(int packageID, DateTime? assigned, DateTime? collected, DateTime? delivered)
        {
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID);

            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }

            Package package = DataSource.packages[packageIndex];
            package.Assigned = assigned;
            package.Collected = collected;
            package.Delivered = delivered;
            DataSource.packages[packageIndex] = package;
        }

        #endregion

        #region Delete Methods

        public void RemovePackage(int packageID)
        {
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }
            
            DataSource.packages.RemoveAt(packageIndex);
        }

        #endregion

        #region Getter Methods

        public Package GetPackage(int packageID)
        {
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }
            
            return DataSource.packages[packageIndex];
        }

        public IEnumerable<Package> GetPackagesList()
        {
            return from package in DataSource.packages
                   select package;
        }

        #endregion

        #region Find Methods

        public IEnumerable<Package> FindPackages(Predicate<Package> predicate)
        {
            return from package in DataSource.packages
                   where predicate(package)
                   select package;
        }

        #endregion
    }
}
