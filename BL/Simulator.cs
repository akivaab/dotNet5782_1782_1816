using System;
using BO;
using System.Threading;
using static BL.BL;
using System.Linq;
using System.Collections.Generic;

namespace BL
{
    /// <summary>
    /// Simulator of a drone performing its duties.
    /// </summary>
    class Simulator
    {
        #region Fields
        /// <summary>
        /// Delay between checks for the drone to perform actions.
        /// </summary>
        private const int delay = 500;

        /// <summary>
        /// Speed of a drone, in kilometers per second.
        /// </summary>
        private const double droneSpeed = 50;

        /// <summary>
        /// Distance in kilometers a drone travels in the time given by the delay step timer.
        /// </summary>
        private const double distancePerIncrement = droneSpeed * ((double)delay / 1000);

        /// <summary>
        /// System used to report simulator progress.
        /// </summary>
        private Dictionary<string, int> progressMarkers = new()
        {
            { "Running Simulator", 1 },
            { "Cannot Charge", 2 },
            { "Completed Deliveries", 3 }
        };

        /// <summary>
        /// Value marking whether or not the simulation can be cancelled.
        /// </summary>
        private bool allowSimulatorCancellation = true;

        /// <summary>
        /// The drone whose duties are being simulated.
        /// </summary>
        private Drone drone;

        /// <summary>
        /// Instance of the BL layer.
        /// </summary>
        private BL bl;

        /// <summary>
        /// Instance of the DAL layer.
        /// </summary>
        private DalApi.IDal dal;

        /// <summary>
        /// An action which updates the windows displayed in the application when invoked.
        /// </summary>
        private Action<int, IEnumerable<string>> updateDisplay;

        /// <summary>
        /// The ID of the station the drone is currently charging at (or 0 if not charging).
        /// </summary>
        private int chargeStationID;

        /// <summary>
        /// The last time the simulator checked the drone while it was in maintenance.
        /// </summary>
        private DateTime? lastChargeTime;
        #endregion

        #region Constructor (and Helper Methods)
        /// <summary>
        /// Simulator constructor, runs the simulation.
        /// </summary>
        /// <param name="bl">Instance of the BL layer.</param>
        /// <param name="droneID">The ID of the drone being simulated.</param>
        /// <param name="updateDisplay">Action which calls a function that updates the application windows displayed.</param>
        /// <param name="stop">Function that determines when to stop the simulator.</param>
        /// <exception cref="UndefinedObjectException">More than one droneCharge was found with the drone.</exception>
        public Simulator(BL bl, int droneID, Action<int, IEnumerable<string>> updateDisplay, Func<bool> stop)
        {
            this.bl = bl;
            this.dal = this.bl.dal;
            this.updateDisplay = updateDisplay;
            try
            {
                lock (this.bl) lock (this.dal)
                {
                    this.chargeStationID = this.bl.dal.FindDroneCharges(dc => dc.DroneID == droneID).SingleOrDefault().StationID;
                }
            }
            catch (Exception)
            {
                throw new UndefinedObjectException("There are multiple active droneCharges with this drone.");
            }

            while (!(stop() && allowSimulatorCancellation))
            {
                lock (this.bl)
                {
                    drone = this.bl.GetDrone(droneID);
                }
                Thread.Sleep(delay);
                switch (drone.Status)
                {
                    case Enums.DroneStatus.available:
                        assign();
                        break;
                    case Enums.DroneStatus.maintenance:
                        charge();
                        break;
                    case Enums.DroneStatus.delivery:
                        deliver();
                        break;
                    default:
                        break;
                }
            }
        }

        #region From Available Status
        /// <summary>
        /// Assign a package to the drone, or send it to charge if not possible.
        /// </summary>
        private void assign()
        {
            lock (bl)
            {
                try
                {
                    bl.AssignPackage(drone.ID);
                    updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow", "PackageListWindow", "PackageWindow"));
                }
                //no packages to deliver
                catch (EmptyListException)
                {
                    if (drone.Battery != 100)
                    {
                        try
                        {
                            chargeStationID = bl.selectChargeStation(drone.ID);
                            updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow", "StationListWindow", "StationWindow"));
                            allowSimulatorCancellation = false;
                        }
                        //cannot charge at any reachable station
                        catch (UnableToChargeException)
                        {
                            updateDisplay.Invoke(progressMarkers["Cannot Charge"], updateWindows("DroneWindow"));
                            return;
                        }
                    }
                    else
                    {
                        //must wait for creation of viable package
                        updateDisplay.Invoke(progressMarkers["Completed Deliveries"], updateWindows("DroneWindow"));
                    }
                }
            }
        }
        #endregion

