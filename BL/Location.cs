
namespace IBL
{
    namespace BO
    {
        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public Location(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
            public Location()
            {
            }
            public override string ToString()
            {
                return $"{Latitude}, {Longitude}";
            }
        }
    }
}
