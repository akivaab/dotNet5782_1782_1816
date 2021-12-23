
namespace BO
{
    /// <summary>
    /// Location logical entity.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// The latitude coordinates.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// The longitude coordinates.
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Location constructor (for brevity in code).
        /// </summary>
        /// <param name="latitude">The latitude coordinates.</param>
        /// <param name="longitude">The longitude coordinates.</param>
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// Default Location constructor.
        /// </summary>
        public Location() { }

        /// <summary>
        /// Convert a Location entity to a string.
        /// </summary>
        /// <returns>String representation of a Location.</returns>
        public override string ToString()
        {
            return $"{Latitude}, {Longitude}";
        }
    }
}
