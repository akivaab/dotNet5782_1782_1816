using System;
using System.Collections.Generic;


namespace IBL
{
    public interface IBL
    {
        public void AddStation(int StationID, string Name, double latitude, double longitude, int numChargeSlots);

        //need to add weight
        public void AddDrone(int DroneID ,int stationID);

        public void Addcustomer(int CustomeID, string Name, string Phone);

        //need add the weigh/status field
        public void AddPackage(int SenderID ,int ReceiverId );

        public void UpdateDroneName(int DroneID, string Name);

        public void UpdateStation(int StationID, string Name, int NumberChargingSlots );

        public void UpdateCustomer(int CustomerID, string Name, string phone);

        public void SendDroneCharge(int DroneID);

        public void ReleaseFromCharge(int DroneID, DateTime ChargingTime);

        public void AssignPackageDrone(int DroneID);

        public void CollectPackage(int DroneID);

        public void DeliveryPackage(int DroneID);

        public void DisplayStation(int StationID);

        public void DisplayDrone(int DroneID);

        public void DisplayCustomer(int CustomerID);

        public void DisplayPackage(int PackageID);

        public void DisplayAllStation();

        public void DisplayAllDrones();

        public void DisplayAllCustomers();

        public void DisplayAllPackage();

        public void DisplayAllUnassignedPackages();

        public void DisplayFreeStations();
    }
}
