
namespace IDAL
{
    namespace DO
    {
        public struct Drone
        {
            public int ID { get; set; }
            public string Model { get; set; }
            public Enums.WeightCategories MaxWeight { get; set; }
            public Enums.DroneStatuses Status { get; set; }
            public double BatteryLevel { get; set; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Model: {Model}, Status: {Status}, Battery Level: {BatteryLevel}, Max. Weight: {MaxWeight}";
            }
        }
    }
}
