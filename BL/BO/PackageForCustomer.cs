
namespace BO
{
    /// <summary>
    /// PackageForCustomer logical entity, represents a package being transacted.
    /// </summary>
    public class PackageForCustomer
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
        /// The status of the package (created, assigned, collected, delivered).
        /// </summary>
        public Enums.PackageStatus Status { get; set; }

        /// <summary>
        /// The other party involved in the transaction of the package.
        /// </summary>
        public CustomerForPackage OtherParty { get; set; }

        /// <summary>
        /// PackageForCustomer constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The package ID.</param>
        /// <param name="weight">The weight of the package.</param>
        /// <param name="priority">The priority of the package (regular, fast, emergency).</param>
        /// <param name="status">The status of the package (created, assigned, collected, delivered).</param>
        /// <param name="otherParty">The other party involved in the transaction of the package.</param>
        public PackageForCustomer(int id, Enums.WeightCategories weight, Enums.Priorities priority, Enums.PackageStatus status, CustomerForPackage otherParty)
        {
            ID = id;
            Weight = weight;
            Priority = priority;
            Status = status;
            OtherParty = otherParty;
        }

        /// <summary>
        /// Convert a PackageForCustomer entity to a string.
        /// </summary>
        /// <returns>String representation of a PackageForCustomer.</returns>
        public override string ToString()
        {
            return $"Package ID: {ID}, Weight: {Weight}, Priority: {Priority}, Status: {Status}. Transaction with {OtherParty}.";
        }
    }
}
