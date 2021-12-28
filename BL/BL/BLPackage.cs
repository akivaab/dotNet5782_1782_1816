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
        #region Add Methods

        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            try
            {
                DO.Customer sender = dalObject.GetCustomer(senderID);
                DO.Customer receiver = dalObject.GetCustomer(receiverID);
                int packageID = dalObject.AddPackage(senderID, receiverID, (DO.Enums.WeightCategories)weight, (DO.Enums.Priorities)priority);

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

        #endregion

        #region Update Methods

        public void AssignPackage(int droneID)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (drones[droneIndex].Status != Enums.DroneStatus.available)
            {
                throw new UnableToAssignException("The drone is not currently available and so cannot be assigned a package.");
            }

            try
            {
                IEnumerable<DO.Package> dalPackages = dalObject.FindPackages(p => p.DroneID == null);
                int bestPackageID = findBestPackage(dalPackages, drones[droneIndex]);
                dalObject.AssignPackage(bestPackageID, droneID);

                drones[droneIndex].Status = Enums.DroneStatus.delivery;
                drones[droneIndex].PackageID = bestPackageID;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        public void CollectPackage(int droneID)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (drones[droneIndex].PackageID == null)
            {
                throw new UnableToCollectException("The drone has not been assigned a package.");
            }
            
            DO.Package dalPackage = dalObject.GetPackage((int)drones[droneIndex].PackageID);
            
            if (dalPackage.Assigned == null || dalPackage.Collected != null)
            {
                throw new UnableToCollectException("The drone is currently unable to collect a package.");
            }

            try
            {
                dalObject.CollectPackage(dalPackage.ID, droneID);

                Location senderLocation = getCustomerLocation(dalPackage.SenderID);
                drones[droneIndex].Battery = Math.Max(drones[droneIndex].Battery - (powerConsumption.ElementAt((int)Enums.WeightCategories.free) * getDistance(drones[droneIndex].Location, senderLocation)), 0);
                drones[droneIndex].Location = senderLocation;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public void DeliverPackage(int droneID)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            //if this drone isn't assigned a package
            if (drones[droneIndex].PackageID == null)
            {
                throw new UnableToDeliverException("The drone has not been assigned a package.");
            }
            
            DO.Package dalPackage = dalObject.GetPackage((int)drones[droneIndex].PackageID);
            
            //if this package isn't in a state to be delivered
            if (dalPackage.Collected == null || dalPackage.Delivered != null)
            {
                throw new UnableToDeliverException("The drone is currently unable to deliver a package.");
            }

            try
            {
                dalObject.DeliverPackage(dalPackage.ID, droneID);

                Location receiverLocation = getCustomerLocation(dalPackage.ReceiverID);
                drones[droneIndex].Battery = Math.Max(drones[droneIndex].Battery - (powerConsumption.ElementAt((int)dalPackage.Weight) * getDistance(drones[droneIndex].Location, receiverLocation)), 0);
                drones[droneIndex].Location = receiverLocation;
                drones[droneIndex].Status = Enums.DroneStatus.available;
                drones[droneIndex].PackageID = null;

                dalObject.RemovePackage(dalPackage.ID);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        #endregion

        #region Remove Methods
        public void RemovePackage(int packageID)
        {
            try
            {
                DO.Package dalPackage = dalObject.GetPackage(packageID);
                if (dalPackage.Assigned != null)
                {
                    throw new UnableToRemoveException("The package has already been assigned to a drone.");
                }

                dalObject.RemovePackage(packageID);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        #endregion

        #region Getter Methods
        public Package GetPackage(int packageID)
        {
            try
            {
                DO.Package dalPackage = dalObject.GetPackage(packageID);

                DO.Customer dalPackageSender = dalObject.GetCustomer(dalPackage.SenderID);
                DO.Customer dalPackageReceiver = dalObject.GetCustomer(dalPackage.ReceiverID);
                CustomerForPackage senderForPackage = new(dalPackageSender.ID, dalPackageSender.Name);
                CustomerForPackage receiverForPackage = new(dalPackageReceiver.ID, dalPackageReceiver.Name);

                int droneIndex = drones.FindIndex(d => d.ID == dalPackage.DroneID);
                DroneDelivering droneDelivering = droneIndex != -1 ? new(drones[droneIndex].ID, drones[droneIndex].Battery, drones[droneIndex].Location) : null;

                Package package = new(dalPackage.ID, senderForPackage, receiverForPackage, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, droneDelivering, dalPackage.Requested, dalPackage.Assigned, dalPackage.Collected, dalPackage.Delivered);
                return package;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }
        
        public IEnumerable<PackageToList> GetPackagesList()
        {
            try
            {
                IEnumerable<DO.Package> dalPackages = dalObject.GetPackagesList();
                IEnumerable<PackageToList> packageToLists = from DO.Package dalPackage in dalPackages
                                                            let senderName = dalObject.GetCustomer(dalPackage.SenderID).Name
                                                            let receiverName = dalObject.GetCustomer(dalPackage.ReceiverID).Name
                                                            select new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage));
                return packageToLists;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        #endregion

        #region Find Methods

        public IEnumerable<PackageToList> FindPackages(Predicate<DO.Package> predicate)
        {
            try
            {
                IEnumerable<DO.Package> dalPackages = dalObject.FindPackages(predicate);
                IEnumerable<PackageToList> packageToLists = from DO.Package dalPackage in dalPackages
                                                            let senderName = dalObject.GetCustomer(dalPackage.SenderID).Name
                                                            let receiverName = dalObject.GetCustomer(dalPackage.ReceiverID).Name
                                                            select new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, Enums.PackageStatus.created);
                return packageToLists;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
        }

        #endregion
    }
}
