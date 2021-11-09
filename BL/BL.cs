using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public List<DroneToList> drones;

        public BL()
        {
            IDAL.IDal dalObject = new DalObject.DalObject();
            double[] powerConsumption = dalObject.DronePowerConsumption();
            IEnumerable<IDAL.DO.Drone> dataLayerDrones = dalObject.DisplayDronesList();
            //IEnumerable<IDAL.DO.Package> dataLayerPackages = dalObject.DisplayPackagesList();
            

        }

        public void AddCustomer(int customerID, string name, string phone)
        {
            throw new NotImplementedException();
        }

        public void AddDrone(int droneID, Enums.WeightCategories weight, int stationID)
        {
            throw new NotImplementedException();
        }

        public void AddPackage(int senderID, int receiverId, Enums.WeightCategories weight, Enums.Priorities priority)
        {
            throw new NotImplementedException();
        }

        public void AddStation(int stationID, string name, double latitude, double longitude, int numChargeSlots)
        {
            throw new NotImplementedException();
        }

        public void AssignPackageDrone(int droneID)
        {
            throw new NotImplementedException();
        }

        public void CollectPackage(int droneID)
        {
            throw new NotImplementedException();
        }

        public void DeliveryPackage(int droneID)
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

        public void DisplayCustomer(int customerID)
        {
            throw new NotImplementedException();
        }

        public void DisplayDrone(int droneID)
        {
            throw new NotImplementedException();
        }

        public void DisplayFreeStations()
        {
            throw new NotImplementedException();
        }

        public void DisplayPackage(int packageID)
        {
            throw new NotImplementedException();
        }

        public void DisplayStation(int stationID)
        {
            throw new NotImplementedException();
        }

        public void ReleaseFromCharge(int droneID, DateTime chargingTime)
        {
            throw new NotImplementedException();
        }

        public void SendDroneCharge(int droneID)
        {
            throw new NotImplementedException();
        }

        public void UpdateCustomer(int customerID, string name, string phone)
        {
            throw new NotImplementedException();
        }

        public void UpdateDroneName(int droneID, string name)
        {
            throw new NotImplementedException();
        }

        public void UpdateStation(int stationID, string name, int numChargingSlots)
        {
            throw new NotImplementedException();
        }
    }
}
