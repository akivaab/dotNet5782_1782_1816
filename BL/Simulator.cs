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
        /// <summary>
        /// Delay between checks for the drone to perform actions.
        /// </summary>
        private const int delay = 500;

        /// <summary>
        /// Speed of a drone, in kilometers per second.
        /// </summary>
        private const double droneSpeed = 30;

        /// <summary>
        /// The drone whose duties are being simulated.
        /// </summary>
        private Drone drone;

        /// <summary>
        /// Instance of the BL layer.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// Collection of names of the windows that need to be updated.
        /// </summary>
        private IEnumerable<string> windowsToUpdate;

        /// <summary>
        /// An action which updates the windows displayed in the application when invoked.
        /// </summary>
        private Action<int, IEnumerable<string>> updateDisplay;

        /// <summary>
        /// Value marking whether to end the simulation.
        /// </summary>
        private bool endSimulation = false;

        /// <summary>
        /// Simulator constructor, runs the simulation.
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="droneID"></param>
        /// <param name="updateDisplay"></param>
        /// <param name="stop"></param>
        public Simulator(BlApi.IBL bl, int droneID, Action<int, IEnumerable<string>> updateDisplay, Func<bool> stop)
        {
            this.bl = bl;
            this.updateDisplay = updateDisplay;
            while(!(stop() || endSimulation))
            {
                try
                {
                    drone = bl.GetDrone(droneID);
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
                    }
                }
                catch (UndefinedObjectException)
                {

                }
                catch (XMLFileLoadCreateException)
                {

                }
            }
        }

        private void assign()
        {
            try
            {
                bl.AssignPackage(drone.ID);
            }
            catch (EmptyListException e)
            {
                if (e.Rectifiable)
                {
                    bl.SendDroneToCharge(drone.ID);
                }
                else
                {
                    endSimulation = true;
                }
            }
            updateDisplay.Invoke();
        }

        private void charge()
        {
            try
            {
                if (drone.Battery == 100)
                {
                    DateTime beganCharging = bl.GetTimeChargeBegan(drone.ID);
                    bl.ReleaseFromCharge(drone.ID, (DateTime.Now - beganCharging).TotalSeconds);
                }
            }
            catch
            {

            }
        }

        private void deliver()
        {
            try
            {
                Package package = bl.GetPackage(drone.PackageInTransfer.ID);

                if (package.CollectingTime == null)
                {
                    bl.CollectPackage(drone.ID);
                }
                else if (package.DeliveringTime == null)
                {
                    bl.DeliverPackage(drone.ID);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="windows"></param>
        private void markWindowsToUpdate(params string[] windows)
        {
            windowsToUpdate = windows;
        }
    }
}
