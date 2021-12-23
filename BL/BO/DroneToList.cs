using System;

namespace BO
{
    /// <summary>
    /// DroneToList logical entity, represents a drone stored in a collection.
    /// </summary>
    public class DroneToList
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
        /// The drone location.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The ID of the package assigned to the drone, or null if there is none.
        /// </summary>
        public int? PackageID { get; set; }

        /// <summary>
        /// DroneToList constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The drone ID.</param>
        /// <param name="model">The drone model.</param>
        /// <param name="maxWeight">The maximum weight the drone can carry.</param>
        /// <param name="battery">The battery level of the drone.</param>
        /// <param name="status">The status of the drone (available, maintenance, delivery).</param>
        /// <param name="location">The drone location.</param>
        /// <param name="packageID">The ID of the package assigned to the drone, or null if there is none.</param>
        public DroneToList(int id, string model, Enums.WeightCategories maxWeight, double battery, Enums.DroneStatus status, Location location, int? packageID)
        {
            ID = id;
            Model = model;
            MaxWeight = maxWeight;
            Battery = battery;
            Status = status;
            Location = location;
            PackageID = packageID;
        }
        public DroneToList() { }

        /// <summary>
        /// Convert a DroneToList entity to a string.
        /// </summary>
        /// <returns>String representation of a DroneToList.</returns>
        public override string ToString()
        {
            return $"Drone ID: {ID}, Model: {Model}, Max. Weight: {MaxWeight}, Battery: {Math.Floor(Battery)}%, Status: {Status}, Location {Location}\n" +
                (PackageID != null ? $"Assigned to deliver Package {PackageID}" : "Not assigned any package");
        }
    }
}