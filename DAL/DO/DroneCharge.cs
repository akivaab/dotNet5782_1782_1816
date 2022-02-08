using System;

namespace DO
{
    /// <summary>
    /// DroneCharge data entity, represents a drone and the station it is currently charging in.
    /// </summary>
    public struct DroneCharge
    {
        /// <summary>
        /// The drone ID.
        /// </summary>
        public int DroneID { get; set; }

        /// <summary>
        /// The station ID.
        /// </summary>
        public int StationID { get; set; }

        /// <summary>
        /// Time a drone began charging in a station (null if it hasn't).
        /// </summary>
        public DateTime? BeganCharge { get; set; }

        /// <summary>
        /// Represents if a DroneCharge is currently active or not.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Convert a DroneCharge entity to a string.
        /// </summary>
        /// <returns>String representation of a DroneCharge.</returns>
        public override string ToString()
        {
            return $"Drone ID: {DroneID}, Station ID: {StationID}";
        }
    }
}
