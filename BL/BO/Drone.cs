using System;

namespace BO
{
    /// <summary>
    /// Drone logical entity.
    /// </summary>
    public class Drone
    {
        /// <summary>
        /// The drone ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The drone model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The maximum weight the drone can carry.
        /// </summary>
        public Enums.WeightCategories MaxWeight { get; set; }
        
        /// <summary>
        /// The battery level of the drone.
        /// </summary>
        public double Battery { get; set; }

        /// <summary>
        /// The status of the drone (available, maintenance, delivery).
        /// </summary>
        public Enums.DroneStatus Status { get; set; }

        /// <summary>
        /// The package the drone is currently transferring.
        /// </summary>
        public PackageInTransfer PackageInTransfer { get; set; }
        
        /// <summary>
        /// The drone location.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// Drone constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The drone ID.</param>
        /// <param name="model">The drone model.</param>
        /// <param name="maxWeight">The maximum weight the drone can carry.</param>
        /// <param name="battery">The battery level of the drone.</param>
        /// <param name="status">The status of the drone (available, maintenance, delivery).</param>
        /// <param name="packageInTransfer">The package the drone is currently transferring.</param>
        /// <param name="location">The drone location.</param>
        public Drone(int id, string model, Enums.WeightCategories maxWeight, double battery, Enums.DroneStatus status, PackageInTransfer packageInTransfer, Location location)
        {
            ID = id;
            Model = model;
            MaxWeight = maxWeight;
            Battery = battery;
            Status = status;
            PackageInTransfer = packageInTransfer;
            Location = location;
        }

        /// <summary>
        /// Convert a Drone entity to a string.
        /// </summary>
        /// <returns>String representation of a Drone.</returns>
        public override string ToString()
        {
            return $"Drone ID: {ID}, Model: {Model}, Max. Weight: {MaxWeight}, Battery: {Math.Floor(Battery)}%, Status: {Status}, Location: {Location}\n" +
                $"Currently transferring package: {PackageInTransfer}";
        }
    }
}
