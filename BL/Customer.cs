
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class Customer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public Location Location { get; set; }
            public List<PackageForCustomer> PackagesToSend { get; set; }
            public List<PackageForCustomer> PackagesToReceive { get; set; }
            public override string ToString()
            {
                return $"Customer ID: {ID}, Name: {Name}, Phone Number: {Phone}, Location: {Location}\n" +
                    $"Sending Packages: {PackagesToSend}\n" +
                    $"Receiving Packages: {PackagesToReceive}";
            }
        }
    }
}