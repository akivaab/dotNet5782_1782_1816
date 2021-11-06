
namespace IBL
{
    namespace BO
    {
        public class PackageForCustomer
        {
            public int ID { get; }
            public Enums.WeightCategories Weight { get; }
            public Enums.Priorities Priority { get; }
            public Enums.PackageStatus Status { get; }
            public CustomerForPackage OtherParty { get; }
            public override string ToString()
            {
                return $"Package ID: {ID}, weight: {Weight}, priority: {Priority}, status: {Status}. Transaction with {OtherParty}.";
            }
        }
    }
}