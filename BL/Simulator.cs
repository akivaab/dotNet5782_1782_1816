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
        /// Simulator constructor, runs the simulation.
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="droneID"></param>
        /// <param name="action"></param>
        /// <param name="stop"></param>
        public Simulator(BlApi.IBL bl, int droneID, Action<int, IEnumerable<Enum>> action, Func<bool> stop)
        {
            while(!stop())
            {
                
            }
        }

        /// <summary>
        /// Indicators for which windows need to be updated as per the simulation.
        /// </summary>
        public enum UpdateWindows
        {
            droneList, drone, packageList, package, stationList, station, customerList, customer
        }
    }
}
