namespace IBL
{
    namespace BO
    {
        public class DroneDelivering        
        {
            public int ID { get; }
            public double Battery { get; }
            public Location CurrentLocation { get; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Battery Status: {Battery}, Current Location: {CurrentLocation}";
            }
        }
    }
}