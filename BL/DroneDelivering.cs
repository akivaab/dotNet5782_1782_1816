namespace IBL
{
    namespace BO
    {
        public class DroneDelivering        
        {
            public int ID { get; set; }
            public double Battery { get; set; }
            public Location CurrentLocation { get; set; }
            public DroneDelivering(int id, double battery, Location currentLocation)
            {
                ID = id;
                Battery = battery;
                CurrentLocation = currentLocation;
            }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Battery Status: {Battery}, Current Location: {CurrentLocation}";
            }
        }
    }
}