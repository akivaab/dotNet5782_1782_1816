using System;

namespace DO
{
    /// <summary>
    /// Package data entity.
    /// </summary>
    public struct Package
    {
        /// <summary>
        /// The package ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The ID of the package sender.
        /// </summary>
        public int SenderID { get; set; }

        /// <summary>
        /// The ID of the package receiver.
        /// </summary>
        public int ReceiverID { get; set; }

        /// <summary>
        /// The weight of the package.
        /// </summary>
        public Enums.WeightCategories Weight { get; set; }

        /// <summary>
        /// The priority of the package.
        /// </summary>
        public Enums.Priorities Priority { get; set; }

        /// <summary>
        /// The ID of the drone assigned to the package, or null if there is none.
        /// </summary>
        public int? DroneID { get; set; }

        /// <summary>
        /// The time the package was requested/created.
        /// </summary>
        public DateTime? Requested { get; set; }

        /// <summary>
        /// The time the package was assigned to a drone.
        /// </summary>
        public DateTime? Assigned { get; set; }

        /// <summary>
        /// The time the package was collected by a drone.
        /// </summary>
        public DateTime? Collected { get; set; }

        /// <summary>
        /// The time the package was delivered by a drone.
        /// </summary>
        public DateTime? Delivered { get; set; }

        /// <summary>
        /// Represents if a package is currently active or not.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Convert a Package entity to a string.
        /// </summary>
        /// <returns>String representation of a Package.</returns>
        public override string ToString()
        {
            return $"Package ID: {ID}, Sender ID: {SenderID}, Receiver ID: {ReceiverID}, Weight: {Weight}, Priority: {Priority}, Drone ID: {DroneID}, " +
                    $"Time requested: {Requested}, Time scheduled: {Assigned}, Time picked up: {Collected}, Time delivered: {Delivered}";
        }
    }
}
