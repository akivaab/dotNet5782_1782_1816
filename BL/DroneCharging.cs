
namespace IBL
{
    namespace BO
    {
        public class DroneCharging
        {
            public int ID { get; set; }
            public double Battery { get; set; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Battery Status: {Battery}";
            }
        }
    }
}