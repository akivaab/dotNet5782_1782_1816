using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BO;

namespace BL
{
    /// <summary>
    /// Drone-related functionality of the Business Layer.
    /// </summary>
    partial class BL : BlApi.IBL
    {
        #region Add Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Drone AddDrone(int droneID, string model, Enums.WeightCategories maxWeight, int stationID)
        {
            if (drones.Exists(d => d.ID == droneID))
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }

            try
            {
                lock (dal)
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

                    Location droneLocation = new(dalStation.Latitude, dalStation.Longitude);
                    Random random = new Random();
                    double battery = random.Next(20, 41);
                    drones.Add(new DroneToList(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, droneLocation, null));

                    Drone drone = new(droneID, model, maxWeight, battery, Enums.DroneStatus.maintenance, null, droneLocation);
                    return drone;
                }
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDroneModel(int droneID, string model)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            try
            {
                lock (dal)
                {
                    dal.UpdateDroneModel(droneID, model);
                }
            }
            catch (DO.UndefinedObjectException e)
            {
                throw new UndefinedObjectException(e.Message, e);
            }
            catch (DO.XMLFileLoadCreateException e)
            {
                throw new XMLFileLoadCreateException(e.Message, e);
            }

            drones[droneIndex].Model = model;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendDroneToCharge(int droneID)
        {
            int chargeStationID = selectChargeStation(droneID);
            sendToChargeStation(droneID, chargeStationID);
        }

        #region SendDroneToCharge Helper Methods
        /// <summary>
        /// Allot a charging slot in a station for the drone to charge at (and set its state to maintenance).
        /// </summary>
        /// <param name="droneID">The ID of the drone needing to be charged.</param>
        /// <returns>The ID of the alloted charging station.</returns>
        /// <exception cref="UndefinedObjectException">The drone given does not exist.</exception>
        /// <exception cref="UnableToChargeException">The drone cannot be sent to a station to charge.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        internal int selectChargeStation(int droneID)
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

            try
            {
                lock (dal)
                {
                    IEnumerable<DO.Station> reachableStations = getReachableStations(drones[droneIndex]);
                    if (reachableStations.Count() == 0)
                    {
                        throw new UnableToChargeException("Either the drone does not have enough battery to reach a station, or all stations are occupied.");
                    }

                    Location closestStationLocation = getClosestStation(drones[droneIndex].Location, reachableStations);
                    IEnumerable<DO.Station> dalStations = dal.GetStationsList();
                    DO.Station dalStation = dalStations.Where(s => s.Latitude == closestStationLocation.Latitude && s.Longitude == closestStationLocation.Longitude).First();

                    dal.AllotChargeSlot(droneID, dalStation.ID);
                    drones[droneIndex].Status = Enums.DroneStatus.maintenance;

                    return dalStation.ID;
                }
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

        /// <summary>
        /// Send the drone to the station to begin charging.
        /// </summary>
        /// <param name="droneID">The ID of the drone arriving to charge.</param>
        /// <param name="chargeStationID">The ID of the station the drone will charge at.</param>
        /// <exception cref="UndefinedObjectException">The drone given does not exist.</exception>
        /// <exception cref="UnableToChargeException">THe drone cannot start charging if it is not in the maintenance state.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        internal void sendToChargeStation(int droneID, int chargeStationID)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (drones[droneIndex].Status != Enums.DroneStatus.maintenance)
            {
                throw new UnableToChargeException("The drone cannot be sent to charge unless it is in the maintenance state.");
            }

            try
            {
                lock (dal)
                {
                    DO.Station chargeStation = dal.GetStation(chargeStationID);
                    Location chargeStationLocation = new(chargeStation.Latitude, chargeStation.Longitude);
                    dal.BeginCharge(droneID, chargeStationID);
                    drones[droneIndex].Battery = Math.Max(drones[droneIndex].Battery - (powerConsumption.ElementAt((int)Enums.WeightCategories.free) * getDistance(drones[droneIndex].Location, chargeStationLocation)), 0);
                    drones[droneIndex].Location = chargeStationLocation;
                }
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ReleaseFromCharge(int droneID, double chargingTimeInSeconds)
        {
            chargeDrone(droneID, chargingTimeInSeconds);
            releaseDrone(droneID);
        }

        #region ReleaseFromCharge Helper Methods
        /// <summary>
        /// Increase the battery level based on how long the drone was charging.
        /// </summary>
        /// <param name="droneID">The ID of the drone charging.</param>
        /// <param name="chargingTimeInSeconds">The amount of time in seconds the drone has charged.</param>
        /// <exception cref="UndefinedObjectException">The drone given does not exist.</exception>
        /// <exception cref="UnableToReleaseException">Cannot increase the battery level as the drone is not in maintenance.</exception>
        internal void chargeDrone(int droneID, double chargingTimeInSeconds)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }

            if (drones[droneIndex].Status != Enums.DroneStatus.maintenance)
            {
                throw new UnableToReleaseException("The drone is not currently charging.");
            }

            drones[droneIndex].Battery = Math.Min(drones[droneIndex].Battery + (chargeRatePerSecond * chargingTimeInSeconds), 100);
        }

        /// <summary>
        /// Release the drone from the charging station.
        /// </summary>
        /// <param name="droneID">The ID of the drone being releaseed.</param>
        /// <exception cref="UndefinedObjectException">The drone given does not exist.</exception>
        /// <exception cref="UnableToReleaseException">Cannot release the drone as it is not currently charging.</exception>
        /// <exception cref="XMLFileLoadCreateException">Failed to save/load xml.</exception>
        internal void releaseDrone(int droneID)
        {
            int droneIndex = drones.FindIndex(d => d.ID == droneID);
            if (droneIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID.");
            }
            
            if (drones[droneIndex].Status != Enums.DroneStatus.maintenance)
            {
                throw new UnableToReleaseException("The drone is not currently charging and so cannot be released.");
            }

            try 
            {
                lock (dal)
                {
                    DO.DroneCharge dalDroneCharge = dal.FindDroneCharges(dc => dc.DroneID == droneID).Single();
                    dal.ReleaseDroneFromCharging(droneID, dalDroneCharge.StationID);
                }
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
        #endregion

        #region Remove Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
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

                lock (dal)
                {
                    dal.RemoveDrone(droneID);
                }
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
        [MethodImpl(MethodImplOptions.Synchronized)]
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
                    lock (dal)
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public DateTime GetTimeChargeBegan(int droneID)
        {
            try
            {
                lock (dal)
                {
                    return dal.GetTimeChargeBegan(droneID) ?? throw new UndefinedObjectException("This drone has not yet begun charging.");
                }
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> GetDronesList()
        {
            return drones.OrderBy(d => d.ID);
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneToList> FindDrones(Predicate<DroneToList> predicate)
        {
            return from DroneToList drone in drones
                   where predicate(drone)
                   select drone;
        }
        #endregion
    }
}