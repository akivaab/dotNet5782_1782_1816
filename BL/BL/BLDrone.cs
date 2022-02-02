﻿using System;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    /// <summary>
    /// Drone-related functionality of the Business Layer.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        #region Add Methods
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories maxWeight, int stationID)
        {
            if (drones.Exists(d => d.ID == droneID))
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }

            try
            {
                //these statements may throw exceptions:
                DO.Station dalStation = dal.GetStation(stationID);
                if (dalStation.AvailableChargeSlots <= 0)
                {
                    throw new UnableToChargeException("There are no available charge slots for the drone in the given station.");
                }

                dal.AddDrone(droneID, model, (DO.Enums.WeightCategories)maxWeight);

                //the drone starts out charging in a station
                dal.ChargeDrone(droneID, stationID);  

                //add to drones, the list of DroneToList entities
                Location droneLocation = new(dalStation.Latitude, dalStation.Longitude);
                Random random = new Random();
                double battery = random.Next(20, 41);
                drones.Add(new DroneToList(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, droneLocation, null));
                
                Drone drone = new(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, null, droneLocation);
                return drone;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.NonUniqueIdException e)
            {
                throw new NonUniqueIdException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Update Methods
        public void UpdateDroneModel(int droneID, string model)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            try
            {
                //update in the data layer
                dal.UpdateDroneModel(droneID, model);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }

            //update in the list of DroneToLists
            drones[droneIndex].Model = model;
        }

        public void SendDroneToCharge(int droneID)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (drones[droneIndex].Status != Enums.DroneStatus.available)
            {
                throw new UnableToChargeException("The drone cannot currently be sent to charge.");
            }

            IEnumerable<DO.Station> reachableStations = getReachableStations(drones[droneIndex]);
            if (reachableStations.Count() == 0)
            {
                throw new UnableToChargeException("Either the drone does not have enough battery to reach a station, or all stations are occupied.");
            }

            try 
            { 
                Location closestStationLocation = getClosestStation(drones[droneIndex].Location, reachableStations);
                IEnumerable<DO.Station> dalStations = dal.GetStationsList();
                DO.Station dalStation = dalStations.Where(s => s.Latitude == closestStationLocation.Latitude && s.Longitude == closestStationLocation.Longitude).First();
                
                dal.ChargeDrone(droneID, dalStation.ID);

                drones[droneIndex].Battery = Math.Max(drones[droneIndex].Battery - (powerConsumption.ElementAt((int)Enums.WeightCategories.free) * getDistance(drones[droneIndex].Location, closestStationLocation)), 0);
                drones[droneIndex].Location = closestStationLocation;
                drones[droneIndex].Status = Enums.DroneStatus.maintenance;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        
        public void ReleaseFromCharge(int droneID, double chargingTimeInSeconds)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            //check drone status
            if (drones[droneIndex].Status != Enums.DroneStatus.maintenance)
            {
                throw new UnableToReleaseException("The drone is not currently charging and so cannot be released.");
            }

            try 
            {
                DO.DroneCharge dalDroneCharge = dal.FindDroneCharges(dc => dc.DroneID == droneID).Single();
                dal.ReleaseDroneFromCharging(droneID, dalDroneCharge.StationID);

                drones[droneIndex].Battery = Math.Min(drones[droneIndex].Battery + (chargeRatePerSecond * chargingTimeInSeconds), 100);
                drones[droneIndex].Status = Enums.DroneStatus.available;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Remove Methods
        public void RemoveDrone(int droneID)
        {
            try
            {
                int droneIndex = drones.FindIndex(d => d.ID == droneID);
                if (droneIndex == -1)
                {
                    throw new UndefinedObjectException("There is no drone with the given ID.");
                }

                if (drones[droneIndex].Status == Enums.DroneStatus.delivery)
                {
                    throw new UnableToRemoveException("The drone is currently delivering.");
                }

                if (drones[droneIndex].Status == Enums.DroneStatus.maintenance)
                {
                    throw new UnableToRemoveException("The drone is currently charging.");
                }

                dal.RemoveDrone(droneID);
                drones.RemoveAt(droneIndex);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }
        #endregion

        #region Getter Methods
        public Drone GetDrone(int droneID)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }
            DroneToList droneToList = drones[droneIndex];

            try
            {
                PackageInTransfer packageInTransfer;

                // if this drone is delivering a package
                if (droneToList.PackageID != null && droneToList.Status == Enums.DroneStatus.delivery)
                {
                    DO.Package dalPackage = dal.GetPackage((int)droneToList.PackageID);

                    DO.Customer sender = dal.GetCustomer(dalPackage.SenderID);
                    DO.Customer receiver = dal.GetCustomer(dalPackage.ReceiverID);
                    
                    CustomerForPackage packageSender = new(sender.ID, sender.Name);
                    CustomerForPackage packageReceiver = new(receiver.ID, receiver.Name);

                    Location currentLocation = droneToList.Location;
                    Location collectLocation = new(sender.Latitude, sender.Longitude);
                    Location deliveryLocation = new(receiver.Latitude, receiver.Longitude);
                    
                    bool status = dalPackage.Collected != null && dalPackage.Delivered == null ? true : false;

                    double distance = dalPackage.Collected == null ? getDistance(currentLocation, collectLocation) : getDistance(collectLocation, deliveryLocation);
                    
                    packageInTransfer = new(dalPackage.ID, (Enums.WeightCategories)dalPackage.Weight, (Enums.Priorities)dalPackage.Priority, status, packageSender, packageReceiver, collectLocation, deliveryLocation, distance);
                }
                else
                {
                    packageInTransfer = null;
                }

                Drone drone = new(droneToList.ID, droneToList.Model, droneToList.MaxWeight, droneToList.Battery, droneToList.Status, packageInTransfer, droneToList.Location);
                return drone;
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }

        public DateTime GetTimeChargeBegan(int droneID)
        {
            try
            {
                return dal.GetTimeChargeBegan(droneID);
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }
        }

        public IEnumerable<DroneToList> GetDronesList()
        {
            return drones.OrderBy(d => d.ID);
        }
        #endregion

        #region Find Methods
        public IEnumerable<DroneToList> FindDrones(Predicate<DroneToList> predicate)
        {
            return from DroneToList drone in drones
                   where predicate(drone)
                   select drone;
        }
        #endregion
    }
}