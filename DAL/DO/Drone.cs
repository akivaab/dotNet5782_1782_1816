
namespace DO
{
    /// <summary>
    /// Drone data entity.
    /// </summary>
    public struct Drone
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
        /// The maximum weight a drone can carry.
        /// </summary>
        public Enums.WeightCategories MaxWeight { get; set; }

        /// <summary>
        /// Convert a Drone entity to a string.
        /// </summary>
        /// <returns>String representation of a Drone.</returns>
        public override string ToString()
        {
            return $"Drone ID: {ID}, Model: {Model}, Max. Weight: {MaxWeight}";
        }
    }
}
