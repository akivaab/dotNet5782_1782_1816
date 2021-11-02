using System;
using System.Collections.Generic;
using IBL.BO;
using IDAL.DO;

namespace IBL
{
    public partial class BL : IBL
    {
        public List<Drone> drones;

        public BL()
        {
           
            IDal.IDal dalObject = new DalObject.DalObject();
            double[] powerConsumption = dalObject.DronePowerConsumption();
            IEnumerable<Drone> enumerable = dalObject.DisplayDronesList();
            drones = (List<Drone>)enumerable;

        }

        public void Addcustomer(int CustomeID, string Name, string Phone)
        {
            throw new NotImplementedException();
        }

        public void AddDrone(int DroneID, int stationID)
        {
            throw new NotImplementedException();
        }

        public void AddPackage(int SenderID, int ReceiverId)
        {
            throw new NotImplementedException();
        }

        public void AddStation(int StationID, string Name, double latitude, double longitude, int numChargeSlots)
        {
            throw new NotImplementedException();
        }

        public void AssignPackageDrone(int DroneID)
        {
            throw new NotImplementedException();
        }

        public void CollectPackage(int DroneID)
        {
            throw new NotImplementedException();
        }

        public void DeliveryPackage(int DroneID)
        {
            throw new NotImplementedException();
        }

        public void DisplayAllCustomers()
        {
            throw new NotImplementedException();
        }

        public void DisplayAllDrones()
        {
            throw new NotImplementedException();
        }

        public void DisplayAllPackage()
        {
            throw new NotImplementedException();
        }

        public void DisplayAllStation()
        {
            throw new NotImplementedException();
        }

        public void DisplayAllUnassignedPackages()
        {
            throw new NotImplementedException();
        }

        public void DisplayCustomer(int CustomerID)
        {
            throw new NotImplementedException();
        }

        public void DisplayDrone(int DroneID)
        {
            throw new NotImplementedException();
        }

        public void DisplayFreeStations()
        {
            throw new NotImplementedException();
        }

        public void DisplayPackage(int PackageID)
        {
            throw new NotImplementedException();
        }

        public void DisplayStation(int StationID)
        {
            throw new NotImplementedException();
        }

        public void ReleaseFromCharge(int DroneID, DateTime ChargingTime)
        {
            throw new NotImplementedException();
        }

        public void SendDroneCharge(int DroneID)
        {
            throw new NotImplementedException();
        }

        public void UpdateCustomer(int CustomerID, string Name, string phone)
        {
            throw new NotImplementedException();
        }

        public void UpdateDroneName(int DroneID, string Name)
        {
            throw new NotImplementedException();
        }

        public void UpdateStation(int StationID, string Name, int NumberChargingSlots)
        {
            throw new NotImplementedException();
        }
    }
}
