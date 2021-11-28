using System.Collections.Generic;
using IDAL.DO;

namespace DalObject
{
    public partial class DalObject : IDAL.IDal
    {
        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight)
        {
            if (DataSource.Drones.FindIndex(drone => drone.ID == id) != -1)
            {
                throw new NonUniqueIdException();
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
                throw new UndefinedObjectException();
            }
            Station station = DataSource.Stations[stationIndex];
            station.AvailableChargeSlots--;
            DataSource.Stations[stationIndex] = station;
            DroneCharge droneCharge = new DroneCharge();
            droneCharge.DroneID = droneID;
            droneCharge.StationID = stationID;
            DataSource.DroneCharges.Add(droneCharge);
        }

        public void ReleaseDroneFromCharging(int droneID, int stationID)
        {
            int droneIndex = DataSource.Drones.FindIndex(drone => drone.ID == droneID);
            int stationIndex = DataSource.Stations.FindIndex(station => station.ID == stationID);
            int droneChargeIndex = DataSource.DroneCharges.FindIndex(droneCharge => droneCharge.DroneID == droneID);
            if (droneIndex == -1 || stationIndex == -1 || droneChargeIndex == -1)
            {
                throw new UndefinedObjectException();
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
                throw new UndefinedObjectException();
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
                throw new UndefinedObjectException();
            }
            return DataSource.Drones[droneIndex];
        }

        public IEnumerable<Drone> DisplayDronesList()
        {
            List<Drone> drones = new();
            foreach (Drone drone in DataSource.Drones)
            {
                drones.Add(drone);
            }
            return drones;
        }

        public double[] DronePowerConsumption()
        {
            double[] powerConsumptionValues = new double[5];
            powerConsumptionValues[0] = DataSource.Config.Free;
            powerConsumptionValues[1] = DataSource.Config.LightWeight;
            powerConsumptionValues[2] = DataSource.Config.MidWeight;
            powerConsumptionValues[3] = DataSource.Config.HeavyWeight;
            powerConsumptionValues[4] = DataSource.Config.ChargingRate;
            return powerConsumptionValues;
        }
    }
}
