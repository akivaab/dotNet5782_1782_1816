
namespace BO
{
    public class PackageInTransfer
    {
        public int ID { get; set; }
        public Enums.WeightCategories Weight { get; set; }
        public Enums.Priorities Priority { get; set; }
        public bool Status { get; set; }
        public CustomerForPackage Sender { get; set; }
        public CustomerForPackage Receiver { get; set; }
        public Location CollectLocation { get; set; }
        public Location DeliveryLocation { get; set; }
        public double DeliveryDistance { get; set; }
        public PackageInTransfer(int id, Enums.WeightCategories weight, Enums.Priorities priority, bool status, CustomerForPackage sender, CustomerForPackage receiver, Location collectLocation, Location deliveryLocation, double deliveryDistance)
        {
            ID = id;
            Weight = weight;
            Priority = priority;
            Status = status;
            Sender = sender;
            Receiver = receiver;
            CollectLocation = collectLocation;
            DeliveryLocation = deliveryLocation;
            DeliveryDistance = deliveryDistance;
        }
        public override string ToString()
        {
            return $"Package ID: {ID}, Weight: {Weight}, Priority: {Priority}\n" +
                $"Package in transfer: {Status}, Distance to deliver: {DeliveryDistance} km\n" +
                $"  Collect from {Sender} at {CollectLocation}\n" + 
                $"  Deliver to {Receiver} at {DeliveryLocation}";
        }
    }
}
