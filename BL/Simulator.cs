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
        private const double droneSpeed = 30;

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
        public Simulator(BL bl, int droneID, Action<int, IEnumerable<string>> updateDisplay, Func<bool> stop)
        {
            this.bl = bl;
            this.dal = this.bl.dal;
            this.updateDisplay = updateDisplay;
            lock (this.bl) lock (this.dal)
            {
                this.chargeStationID = bl.dal.FindDroneCharges(dc => dc.DroneID == droneID).SingleOrDefault().StationID;
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

        /// <summary>
        /// Assign a package to the drone, or send it ot charge if not possible.
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
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Charge the drone until its battery reaches 100%.
        /// </summary>
        private void charge()
        {
            lock (bl) lock (dal)
            {
                //if the drone is not yet at its designated station to charge, send it there
                if (dal.FindDroneCharges(dc => dc.DroneID == drone.ID && dc.StationID == chargeStationID).SingleOrDefault().BeganCharge == null)
                {
                    bl.sendToChargeStation(drone.ID, chargeStationID);
                    updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow"));
                    allowSimulatorCancellation = true;
                    return;
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
                else
                {
                    DateTime currentTime = DateTime.Now;
                    bl.chargeDrone(drone.ID, (currentTime - (DateTime)lastChargeTime).TotalSeconds);
                    updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow"));
                    lastChargeTime = currentTime;
                }
            }
        }

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
                    bl.CollectPackage(drone.ID);
                    updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneWindow", "PackageListWindow", "PackageWindow"));
                }
                else if (package.DeliveringTime == null)
                {
                    bl.DeliverPackage(drone.ID);
                    updateDisplay.Invoke(progressMarkers["Running Simulator"], updateWindows("DroneListWindow", "DroneWindow", "PackageListWindow", "PackageWindow", "CustomerWindow"));
                }
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
        #endregion
    }
}
