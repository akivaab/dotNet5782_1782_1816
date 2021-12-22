using System;
using System.Collections.Generic;
using System.Linq;
using DO;

namespace DalObject
{
    partial class DalObject : DalApi.IDal
    {
        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight)
        {
            if (DataSource.Drones.FindIndex(drone => drone.ID == id) != -1)
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }
            
            Drone drone = new();
            drone.ID = id;
            drone.Model = model;
            drone.MaxWeight = maxWeight;
            DataSource.Drones.Add(drone);
        }

        public void ChargeDrone(int droneID, int stationID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int stationIndex = DataSource.Stations.FindIndex(station => station.ID == stationID);
            
            if (droneIndex == -1 || stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "station") + " with the given ID.");
            }

            Station station = DataSource.Stations[stationIndex];
            station.AvailableChargeSlots--;
            DataSource.Stations[stationIndex] = station;
            DroneCharge droneCharge = new DroneCharge();
            droneCharge.DroneID = droneID;
            droneCharge.StationID = stationID;
            droneCharge.BeganCharge = DateTime.Now;
            DataSource.DroneCharges.Add(droneCharge);
        }

        public void ReleaseDroneFromCharging(int droneID, int stationID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int stationIndex = DataSource.Stations.FindIndex(station => station.ID == stationID);
            int droneChargeIndex = DataSource.DroneCharges.FindIndex(droneCharge => droneCharge.DroneID == droneID);
            
            if (droneIndex == -1 || stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "station") + " with the given ID.");
            }
            if (droneChargeIndex == -1)
            {
                throw new UndefinedObjectException("The given drone is not charging in the given station.");
            }

            Station station = DataSource.Stations[stationIndex];
            station.AvailableChargeSlots++;
            DataSource.Stations[stationIndex] = station;
            DataSource.DroneCharges.Remove(DataSource.DroneCharges[droneChargeIndex]);
        }

        public void UpdateDroneModel(int droneID, string model)
        {
            int droneIndex = DataSource.Drones.FindIndex(d => d.ID == droneID);

            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            Drone drone = DataSource.Drones[droneIndex];
            drone.Model = model;
            DataSource.Drones[droneIndex] = drone;
        }

        public Drone DisplayDrone(int droneID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }
            
            return DataSource.Drones[droneIndex];
        }

        public IEnumerable<Drone> DisplayDronesList()
        {
            return from drone in DataSource.Drones
                   select drone;
        }

        public IEnumerable<double> DronePowerConsumption()
        {
            double[] powerConsumptionValues = new double[5];
            powerConsumptionValues[0] = DataSource.Config.Free;
            powerConsumptionValues[1] = DataSource.Config.LightWeight;
            powerConsumptionValues[2] = DataSource.Config.MidWeight;
            powerConsumptionValues[3] = DataSource.Config.HeavyWeight;
            powerConsumptionValues[4] = DataSource.Config.ChargingRate;
            return powerConsumptionValues;
        }

        public DateTime GetTimeChargeBegan(int droneID)
        {
            int droneChargeIndex = DataSource.DroneCharges.FindIndex(droneCharge => droneCharge.DroneID == droneID);
            
            if (droneChargeIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            return DataSource.DroneCharges[droneChargeIndex].BeganCharge;
        }
    }
}
