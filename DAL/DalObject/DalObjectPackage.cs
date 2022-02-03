using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DO;

namespace DalObject
{
    /// <summary>
    /// Package-related functionality of the Data Layer.
    /// </summary>
    partial class DalObject : DalApi.IDal
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public int AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {   
            bool senderExists = DataSource.customers.Exists(customer => customer.ID == senderID && customer.Active);
            bool receiverExists = DataSource.customers.Exists(customer => customer.ID == receiverID && customer.Active);
            if (!(senderExists && receiverExists))
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
        [MethodImpl(MethodImplOptions.Synchronized)]
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

        [MethodImpl(MethodImplOptions.Synchronized)]
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

        [MethodImpl(MethodImplOptions.Synchronized)]
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

        [MethodImpl(MethodImplOptions.Synchronized)]
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
        [MethodImpl(MethodImplOptions.Synchronized)]
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Package GetPackage(int packageID)
        {
            int packageIndex = DataSource.packages.FindIndex(package => package.ID == packageID && package.Active);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }
            
            return DataSource.packages[packageIndex];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> GetPackagesList()
        {
            return from package in DataSource.packages
                   where package.Active
                   select package;
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> FindPackages(Predicate<Package> predicate)
        {
            return from package in DataSource.packages
                   where predicate(package) && package.Active
                   select package;
        }
        #endregion
    }
}
