using System;
using IBL.BO;


namespace IBL
{
    public interface IBL
    {
        public Station AddStation(int stationID, int name, Location location, int numAvailableChargingSlots);

        public Drone AddDrone(int droneID, string model, Enums.WeightCategories weight, int stationID);

        public Customer AddCustomer(int customerID, string name, string phone, Location location);

        public Package AddPackage(int senderID ,int receiverID, Enums.WeightCategories weight, Enums.Priorities priority);

        public void UpdateDroneModel(int droneID, string model);

        public void UpdateStation(int stationID, int name = -1, int numChargingSlots = -1);

        public void UpdateCustomer(int customerID, string name = "", string phone= "");

        public void SendDroneToCharge(int droneID);

        public void ReleaseFromCharge(int droneID, DateTime chargingTime);

        public void AssignPackage(int droneID);

        public void CollectPackage(int droneID);

        public void DeliverPackage(int droneID);

        public void DisplayStation(int stationID);

        public void DisplayDrone(int droneID);

        public void DisplayCustomer(int customerID);

        public void DisplayPackage(int packageID);

        public void DisplayAllStations();

        public void DisplayAllDrones();

        public void DisplayAllCustomers();

        public void DisplayAllPackages();

        public void DisplayAllUnassignedPackages();

        public void DisplayFreeStations();
    }
}
