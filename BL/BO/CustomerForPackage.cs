
namespace BO
{
    /// <summary>
    /// CustomerForPackage logical entity, represents a customer who is sending/receiving a package.
    /// </summary>
    public class CustomerForPackage
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
        /// CustomerForPackage constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The customer's ID.</param>
        /// <param name="name">The customer's name.</param>
        public CustomerForPackage(int id, string name)
        {
            ID = id;
            Name = name;
        }

        /// <summary>
        /// Convert a CustomerForPackage entity to a string.
        /// </summary>
        /// <returns>String representation of a CustomerForPackage.</returns>
        public override string ToString()
        {
            return $"Package Customer: ID: {ID}, Name: {Name}";
        }
    }
}