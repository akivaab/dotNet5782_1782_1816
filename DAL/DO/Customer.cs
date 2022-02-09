
namespace DO
{
    /// <summary>
    /// Customer data entity.
    /// </summary>
    public struct Customer
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
        /// The customer's latitude coordinates.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The customer's longitude coordinates.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Represents if a customer is currently active or not.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The customer password.
        /// Defalt password is the customer ID.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Convert a Customer entity to a string.
        /// </summary>
        /// <returns>String representation of a Customer.</returns>
        public override string ToString()
        {
            return $"Customer Name: {Name}, ID: {ID}, Phone Number: {Phone}, Location: {Latitude}, {Longitude}";
        }
    }
}
