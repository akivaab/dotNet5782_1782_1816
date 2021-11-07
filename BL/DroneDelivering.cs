namespace IBL
{
    namespace BO
    {
        public class DroneDelivering        
        {
            public int ID { get; set; }
            public double Battery { get; set; }
            public Location CurrentLocation { get; set; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Battery Status: {Battery}, Current Location: {CurrentLocation}";
            }
        }
    }
}