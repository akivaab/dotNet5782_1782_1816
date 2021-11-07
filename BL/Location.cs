
namespace IBL
{
    namespace BO
    {
        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public override string ToString()
            {
                return $"{Latitude}, {Longitude}";
            }
        }
    }
}
