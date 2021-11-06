
namespace IBL
{
    namespace BO
    {
        public class Location
        {
            public double Latitude { get; }
            public double Longitude { get; }
            public override string ToString()
            {
                return $"{Latitude}, {Longitude}";
            }
        }
    }
}
