
namespace BO
{
    /// <summary>
    /// CustomerToList logical entity, represents a customer stored in a collection.
    /// </summary>
    public class CustomerToList
    {
        /// <summary>
        /// The customer's ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The customer's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The customer's phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// The number of packages sent by the customer that have already been delivered.
        /// </summary>
        public int NumDeliveredPackagesSent { get; set; }

        /// <summary>
        /// The number of packages sent by the customer that have not yet been delivered.
        /// </summary>
        public int NumUndeliveredPackagesSent { get; set; }

        /// <summary>
        /// The number of packages the customer already received.
        /// </summary>
        public int NumPackagesReceived { get; set; }

        /// <summary>
        /// The number of packages the customer can expect to receive.
        /// </summary>
        public int NumPackagesExpected { get; set; }

        /// <summary>
        /// CustomerToList constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The customer's ID.</param>
        /// <param name="name">The customer's name.</param>
        /// <param name="phone">The customer's phone number.</param>
        /// <param name="numDeliveredPackagesSent">The number of packages sent by the customer that have already been delivered.</param>
        /// <param name="numUndeliveredPackagesSent">The number of packages sent by the customer that have not yet been delivered.</param>
        /// <param name="numPackagesReceived">The number of packages the customer already received.</param>
        /// <param name="numPackagesExpected">The number of packages the customer can expect to receive.</param>
        public CustomerToList(int id, string name, string phone, int numDeliveredPackagesSent, int numUndeliveredPackagesSent, int numPackagesReceived, int numPackagesExpected)
        {
            ID = id;
            Name = name;
            Phone = phone;
            NumDeliveredPackagesSent = numDeliveredPackagesSent;
            NumUndeliveredPackagesSent = numUndeliveredPackagesSent;
            NumPackagesReceived = numPackagesReceived;
            NumPackagesExpected = numPackagesExpected;
        }

        /// <summary>
        /// Convert a CustomerToList entity to a string.
        /// </summary>
        /// <returns>String representation of a CustomerToList.</returns>
        public override string ToString()
        {
            return $"Customer ID: {ID}, Name: {Name}, Phone Number: {Phone}\n" +
                $"Number of packages sent that were delivered: {NumDeliveredPackagesSent}\n" +
                $"Number of packages sent not yet delivered: {NumUndeliveredPackagesSent}\n" +
                $"Number of packages received: {NumPackagesReceived}\n" +
                $"Number of packages expecting to receive: {NumPackagesExpected}";
        }
    }
}