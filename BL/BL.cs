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
            List<IDAL.DO.Drone> dataLayerDrones = (List<IDAL.DO.Drone>)dalObject.DisplayDronesList();

            List<IDAL.DO.Package> dataLayerPackages = (List<IDAL.DO.Package>)dalObject.DisplayPackagesList();
            foreach (IDAL.DO.Package package in dataLayerPackages)
            {
                if (package.Delivered == DateTime.MinValue && package.DroneID != 0) //undelivered, drone assigned
                {
                    IDAL.DO.Drone drone = dataLayerDrones.Find(drone => drone.ID == package.DroneID);
                    dataLayerDrones.Remove(drone);
                    DroneToList droneToList = new();
                    droneToList.ID = drone.ID;
                    droneToList.Model = drone.Model;
                    droneToList.Weight = (Enums.WeightCategories)drone.MaxWeight;
                    droneToList.Status = Enums.DroneStatus.delivery;
                    if (package.Assigned != DateTime.MinValue && package.Collected == DateTime.MinValue) //assigned but not collected
                    {
                        droneToList.Location = ClosestStation(package.SenderID); //station closest to sender
                    }
                    else if (package.Collected != DateTime.MinValue)
                    {
                        droneToList.Location = LocationOf(package.SenderID); //sender location
                    }
                    droneToList.Battery = HighEnoughRandomPower();
                    droneToList.PackageID = package.ID;
                    drones.Add(droneToList);
                }
            }
            Random random = new Random();
            foreach (IDAL.DO.Drone drone in dataLayerDrones) //remaining drones not delivering
            {
                DroneToList droneToList = new();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.Weight = (Enums.WeightCategories)drone.MaxWeight;
                int randInt = random.Next(1, 3);
                if (randInt == 1)
                {
                    droneToList.Status = Enums.DroneStatus.available;
                    List<IDAL.DO.Package> dataLayerPackages = (List<IDAL.DO.Package>)dalObject.DisplayPackagesList();
                    IDAL.DO.Customer customer = RandomReceiverDeliveredPackage(dataLayerPackages);
                    double customerLatitude = customer.Latitude;
                    double customerLongitude = customer.Longitude;
                    droneToList.Location.Latitude = customerLatitude;
                    droneToList.Location.Longitude = customerLongitude;
                    droneToList.Battery = HighEnoughRandomPower();
                }
                else if (randInt == 2)
                {
                    droneToList.Status = Enums.DroneStatus.maintenance;
                    List<IDAL.DO.Station> dataLayerStations = (List<IDAL.DO.Station>)dalObject.DisplayStationsList();
                    int randStation = random.Next(dataLayerStations.Count);
                    double stationLatitude = dataLayerStations[randStation].Latitude;
                    double stationLongitude = dataLayerStations[randStation].Longitude;
                    droneToList.Location.Latitude = stationLatitude;
                    droneToList.Location.Longitude = stationLongitude;
                    droneToList.Battery = random.Next(20);
                }
                droneToList.PackageID = 0;
                drones.Add(droneToList);
            }
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
