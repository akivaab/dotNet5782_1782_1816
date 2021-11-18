using System;
using System.Collections.Generic;
using IBL.BO;

namespace IBL
{
    public partial class BL : IBL
    {
        public List<DroneToList> Drones;
        public IDAL.IDal DalObject;
        public double[] PowerConsumption;
        public double ChargeRatePerHour;
        public BL()
        {
            DalObject = new DalObject.DalObject();
            double[] powerConsumption = DalObject.DronePowerConsumption();
            PowerConsumption = new double[4];
            Array.Copy(powerConsumption, PowerConsumption, 4);
            ChargeRatePerHour = powerConsumption[4];

            List<IDAL.DO.Drone> dalDrones = new(DalObject.DisplayDronesList());

            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            foreach (IDAL.DO.Package package in dalPackages)
            {
                if (package.Delivered == DateTime.MinValue && package.DroneID != 0) //if package is undelivered but there is a drone assigned
                {
                    IDAL.DO.Drone drone = dalDrones.Find(drone => drone.ID == package.DroneID);
                    dalDrones.Remove(drone);

                    Location droneLocation = new();
                    if (package.Assigned != DateTime.MinValue && package.Collected == DateTime.MinValue) //if package assigned but not collected
                    {
                        droneLocation = getClosestStation(getCustomerLocation(package.SenderID)); //station closest to sender
                    }
                    else if (package.Collected != DateTime.MinValue) //if package is collected
                    {
                        droneLocation = getCustomerLocation(package.SenderID); //sender location
                    }

                    double battery = randomBatteryPower(droneLocation, package, powerConsumption[(int)package.Weight]);
                    DroneToList droneToList = new(drone.ID, drone.Model, (Enums.WeightCategories)drone.MaxWeight, battery, Enums.DroneStatus.delivery, droneLocation, package.ID);
                    Drones.Add(droneToList);
                }
            }
            Random random = new Random();
            foreach (IDAL.DO.Drone drone in dalDrones) //remaining drones that are not delivering
            {
                DroneToList droneToList = new();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.MaxWeight = (Enums.WeightCategories)drone.MaxWeight;

                int randInt = random.Next(1, 3);
                if (randInt == 1)
                {
                    droneToList.Status = Enums.DroneStatus.maintenance;

                    List<IDAL.DO.Station> dataLayerStations = new(DalObject.DisplayStationsList());
                    int randStation = random.Next(dataLayerStations.Count);
                    droneToList.Location = new(dataLayerStations[randStation].Latitude, dataLayerStations[randStation].Longitude);
                    
                    droneToList.Battery = random.Next(20);
                }
                else if (randInt == 2)
                {
                    droneToList.Status = Enums.DroneStatus.available;
                    
                    IDAL.DO.Customer customer = randomPackageReceiver(dalPackages);
                    droneToList.Location = new Location(customer.Latitude, customer.Longitude);
                    
                    droneToList.Battery = random.Next((int)Math.Ceiling(powerConsumption[(int)Enums.WeightCategories.free] * getDistance(droneToList.Location, getClosestStation(droneToList.Location))), 101);
                }
                droneToList.PackageID = -1; //no package assigned
                Drones.Add(droneToList);
            }
        }
    }
}
