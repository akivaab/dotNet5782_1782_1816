using System;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    /// <summary>
    /// Package-related functionality of the Business Layer.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            try
            {
                DO.Customer sender = DalObject.DisplayCustomer(senderID);
                DO.Customer receiver = DalObject.DisplayCustomer(receiverID);
                int packageID = DalObject.AddPackage(senderID, receiverID, (DO.Enums.WeightCategories)weight, (DO.Enums.Priorities)priority);

                CustomerForPackage packageSender = new(sender.ID, sender.Name);
                CustomerForPackage packageReceiver = new(receiver.ID, receiver.Name);

                Package package = new(packageID, packageSender, packageReceiver, weight, priority, null, DateTime.Now, null, null, null);
                return package;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public void AssignPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (Drones[droneIndex].Status != Enums.DroneStatus.available)
            {
                throw new UnableToAssignException("The drone is not currently available and so cannot be assigned a package.");
            }

            try
            {
                IEnumerable<DO.Package> dalPackages = DalObject.FindPackages(p => p.DroneID == null);
                int bestPackageID = findBestPackage(dalPackages, Drones[droneIndex]);
                DalObject.AssignPackage(bestPackageID, droneID);

                Drones[droneIndex].Status = Enums.DroneStatus.delivery;
                Drones[droneIndex].PackageID = bestPackageID;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        public void CollectPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (Drones[droneIndex].PackageID == null)
            {
                throw new UnableToCollectException("The drone has not been assigned a package.");
            }
            
            DO.Package dalPackage = DalObject.DisplayPackage((int)Drones[droneIndex].PackageID);
            
            if (dalPackage.Assigned == null || dalPackage.Collected != null)
            {
                throw new UnableToCollectException("The drone is currently unable to collect a package.");
            }

            try
            {
                DalObject.CollectPackage(dalPackage.ID, droneID);

                Location senderLocation = getCustomerLocation(dalPackage.SenderID);
                Drones[droneIndex].Battery = Math.Max(Drones[droneIndex].Battery - (PowerConsumption[(int)Enums.WeightCategories.free] * getDistance(Drones[droneIndex].Location, senderLocation)), 0);
                Drones[droneIndex].Location = senderLocation;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public void DeliverPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            //if this drone isn't assigned a package
            if (Drones[droneIndex].PackageID == null)
            {
                throw new UnableToDeliverException("The drone has not been assigned a package.");
            }
            
            DO.Package dalPackage = DalObject.DisplayPackage((int)Drones[droneIndex].PackageID);
            
            //if this package isn't in a state to be delivered
            if (dalPackage.Collected == null || dalPackage.Delivered != null)
            {
                throw new UnableToDeliverException("The drone is currently unable to deliver a package.");
            }

            try
            {
                DalObject.DeliverPackage(dalPackage.ID, droneID);

                Location receiverLocation = getCustomerLocation(dalPackage.ReceiverID);
                Drones[droneIndex].Battery = Math.Max(Drones[droneIndex].Battery - (PowerConsumption[(int)dalPackage.Weight] * getDistance(Drones[droneIndex].Location, receiverLocation)), 0);
                Drones[droneIndex].Location = receiverLocation;
                Drones[droneIndex].Status = Enums.DroneStatus.available;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public Package DisplayPackage(int packageID)
        {
            try
            {
                DO.Package dalPackage = DalObject.DisplayPackage(packageID);

                DO.Customer dalPackageSender = DalObject.DisplayCustomer(dalPackage.SenderID);
                DO.Customer dalPackageReceiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
                CustomerForPackage senderForPackage = new(dalPackageSender.ID, dalPackageSender.Name);
                CustomerForPackage receiverForPackage = new(dalPackageReceiver.ID, dalPackageReceiver.Name);

                int droneIndex = Drones.FindIndex(d => d.ID == dalPackage.DroneID);
                DroneDelivering droneDelivering = droneIndex != -1 ? new(Drones[droneIndex].ID, Drones[droneIndex].Battery, Drones[droneIndex].Location) : null;

                Package package = new(dalPackage.ID, senderForPackage, receiverForPackage, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, droneDelivering, dalPackage.Requested, dalPackage.Assigned, dalPackage.Collected, dalPackage.Delivered);
                return package;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public IEnumerable<PackageToList> DisplayAllPackages()
        {
            try
            {
                IEnumerable<DO.Package> dalPackages = DalObject.DisplayPackagesList();
                IEnumerable<PackageToList> packageToLists = from DO.Package dalPackage in dalPackages
                                                            let senderName = DalObject.DisplayCustomer(dalPackage.SenderID).Name
                                                            let receiverName = DalObject.DisplayCustomer(dalPackage.ReceiverID).Name
                                                            select new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage));
                return packageToLists;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        public IEnumerable<PackageToList> FindPackages(Predicate<DO.Package> predicate)
        {
            try
            {
                IEnumerable<DO.Package> dalPackages = DalObject.FindPackages(predicate);
                IEnumerable<PackageToList> packageToLists = from DO.Package dalPackage in dalPackages
                                                            let senderName = DalObject.DisplayCustomer(dalPackage.SenderID).Name
                                                            let receiverName = DalObject.DisplayCustomer(dalPackage.ReceiverID).Name
                                                            select new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, Enums.PackageStatus.created);
                return packageToLists;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
    }
}
