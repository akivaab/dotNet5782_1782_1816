
namespace BO
{
    /// <summary>
    /// DroneDelivering logical entity, represents a drone delivering a package.
    /// </summary>
    public class DroneDelivering        
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
        /// The current location of the drone.
        /// </summary>
        public Location CurrentLocation { get; set; }

        /// <summary>
        /// DroneDelivering constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The drone ID.</param>
        /// <param name="battery">The battery level of the drone.</param>
        /// <param name="currentLocation">The current location of the drone.</param>
        public DroneDelivering(int id, double battery, Location currentLocation)
        {
            ID = id;
            Battery = battery;
            CurrentLocation = currentLocation;
        }

        /// <summary>
        /// Convert a DroneDelivering entity to a string.
        /// </summary>
        /// <returns>String representation of a DroneDelivering.</returns>
        public override string ToString()
        {
            return $"Drone ID: {ID}, Battery Status: {Battery}, Current Location: {CurrentLocation}";
        }
    }
}