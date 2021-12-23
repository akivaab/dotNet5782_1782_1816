using System;

namespace BO
{
    /// <summary>
    /// Package logical entity.
    /// </summary>
    public class Package
    {
        /// <summary>
        /// The package ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The customer sending the package.
        /// </summary>
        public CustomerForPackage Sender { get; set; }

        /// <summary>
        /// The customer receiving the package.
        /// </summary>
        public CustomerForPackage Receiver { get; set; }

        /// <summary>
        /// The weight of the package.
        /// </summary>
        public Enums.WeightCategories Weight { get; set; }

        /// <summary>
        /// The priority of the package (regular, fast, emergency).
        /// </summary>
        public Enums.Priorities Priority { get; set; }

        /// <summary>
        /// The drone delivering the package.
        /// </summary>
        public DroneDelivering DroneDelivering { get; set; }

        /// <summary>
        /// The time the package was requested/created.
        /// </summary>
        public DateTime? RequestTime { get; set; }

        /// <summary>
        /// The time the package was assigned to a drone.
        /// </summary>
        public DateTime? AssigningTime { get; set; }

        /// <summary>
        /// The time a package was collected by a drone.
        /// </summary>
        public DateTime? CollectingTime { get; set; }

        /// <summary>
        /// The time a package was delivered by a drone.
        /// </summary>
        public DateTime? DeliveringTime { get; set; }

        /// <summary>
        /// Package constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The package ID.</param>
        /// <param name="sender">The customer sending the package.</param>
        /// <param name="receiver">The customer receiving the package.</param>
        /// <param name="weight">The weight of the package.</param>
        /// <param name="priority">The priority of the package (regular, fast, emergency).</param>
        /// <param name="droneDelivering">The drone delivering the package.</param>
        /// <param name="requestTime">The time the package was requested/created.</param>
        /// <param name="assigningTime">The time the package was assigned to a drone.</param>
        /// <param name="collectingTime">The time the package was collected by a drone.</param>
        /// <param name="deliveringTime">The time a package was delivered by a drone.</param>
        public Package(int id, CustomerForPackage sender, CustomerForPackage receiver, Enums.WeightCategories weight, Enums.Priorities priority, DroneDelivering droneDelivering, DateTime? requestTime, DateTime? assigningTime, DateTime? collectingTime, DateTime? deliveringTime)
        {
            ID = id;
            Sender = sender;
            Receiver = receiver;
            Weight = weight;
            Priority = priority;
            DroneDelivering = droneDelivering;
            RequestTime = requestTime;
            AssigningTime = assigningTime;
            CollectingTime = collectingTime;
            DeliveringTime = deliveringTime;
        }

        /// <summary>
        /// Convert a Package entity to a string.
        /// </summary>
        /// <returns>String representation of a Package.</returns>
        public override string ToString()
        {
            return $"Package ID: {ID}, Weight: {Weight}, Priority: {Priority}\n" +
                $"Send from {Sender} to {Receiver} via drone {DroneDelivering}\n" +
                $"Package requested at {RequestTime}, assigned to drone at {AssigningTime}, picked up at {CollectingTime}, delivered at {DeliveringTime}";
        }
    }
}