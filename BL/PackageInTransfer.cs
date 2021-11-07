
namespace IBL
{
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
            public override string ToString()
            {
                return $"Package ID: {ID}, weight: {Weight}, priority: {Priority}\n" +
                    $"Package in transfer: {Status}\n" +
                    $"Collect from {Sender} at {CollectLocation}, Deliver to {Receiver} at {DeliveryLocation}, distance: {DeliveryDistance}";
            }
        }
    }
}