        #region From Maintenance Status
        /// <summary>
        /// Send the drone to charge and charge it until its battery reaches 100%.
        /// </summary>
        /// <exception cref="UndefinedObjectException">More than one droneCharge has thos drone-station pair.</exception>
        private void charge()
        {
            lock (bl) lock (dal)
            {
                try
                {
                    //if the drone is not yet at its designated station to charge, send it there  
                    if (dal.FindDroneCharges(dc => dc.DroneID == drone.ID && dc.StationID == chargeStationID).SingleOrDefault().BeganCharge == null)
                        {
                            Location chargeStationLocation = bl.GetStation(chargeStationID).Location;
                            
                            //send the drone the distance allowed by the step timer
                            if (longDistanceTo(chargeStationLocation))
                            {
                                double battery = drone.Battery - (dal.DronePowerConsumption().ElementAt((int)Enums.WeightCategories.free) * distancePerIncrement);
                                Location location = calculateMidwayLocation(drone.Location, chargeStationLocation, distancePerIncrement);
                                bl.updateViaSimulator(drone.ID, battery, location);
                            }
                            //the drone can reach the destination within the time of the step timer
                            else
                            {
                                bl.sendToChargeStation(drone.ID, chargeStationID);
                                allowSimulatorCancellation = true;
                            }
                            updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow"));
                            return;
                        }
                    }
                catch (Exception)
                {
                    throw new UndefinedObjectException("There are multiple active droneCharges with this drone and station pairing.");
                }
            }
            lock (bl)
            {
                if (lastChargeTime == null)
                {
                    lastChargeTime = bl.GetTimeChargeBegan(drone.ID);
                }

                if (drone.Battery == 100)
                {
                    bl.releaseDrone(drone.ID);
                    updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow", "StationListWindow", "StationWindow"));
                    chargeStationID = 0;
                    lastChargeTime = null;
                }
                //charge the drone incrementally based on the delay step timer
                else
                {
                    DateTime currentTime = DateTime.Now;
                    bl.chargeDrone(drone.ID, (currentTime - (DateTime)lastChargeTime).TotalSeconds);
                    updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow", "StationWindow"));
                    lastChargeTime = currentTime;
                }
            }
        }
        #endregion

