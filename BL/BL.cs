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
            //initialize fields
            Drones = new();
            DalObject = new DalObject.DalObject();
            double[] powerConsumption = DalObject.DronePowerConsumption();
            PowerConsumption = new double[4];
            Array.Copy(powerConsumption, PowerConsumption, 4);
            ChargeRatePerHour = powerConsumption[4];

            //remove problematic entities from the data layer
            dataCleanup();
            
            List<IDAL.DO.Drone> dalDrones = new(DalObject.DisplayDronesList());
            List<IDAL.DO.Package> dalPackages = new(DalObject.DisplayPackagesList());
            
            foreach (IDAL.DO.Package package in dalPackages)
            {
                //if package is undelivered but there is a drone assigned
                if (package.Delivered == null && package.DroneID != null) 
                {
                    IDAL.DO.Drone drone = dalDrones.Find(drone => drone.ID == package.DroneID);
                    dalDrones.Remove(drone);

                    Location droneLocation = new();

                    //if package assigned but not collected
                    if (package.Assigned != null && package.Collected == null) 
                    {
                        //set station closest to sender as drone location
                        droneLocation = getClosestStation(getCustomerLocation(package.SenderID)); 
                    }
                    //if package is collected
                    else if (package.Collected != null) 
                    {
                        //set sender location as drone location
                        droneLocation = getCustomerLocation(package.SenderID);
                    }

                    //get random battery level
                    double battery = randomBatteryPower(droneLocation, package, powerConsumption[(int)package.Weight]);
                    
                    DroneToList droneToList = new(drone.ID, drone.Model, (Enums.WeightCategories)drone.MaxWeight, battery, Enums.DroneStatus.delivery, droneLocation, package.ID);
                    Drones.Add(droneToList);
                }
            }

            Random random = new Random();

            //for remaining drones that are not delivering
            foreach (IDAL.DO.Drone drone in dalDrones) 
            {
                DroneToList droneToList = new();
                droneToList.ID = drone.ID;
                droneToList.Model = drone.Model;
                droneToList.MaxWeight = (Enums.WeightCategories)drone.MaxWeight;

                //randomly choose state of drone
                int randInt = random.Next(1, 3);
                if (randInt == 1)
                {
                    droneToList.Status = Enums.DroneStatus.maintenance;

                    //get random station as drone location
                    List<IDAL.DO.Station> dalStations = new(DalObject.DisplayStationsList());
                    int randStation = random.Next(dalStations.Count);
                    droneToList.Location = new(dalStations[randStation].Latitude, dalStations[randStation].Longitude);
                    
                    //get random battery level 0%-20%
                    droneToList.Battery = random.Next(21);

                    //create appropriate DroneCharge entity in data layer
                    DalObject.ChargeDrone(drone.ID, dalStations[randStation].ID);
                }
                else if (randInt == 2)
                {
                    droneToList.Status = Enums.DroneStatus.available;
                    
                    //get random customer that received a package as drone location
                    IDAL.DO.Customer customer = randomPackageReceiver(dalPackages);
                    droneToList.Location = new Location(customer.Latitude, customer.Longitude);
                    
                    //get random battery level
                    droneToList.Battery = random.Next((int)Math.Ceiling(powerConsumption[(int)Enums.WeightCategories.free] * getDistance(droneToList.Location, getClosestStation(droneToList.Location))), 101);
                }

                //no package assigned
                droneToList.PackageID = null; 

                Drones.Add(droneToList);
            }
        }
    }
}
