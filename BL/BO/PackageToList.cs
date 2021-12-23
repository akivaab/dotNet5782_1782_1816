
namespace BO
{
    /// <summary>
    /// PackageToList logical entity, represents a package stored in a collection.
    /// </summary>
    public class PackageToList
    {
        /// <summary>
        /// The package ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the package sender.
        /// </summary>
        public string SenderName { get; set; }

        /// <summary>
        /// The name of the package receiver.
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// The weight of the package.
        /// </summary>
        public Enums.WeightCategories Weight { get; set; }

        /// <summary>
        /// The priority of the package (regular, fast, emergency).
        /// </summary>
        public Enums.Priorities Priority { get; set; }

        /// <summary>
        /// The status of the package (created, assigned, collected, delivered).
        /// </summary>
        public Enums.PackageStatus Status { get; set; }

        /// <summary>
        /// PackageToList constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The package ID.</param>
        /// <param name="senderName">The name of the package sender.</param>
        /// <param name="receiverName">The name of the package receiver.</param>
        /// <param name="weight">The weight of the package.</param>
        /// <param name="priority">The priority of the package (regular, fast, emergency).</param>
        /// <param name="status">The status of the package (created, assigned, collected, delivered).</param>
        public PackageToList(int id, string senderName, string receiverName, Enums.WeightCategories weight, Enums.Priorities priority, Enums.PackageStatus status)
        {
            ID = id;
            SenderName = senderName;
            ReceiverName = receiverName;
            Weight = weight;
            Priority = priority;
            Status = status;
        }

        /// <summary>
        /// Convert a PackageToList entity to a string.
        /// </summary>
        /// <returns>String representation of a PackageToList.</returns>
        public override string ToString()
        {
            return $"Package ID: {ID}, Weight: {Weight}, Priority: {Priority}, Status: {Status}\n" +
                $"Send from {SenderName} to {ReceiverName}";
        }
    }
}
