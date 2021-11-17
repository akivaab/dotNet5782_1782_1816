using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public Package AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            IDAL.DO.Customer sender = DalObject.DisplayCustomer(senderID);
            IDAL.DO.Customer receiver = DalObject.DisplayCustomer(receiverID);
            CustomerForPackage packageSender = new(sender.ID, sender.Name);
            CustomerForPackage packageReceiver = new(receiver.ID, receiver.Name);

            int packageID = DalObject.AddPackage(senderID, receiverID, (IDAL.DO.Enums.WeightCategories)weight, (IDAL.DO.Enums.Priorities)priority);
            Package package = new(packageID, packageSender, packageReceiver, weight, priority, null, DateTime.Now, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
            return package;
        }
        public void AssignPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            if (Drones[droneIndex].Status != Enums.DroneStatus.available)
            {
                throw new Exception();
            }
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayUnassignedPackagesList());
            int bestPackageID = findBestPackage(dalPackages, Drones[droneIndex]);
            if (bestPackageID == -1)
            {
                throw new Exception();
            }
            Drones[droneIndex].Status = Enums.DroneStatus.delivery;
            Drones[droneIndex].PackageID = bestPackageID;
            DalObject.AssignPackage(bestPackageID, droneID);
        }
        public void CollectPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            if (Drones[droneIndex].PackageID == -1)
            {
                throw new Exception();
            }
            IDAL.DO.Package dalPackage = DalObject.DisplayPackage(Drones[droneIndex].PackageID);
            if (dalPackage.Assigned == DateTime.MinValue || dalPackage.Collected != DateTime.MinValue)
            {
                throw new Exception();
            }
            Location senderLocation = getCustomerLocation(dalPackage.SenderID);
            Drones[droneIndex].Battery -= PowerConsumption[0] * getDistance(Drones[droneIndex].Location, senderLocation);
            Drones[droneIndex].Location = senderLocation;
            DalObject.CollectPackage(dalPackage.ID, droneID);
        }
        public void DeliverPackage(int droneID)
        {
            int droneIndex = Drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException();
            }
            if (Drones[droneIndex].PackageID == -1)
            {
                throw new Exception();
            }
            IDAL.DO.Package dalPackage = DalObject.DisplayPackage(Drones[droneIndex].PackageID);
            if (dalPackage.Collected == DateTime.MinValue || dalPackage.Delivered != DateTime.MinValue)
            {
                throw new Exception();
            }
            Location receiverLocation = getCustomerLocation(dalPackage.ReceiverID);
            Drones[droneIndex].Battery -= PowerConsumption[(int)dalPackage.Weight] * getDistance(Drones[droneIndex].Location, receiverLocation);
            Drones[droneIndex].Location = receiverLocation;
            Drones[droneIndex].Status = Enums.DroneStatus.available;
            DalObject.DeliverPackage(dalPackage.ID, droneID);
        }
        public Package DisplayPackage(int packageID)
        {
            IDAL.DO.Package dalPackage = DalObject.DisplayPackage(packageID);

            IDAL.DO.Customer dalPackageSender = DalObject.DisplayCustomer(dalPackage.SenderID);
            IDAL.DO.Customer dalPackageReceiver = DalObject.DisplayCustomer(dalPackage.ReceiverID);
            CustomerForPackage senderForPackage = new(dalPackageSender.ID, dalPackageSender.Name);
            CustomerForPackage receiverForPackage = new(dalPackageReceiver.ID, dalPackageReceiver.Name);

            int droneIndex = Drones.FindIndex(d => d.ID == dalPackage.DroneID);
            DroneDelivering droneDelivering = droneIndex != -1 ? new(Drones[droneIndex].ID, Drones[droneIndex].Battery, Drones[droneIndex].Location) : null;

            Package package = new(dalPackage.ID, senderForPackage, receiverForPackage, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, droneDelivering, dalPackage.Requested, dalPackage.Assigned, dalPackage.Collected, dalPackage.Delivered);
            return package;
        }
        public List<PackageToList> DisplayAllPackages()
        {
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            List<PackageToList> packageToLists = new();
            foreach (IDAL.DO.Package dalPackage in dalPackages)
            {
                string senderName = DalObject.DisplayCustomer(dalPackage.SenderID).Name;
                string receiverName = DalObject.DisplayCustomer(dalPackage.ReceiverID).Name;
                packageToLists.Add(new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, getPackageStatus(dalPackage)));
            }
            return packageToLists;
        }
        public List<PackageToList> DisplayAllUnassignedPackages()
        {
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayUnassignedPackagesList());
            List<PackageToList> packageToLists = new();
            foreach (IDAL.DO.Package dalPackage in dalPackages)
            {
                string senderName = DalObject.DisplayCustomer(dalPackage.SenderID).Name;
                string receiverName = DalObject.DisplayCustomer(dalPackage.ReceiverID).Name;
                packageToLists.Add(new PackageToList(dalPackage.ID, senderName, receiverName, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, Enums.PackageStatus.created));
            }
            return packageToLists;
        }
    }
}
