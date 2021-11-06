
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class Customer
        {
            public int ID { get; }
            public string Name { get; }
            public string Phone { get; }
            public Location Location { get; }
            public List<PackageForCustomer> PackagesToSend { get; }
            public List<PackageForCustomer> PackagesToReceive { get; }
            public override string ToString()
            {
                return $"Customer ID: {ID}, Name: {Name}, Phone Number: {Phone}, Location: {Location}\n" +
                    $"Sending Packages: {PackagesToSend}\n" +
                    $"Receiving Packages: {PackagesToReceive}";
            }
        }
    }
}