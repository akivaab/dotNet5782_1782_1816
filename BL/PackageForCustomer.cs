
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

            public PackageForCustomer(int id, Enums.WeightCategories weight, Enums.Priorities priority, Enums.PackageStatus status, CustomerForPackage otherParty)
            {
                ID = id;
                Weight = weight;
                Priority = priority;
                Status = status;
                OtherParty = otherParty;
            }

            public override string ToString()
            {
                return $"Package ID: {ID}, weight: {Weight}, priority: {Priority}, status: {Status}. Transaction with {OtherParty}.";
            }
        }
    }
}