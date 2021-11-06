
namespace IBL
{
    namespace BO
    {
        public class DroneCharging
        {
            public int ID { get; }
            public double Battery { get; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Battery Status: {Battery}";
            }
        }
    }
}