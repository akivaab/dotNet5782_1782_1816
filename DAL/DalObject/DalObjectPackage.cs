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
        public int AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {   
            int senderIndex = DataSource.customers.FindIndex(customer => customer.ID == senderID && customer.Active);
            int receiverIndex = DataSource.customers.FindIndex(customer => customer.ID == receiverID && customer.Active);
            if (senderIndex == -1 || receiverIndex == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }

            Package package = new();
            package.ID = DataSource.Config.packageID;
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
            DataSource.Config.packageID++;
            DataSource.packages.Add(package);
            return package.ID;
        }
        #endregion

        #region Update Methods
        public void AssignPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID && package.Active);
            
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
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID && package.Active);
            
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
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID && package.Active);
            
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "package") + " with the given ID.");
            }
            
            Package package = DataSource.packages[packageIndex];
            package.Delivered = DateTime.Now;
            DataSource.packages[packageIndex] = package;
        }

        public void ModifyPackageStatus(int packageID, DateTime? assigned, DateTime? collected, DateTime? delivered)
        {
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID && package.Active);

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

        #region Remove Methods
        public void RemovePackage(int packageID)
        {
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID && package.Active);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }

            Package package = DataSource.packages[packageIndex];
            package.Active = false;
            DataSource.packages[packageIndex] = package;
        }
        #endregion

        #region Getter Methods
        public Package GetPackage(int packageID)
        {
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID && package.Active);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }
            
            return DataSource.packages[packageIndex];
        }

        public IEnumerable<Package> GetPackagesList()
        {
            return from package in DataSource.packages
                   where package.Active
                   select package;
        }
        #endregion

        #region Find Methods
        public IEnumerable<Package> FindPackages(Predicate<Package> predicate)
        {
            return from package in DataSource.packages
                   where predicate(package) && package.Active
                   select package;
        }
        #endregion
    }
}
