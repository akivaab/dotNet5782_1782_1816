
namespace IBL
{
    namespace BO
    {
        public class PackageInTransfer
        {
            public int ID { get; }
            public Enums.WeightCategories Weight { get; }
            public Enums.Priorities Priority { get; }
            public bool Status { get; }
            public CustomerForPackage Sender { get; }
            public CustomerForPackage Receiver { get; }
            public Location CollectLocation { get; }
            public Location DeliveryLocation { get; }
            public double DeliveryDistance { get; }
            public override string ToString()
            {
                return $"Package ID: {ID}, weight: {Weight}, priority: {Priority}\n" +
                    $"Package in transfer: {Status}\n" +
                    $"Collect from {Sender} at {CollectLocation}, Deliver to {Receiver} at {DeliveryLocation}, distance: {DeliveryDistance}";
            }
        }
    }
}
