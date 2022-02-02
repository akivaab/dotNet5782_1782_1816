using System;

namespace BO
{
    /// <summary>
    /// PackageInTransfer logical entity, represent a package currently mid-transfer.
    /// </summary>
    public class PackageInTransfer
    {
        /// <summary>
        /// The package ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The weight of the package.
        /// </summary>
        public Enums.WeightCategories Weight { get; set; }

        /// <summary>
        /// The priority of the package (regular, fast, emergency).
        /// </summary>
        public Enums.Priorities Priority { get; set; }

        /// <summary>
        /// Status of whether the package is currently being transferred or not.
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// The sender of the package.
        /// </summary>
        public CustomerForPackage Sender { get; set; }

        /// <summary>
        /// The receiver of the package.
        /// </summary>
        public CustomerForPackage Receiver { get; set; }

        /// <summary>
        /// The location from which the package is being collected by a drone.
        /// </summary>
        public Location CollectLocation { get; set; }

        /// <summary>
        /// The location the package is being delivered to by a drone.
        /// </summary>
        public Location DeliveryLocation { get; set; }

        /// <summary>
        /// The travel distance required to deliver the package.
        /// </summary>
        public double DeliveryDistance { get; set; }

        /// <summary>
        /// PackageInTransfer constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The package ID.</param>
        /// <param name="weight">The weight of the package.</param>
        /// <param name="priority">The priority of the package (regular, fast, emergency).</param>
        /// <param name="status">Status of whether the package is currently being transferred or not.</param>
        /// <param name="sender">The sender of the package.</param>
        /// <param name="receiver">The receiver of the package.</param>
        /// <param name="collectLocation">The location from which the package is being collected by a drone.</param>
        /// <param name="deliveryLocation">The location the package is being delivered to by a drone.</param>
        /// <param name="deliveryDistance">The travel distance required to deliver the package.</param>
        public PackageInTransfer(int id, Enums.WeightCategories weight, Enums.Priorities priority, bool status, CustomerForPackage sender, CustomerForPackage receiver, Location collectLocation, Location deliveryLocation, double deliveryDistance)
        {
            ID = id;
            Weight = weight;
            Priority = priority;
            Status = status;
            Sender = sender;
            Receiver = receiver;
            CollectLocation = collectLocation;
            DeliveryLocation = deliveryLocation;
            DeliveryDistance = deliveryDistance;
        }

        /// <summary>
        /// Convert a PackageInTransfer entity to a string.
        /// </summary>
        /// <returns>String representation of a PackageInTransfer.</returns>
        public override string ToString()
        {
            return $"Package ID: {ID}, Weight: {Weight}, Priority: {Priority}\n" +
                $"Package in transfer: {Status}, Distance to deliver: {Math.Round(DeliveryDistance, 2)} km\n" +
                $"  Collect from {Sender} at {CollectLocation}\n" + 
                $"  Deliver to {Receiver} at {DeliveryLocation}";
        }
    }
}
