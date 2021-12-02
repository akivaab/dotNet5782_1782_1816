
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
            public int? PackageID { get; set; }
            public DroneToList(int id, string model, Enums.WeightCategories maxWeight, double battery, Enums.DroneStatus status, Location location, int? packageID)
            {
                ID = id;
                Model = model;
                MaxWeight = maxWeight;
                Battery = battery;
                Status = status;
                Location = location;
                PackageID = packageID;
            }
            public DroneToList()
            {
            }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Model: {Model}, Weight: {MaxWeight}, Battery: {Battery}%, Status: {Status}, Location {Location}\n" +
                    (PackageID != null ? $"Assigned to deliver Package {PackageID}" : "Not assigned any package");
            }
        }
    }
}