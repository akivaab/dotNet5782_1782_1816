using System.Collections.Generic;

namespace BO
{
    /// <summary>
    /// Station logical entity.
    /// </summary>
    public class Station
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
        /// The station location.
        /// </summary>
        public Location Location { get; set; }

        /// <summary>
        /// The number of charging slots available at the station.
        /// </summary>
        public int AvailableChargeSlots { get; set; }

        /// <summary>
        /// A collection of the drones charging at the station.
        /// </summary>
        public IEnumerable<DroneCharging> DronesCharging { get; set; }

        /// <summary>
        /// Station constructor (for brevity in code).
        /// </summary>
        /// <param name="id">The station ID.</param>
        /// <param name="name">The station name.</param>
        /// <param name="location">The station location.</param>
        /// <param name="chargeSlots">The number of charging slots available at the station.</param>
        /// <param name="dronesCharging">A collection of the drones charging at the station.</param>
        public Station(int id, int name, Location location, int chargeSlots, IEnumerable<DroneCharging> dronesCharging)
        {
            ID = id;
            Name = name;
            Location = location;
            AvailableChargeSlots = chargeSlots;
            DronesCharging = dronesCharging;
        }

        /// <summary>
        /// Convert a Station entity to a string.
        /// </summary>
        /// <returns>String representation of a Station.</returns>
        public override string ToString()
        {
            return $"Station ID: {ID}, Name: {Name}, Location {Location}, Available Charging Slots: {AvailableChargeSlots}\n" +
                $"Charging Drones: {string.Join(";\t", DronesCharging)}";
        }
    }
}
