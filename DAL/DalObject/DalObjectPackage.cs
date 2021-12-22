﻿using System;
using System.Collections.Generic;
using System.Linq;
using DO;

namespace DalObject
{
    partial class DalObject : DalApi.IDal
    {
        public int AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority, int? droneID = null)
        {
            if (DataSource.Customers.FindIndex(customer => customer.ID == senderID) == -1 ||
                DataSource.Customers.FindIndex(customer => customer.ID == receiverID) == -1)
            {
                throw new UndefinedObjectException("There is no customer with the given ID.");
            }
            if (droneID != null && DataSource.Drones.FindIndex(drone => drone.ID == droneID) == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            Package package = new();
            package.ID = DataSource.Config.PackageID;
            package.SenderID = senderID;
            package.ReceiverID = receiverID;
            package.Weight = weight;
            package.Priority = priority;
            package.DroneID = droneID;
            package.Requested = DateTime.Now;
            package.Assigned = null;
            package.Collected = null;
            package.Delivered = null;
            DataSource.Config.PackageID++;
            DataSource.Packages.Add(package);
            return DataSource.Config.PackageID;
        }

        public void AssignPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "package") + " with the given ID.");
            }
            
            Package package = DataSource.Packages[packageIndex];
            package.DroneID = droneID;
            package.Assigned = DateTime.Now;
            DataSource.Packages[packageIndex] = package;
        }

        public void CollectPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "package") + " with the given ID.");
            }
            
            Package package = DataSource.Packages[packageIndex];
            package.Collected = DateTime.Now;
            DataSource.Packages[packageIndex] = package;
        }

        public void DeliverPackage(int packageID, int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            
            if (droneIndex == -1 || packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "package") + " with the given ID.");
            }
            
            Package package = DataSource.Packages[packageIndex];
            package.Delivered = DateTime.Now;
            package.DroneID = 0;
            DataSource.Packages[packageIndex] = package;
        }

        public void RemovePackage(int packageID)
        {
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }
            
            DataSource.Packages.RemoveAt(packageIndex);
        }

        public void ModifyPackageStatus(int packageID, DateTime? assigned, DateTime? collected, DateTime? delivered)
        {
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }

            Package package = DataSource.Packages[packageIndex];
            package.Assigned = assigned;
            package.Collected = collected;
            package.Delivered = delivered;
            DataSource.Packages[packageIndex] = package;
        }

        public Package DisplayPackage(int packageID)
        {
            int packageIndex = DataSource.Packages.FindIndex(package => package.ID == packageID);
            
            if (packageIndex == -1)
            {
                throw new UndefinedObjectException("There is no package with the given ID.");
            }
            
            return DataSource.Packages[packageIndex];
        }

        public IEnumerable<Package> DisplayPackagesList()
        {
            return from package in DataSource.Packages
                   select package;
        }
            
        public IEnumerable<Package> FindPackages(Predicate<Package> predicate)
        {
            return from package in DataSource.Packages
                   where predicate(package)
                   select package;
        }
    }
}