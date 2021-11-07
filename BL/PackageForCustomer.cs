
namespace IBL
{
    namespace BO
    {
        public class PackageForCustomer
        {
            public int ID { get; set; }
            public Enums.WeightCategories Weight { get; set; }
            public Enums.Priorities Priority { get; set; }
            public Enums.PackageStatus Status { get; set; }
            public CustomerForPackage OtherParty { get; set; }
            public override string ToString()
            {
                return $"Package ID: {ID}, weight: {Weight}, priority: {Priority}, status: {Status}. Transaction with {OtherParty}.";
            }
        }
    }
}