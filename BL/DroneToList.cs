
namespace IBL
{
    namespace BO
    {
        public class DroneToList
        {
            public int ID { get; }
            public string Model { get; }
            public Enums.WeightCategories Weight { get; }
            public double Battery { get; }
            public Enums.DroneStatus Status { get; }
            public Location Location { get; }
            public int PackageID { get; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Model: {Model}, Weight: {Weight}, Status: {Status}, Location {Location}\n" +
                    $"Assigned to deliver Package {PackageID}";
            }
        }
    }
}