        #region From Delivery Status
        /// <summary>
        /// Have the drone collect or deliver its package.
        /// </summary>
        private void deliver()
        {
            lock (bl)
            {
                Package package = bl.GetPackage(drone.PackageInTransfer.ID);

                if (package.CollectingTime == null)
                {
                    Location senderLocation = bl.GetCustomer(package.Sender.ID).Location;

                    //send the drone the distance allowed by the step timer
                    if (longDistanceTo(senderLocation))
                    {
                        updateIncrement(package, senderLocation);
                    }
                    //the drone can reach the destination within the time of the step timer
                    else
                    {
                        bl.CollectPackage(drone.ID);
                        updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow", "PackageListWindow", "PackageWindow"));
                        allowSimulatorCancellation = true;
                    }
                }
                else if (package.DeliveringTime == null)
                {
                    Location receiverLocation = bl.GetCustomer(package.Receiver.ID).Location;

                    //send the drone the distance allowed by the step timer
                    if (longDistanceTo(receiverLocation))
                    {
                        updateIncrement(package, receiverLocation);
                    }
                    //the drone can reach the destination within the time of the step timer
                    else
                    {
                        bl.DeliverPackage(drone.ID);
                        updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow", "PackageListWindow", "PackageWindow", "CustomerWindow"));
                        allowSimulatorCancellation = true;
                    }
                }
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Determines if the distance to some location is greater than the drone can travel in one step of the delay timer.
        /// </summary>
        /// <param name="destinationLocation">The destination of the drone.</param>
        /// <returns>True if the incremental distance based on the delay step timer is less than the distance to the destination, false otherwise.</returns>
        private bool longDistanceTo(Location destinationLocation)
        {
            return distancePerIncrement < getDistance(drone.Location, destinationLocation);
        }

        /// <summary>
        /// Update the drone based on the incremental changes during a transaction.
        /// </summary>
        /// <param name="package">The package the transaction is taking place upon.</param>
        /// <param name="customerLocation">Location of the relevant customer in the transaction.</param>
        private void updateIncrement(Package package, Location customerLocation)
        {
            lock (bl) lock (dal)
            {
                double battery = drone.Battery - (dal.DronePowerConsumption().ElementAt((int)package.Weight) * distancePerIncrement);
                Location location = calculateMidwayLocation(drone.Location, customerLocation, distancePerIncrement);
                bl.updateViaSimulator(drone.ID, battery, location);
                updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow"));
                allowSimulatorCancellation = false;
            }
        }

        /// <summary>
        /// Creates a collection of the names of windows that need to be updated.
        /// </summary>
        /// <param name="windowNames">Names of the windows to be updated.</param>
        /// <returns>Collection of the names.</returns>
        private IEnumerable<string> updateWindows(params string[] windowNames)
        {
            return windowNames;
        }

        #region Calculate Coordinates
        /// <summary>
        /// Find the coordinate location of some point between two coordinates.
        /// </summary>
        /// <param name="source">The source location.</param>
        /// <param name="destination">The destination location.</param>
        /// <param name="distance">The distance away from "source" in the direction of "destination".</param>
        /// <returns>The coordinates of the point "distance" away from "source" in the directiob of "destination".</returns>
        private static Location calculateMidwayLocation(Location source, Location destination, double distance)
        {
            double bearing = calculateBearing(source, destination);
            return getMidwayLocation(source, bearing, distance);
        }

        /// <summary>
        /// Find the coordinates of a point a certain distance away from a given location.
        /// </summary>
        /// <param name="location">The starting location.</param>
        /// <param name="azimuth">The bearing/direction away from the starting location.</param>
        /// <param name="distance">The distance from the starting location.</param>
        /// <returns>The coordinates of a point "distance" away from "location" in the direction of "azimuth".</returns>
        private static Location getMidwayLocation(Location location, double azimuth, double distance)
        {
            double radius = 6378.1370;
            double bearing = toRadians(azimuth);
            double lat1 = toRadians(location.Latitude);
            double lon1 = toRadians(location.Longitude);
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distance / radius) + Math.Cos(lat1) * Math.Sin(distance / radius) * Math.Cos(bearing));
            double lon2 = lon1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance / radius) * Math.Cos(lat1), Math.Cos(distance / radius) - Math.Sin(lat1) * Math.Sin(lat2));
            lat2 = toDegrees(lat2);
            lon2 = toDegrees(lon2);
            return new Location(lat2, lon2);
        }

        /// <summary>
        /// Calculate the bearing between two location.
        /// </summary>
        /// <param name="source">The source location.</param>
        /// <param name="destination">The destination lovation.</param>
        /// <returns>The bearing/azimuth from "source" to "destination".</returns>
        private static double calculateBearing(Location source, Location destination)
        {
            double sourceLat = toRadians(source.Latitude);
            double sourceLong = toRadians(source.Longitude);
            double destLat = toRadians(destination.Latitude);
            double destLong = toRadians(destination.Longitude);
            double dLong = destLong - sourceLong;
            double dPhi = Math.Log(Math.Tan((destLat / 2.0) + (Math.PI / 4.0)) / Math.Tan((sourceLat / 2.0) + (Math.PI / 4.0)));
            if (Math.Abs(dLong) > Math.PI)
            {
                if (dLong > 0.0)
                {
                    dLong = -(2.0 * Math.PI - dLong);
                }
                else
                {
                    dLong = 2.0 * Math.PI + dLong;
                }
            }
            double bearing = (toDegrees(Math.Atan2(dLong, dPhi)) + 360.0) % 360.0;
            return bearing;
        }

        /// <summary>
        /// Convert a value in radians to degrees.
        /// </summary>
        /// <param name="radians">The value in radians to convert.</param>
        /// <returns>The equivalent value in degrees.</returns>
        private static double toDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }
        #endregion
        #endregion
        #endregion
    }
}
