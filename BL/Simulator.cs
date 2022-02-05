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
        private const int timerDelay = 500;

        /// <summary>
        /// Speed of a drone, in kilometers per second.
        /// </summary>
        private const double droneSpeed = 30;

        /// <summary>
        /// Collection of names of the windows that need to be updated.
        /// </summary>
        private IEnumerable<string> windowsToUpdate;

        /// <summary>
        /// Simulator constructor, runs the simulation.
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="droneID"></param>
        /// <param name="updateDisplay"></param>
        /// <param name="stop"></param>
        public Simulator(BlApi.IBL bl, int droneID, Action<int, IEnumerable<string>> updateDisplay, Func<bool> stop)
        {
            while(!stop())
            {
                try
                {
                    bl.AssignPackage(droneID);
                }

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
