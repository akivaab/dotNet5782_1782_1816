using System.Collections.Generic;

namespace BO
{
    /// <summary>
    /// Customer logical entity.
    /// </summary>
    public class Customer
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
        /// The customer's location.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// A collection of PackageForCustomer entities that the customer is sending.
        /// </summary>
        public IEnumerable<PackageForCustomer> PackagesToSend { get; set; }

        /// <summary>
        /// A collection of PackageForCustomer entities that the customer is receiving.
        /// </summary>
        public IEnumerable<PackageForCustomer> PackagesToReceive { get; set; }

        /// <summary>
        /// Customer constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The customer's ID.</param>
        /// <param name="name">The customer's name.</param>
        /// <param name="phone">The customer's phone number.</param>
        /// <param name="location">The customer's location.</param>
        /// <param name="packagesToSend">A collection of PackageForCustomer entities that the customer is sending.</param>
        /// <param name="packagesToReceive">A collection of PackageForCustomer entities that the customer is receiving.</param>
        public Customer(int id, string name, string phone, Location location, IEnumerable<PackageForCustomer> packagesToSend, IEnumerable<PackageForCustomer> packagesToReceive)
        {
            ID = id;
            Name = name;
            Phone = phone;
            Location = location;
            PackagesToSend = packagesToSend;
            PackagesToReceive = packagesToReceive;
        }

        /// <summary>
        /// Convert a Customer entity to a string.
        /// </summary>
        /// <returns>String representation of a Customer.</returns>
        public override string ToString()
        {
            return $"Customer ID: {ID}, Name: {Name}, Phone Number: {Phone}, Location: {Location}\n" +
                $"Sending Packages: {string.Join(";\t", PackagesToSend)}\n" +
                $"Receiving Packages: {string.Join(";\t", PackagesToReceive)}";
        }
    }
}