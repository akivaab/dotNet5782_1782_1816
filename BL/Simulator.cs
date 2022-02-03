using System;
using BO;
using System.Threading;
using static BL.BL;
using System.Linq;


namespace BL
{
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
        /// 
        /// </summary>
        /// <param name="bl"></param>
        /// <param name="droneID"></param>
        /// <param name="action"></param>
        /// <param name="stop"></param>
        public Simulator(BlApi.IBL bl, int droneID, Action action, Func<bool> stop)
        {
            while(!stop())
            {

            }
        }
    }
}
