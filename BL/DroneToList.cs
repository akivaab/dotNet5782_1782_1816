
namespace IBL
{
    namespace BO
    {
        public class DroneToList
        {
            public int ID { get; set; }
            public string Model { get; set; }
            public Enums.WeightCategories MaxWeight { get; set; }
            public double Battery { get; set; }
            public Enums.DroneStatus Status { get; set; }
            public Location Location { get; set; }
            public int PackageID { get; set; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Model: {Model}, Weight: {MaxWeight}, Status: {Status}, Location {Location}\n" +
                    $"Assigned to deliver Package {PackageID}";
            }
        }
    }
}