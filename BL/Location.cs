
namespace IBL
{
    namespace BO
    {
        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public Location()
            {
                Latitude = 0;
                Longitude = 0;
            }
            public Location(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }
            public override string ToString()
            {
                return $"{Latitude}, {Longitude}";
            }
        }
    }
}
