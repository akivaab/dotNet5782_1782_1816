
namespace DO
{
    public struct Station
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
        /// The number of available charge slots at the station.
        /// </summary>
        public int AvailableChargeSlots { get; set; }

        /// <summary>
        /// The station latitude coordinates.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The station longitude coordinates.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Represents if a station is currently active or not.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Convert a Station entity to a string.
        /// </summary>
        /// <returns>String representation of a Station.</returns>
        public override string ToString()
        {
            return $"Station Name: {Name}, ID: {ID}, Number of charger slots: {AvailableChargeSlots}, Position: {Latitude}, {Longitude}";
        }
    }
}
