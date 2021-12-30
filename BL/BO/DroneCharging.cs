using System;

namespace BO
{
    /// <summary>
    /// DroneCharging logical entity, represents a drone currently charging in a station.
    /// </summary>
    public class DroneCharging
    {
        /// <summary>
        /// The drone ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The battery level of the drone.
        /// </summary>
        public double Battery { get; set; }
        
        /// <summary>
        /// DroneCharging constructor (for brevity in code).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="battery"></param>
        public DroneCharging(int id, double battery)
        {
            ID = id;
            Battery = battery;
        }

        /// <summary>
        /// Convert a DroneCharging entity to a string.
        /// </summary>
        /// <returns>String representation of a DroneCharging.</returns>
        public override string ToString()
        {
            return $"Drone ID: {ID}, Battery Level: {Math.Floor(Battery)}%";
        }
    }
}
