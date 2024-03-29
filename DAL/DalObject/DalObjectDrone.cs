﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DO;

namespace DalObject
{
    /// <summary>
    /// Drone-related functionality of the Data Layer.
    /// </summary>
    partial class DalObject : DalApi.IDal
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight)
        {
            if (DataSource.drones.Exists(drone => drone.ID == id && drone.Active))
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }
            
            Drone drone = new();
            drone.ID = id;
            drone.Model = model;
            drone.MaxWeight = maxWeight;
            drone.Active = true;
            DataSource.drones.Add(drone);
        }
        #endregion

        #region Update Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ChargeDrone(int droneID, int stationID)
        {
            AllotChargeSlot(droneID, stationID);
            BeginCharge(droneID, stationID);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AllotChargeSlot(int droneID, int stationID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            int stationIndex = DataSource.stations.FindIndex(station => station.ID == stationID && station.Active);

            if (droneIndex == -1 || stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "station") + " with the given ID.");
            }

            Station station = DataSource.stations[stationIndex];
            station.AvailableChargeSlots--;
            DataSource.stations[stationIndex] = station;

            DroneCharge droneCharge = new DroneCharge();
            droneCharge.DroneID = droneID;
            droneCharge.StationID = stationID;
            droneCharge.BeganCharge = null;
            droneCharge.Active = true;
            DataSource.droneCharges.Add(droneCharge);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void BeginCharge(int droneID, int stationID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            int stationIndex = DataSource.stations.FindIndex(station => station.ID == stationID && station.Active);
            int droneChargeIndex = DataSource.droneCharges.FindIndex(dc => dc.DroneID == droneID && dc.StationID == stationID && dc.Active);

            if (droneIndex == -1 || stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "station") + " with the given ID.");
            }
            if (droneChargeIndex == -1)
            {
                throw new UndefinedObjectException("The given drone is not charging in the given station.");
            }

            DroneCharge droneCharge = DataSource.droneCharges[droneChargeIndex];
            droneCharge.BeganCharge = DateTime.Now;
            DataSource.droneCharges[droneChargeIndex] = droneCharge;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseDroneFromCharging(int droneID, int stationID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            int stationIndex = DataSource.stations.FindIndex(station => station.ID == stationID);
            int droneChargeIndex = DataSource.droneCharges.FindIndex(droneCharge => droneCharge.DroneID == droneID && droneCharge.StationID == stationID && droneCharge.Active);
            
            if (droneIndex == -1 || stationIndex == -1)
            {
                throw new UndefinedObjectException("There is no " + (droneIndex == -1 ? "drone" : "station") + " with the given ID.");
            }
            if (droneChargeIndex == -1)
            {
                throw new UndefinedObjectException("The given drone is not charging in the given station.");
            }

            Station station = DataSource.stations[stationIndex];
            station.AvailableChargeSlots++;
            DataSource.stations[stationIndex] = station;

            DroneCharge droneCharge = DataSource.droneCharges[droneChargeIndex];
            droneCharge.Active = false;
            DataSource.droneCharges[droneChargeIndex] = droneCharge;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneModel(int droneID, string model)
        {
            int droneIndex = DataSource.drones.FindIndex(d => d.ID == droneID && d.Active);

            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            Drone drone = DataSource.drones[droneIndex];
            drone.Model = model;
            DataSource.drones[droneIndex] = drone;
        }
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RemoveDrone(int droneID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID");
            }

            Drone drone = DataSource.drones[droneIndex];
            drone.Active = false;
            DataSource.drones[droneIndex] = drone;
        }
        #endregion

        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone GetDrone(int droneID)
        {
            int droneIndex = DataSource.drones.FindIndex(drone => drone.ID == droneID && drone.Active);
            
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }
            
            return DataSource.drones[droneIndex];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Drone> GetDronesList()
        {
            return from drone in DataSource.drones
                   where drone.Active
                   select drone;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<double> DronePowerConsumption()
        {
            double[] powerConsumptionValues = new double[5];
            powerConsumptionValues[0] = DataSource.Config.free;
            powerConsumptionValues[1] = DataSource.Config.lightWeight;
            powerConsumptionValues[2] = DataSource.Config.midWeight;
            powerConsumptionValues[3] = DataSource.Config.heavyWeight;
            powerConsumptionValues[4] = DataSource.Config.chargingRate;
            return powerConsumptionValues;
        }
        #endregion
    }
}
