
namespace BO
{
    /// <summary>
    /// StationToList logical entity, represents a station stored in a collection.
    /// </summary>
    public class StationToList
    {
        /// <summary>
        /// The station ID.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The station name.
        /// </summary>
        public int Name { get; set; }

        /// <summary>
        /// The number of charging slots available at the station.
        /// </summary>
        public int NumAvailableChargeSlots { get; set; }

        /// <summary>
        /// The number of charging slots occupied at the station.
        /// </summary>
        public int NumOccupiedChargeSlots { get; set; }

        /// <summary>
        /// StationToList constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The station ID.</param>
        /// <param name="name">The station name.</param>
        /// <param name="numAvailableChargeSlots">The number of charging slots available at the station.</param>
        /// <param name="numOccupiedChargeSlots">The number of charging slots occupied at the station.</param>
        public StationToList(int id, int name, int numAvailableChargeSlots, int numOccupiedChargeSlots)
        {
            ID = id;
            Name = name;
            NumAvailableChargeSlots = numAvailableChargeSlots;
            NumOccupiedChargeSlots = numOccupiedChargeSlots;
        }

        /// <summary>
        /// Convert a StationToList entity to a string.
        /// </summary>
        /// <returns>String representation of a StationToList.</returns>
        public override string ToString()
        {
            return $"Station ID: {ID}, Name: {Name}\n" +
                    $"Number of charging slots available: {NumAvailableChargeSlots}\n" +
                    $"Number of charging slots occupied: {NumOccupiedChargeSlots}";
        }
    }
}
