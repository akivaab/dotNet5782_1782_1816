using System;
using IBL.BO;


namespace IBL
{
    public interface IBL
    {
        public void AddStation(int stationID, string name, double latitude, double longitude, int numChargingSlots);

        public void AddDrone(int droneID, Enums.WeightCategories weight ,int stationID);

        public void AddCustomer(int customerID, string name, string phone);

        public void AddPackage(int senderID ,int receiverId, Enums.WeightCategories weight, Enums.Priorities priority);

        public void UpdateDroneName(int droneID, string name);

        public void UpdateStation(int stationID, string name, int numbChargingSlots);

        public void UpdateCustomer(int customerID, string name, string phone);

        public void SendDroneCharge(int droneID);

        public void ReleaseFromCharge(int droneID, DateTime chargingTime);

        public void AssignPackageDrone(int droneID);

        public void CollectPackage(int droneID);

        public void DeliveryPackage(int droneID);

        public void DisplayStation(int stationID);

        public void DisplayDrone(int droneID);

        public void DisplayCustomer(int customerID);

        public void DisplayPackage(int packageID);

        public void DisplayAllStation();

        public void DisplayAllDrones();

        public void DisplayAllCustomers();

        public void DisplayAllPackage();

        public void DisplayAllUnassignedPackages();

        public void DisplayFreeStations();
    }
}
