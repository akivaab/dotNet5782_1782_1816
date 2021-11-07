
namespace IBL
{
    namespace BO
    {
        public class CustomerToList
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public int NumDeliveredPackagesSent { get; set; }
            public int NumUndeliveredPackagesSent { get; set; }
            public int NumPackagesReceived { get; set; }
            public int NumPackagesExpected { get; set; }

            public override string ToString()
            {
                return $"Customer ID: {ID}, Name: {Name}, Phone Number: {Phone}\n" +
                    $"Number of packages sent that were delivered: {NumDeliveredPackagesSent}\n" +
                    $"Number of package sent not yet delivered: {NumUndeliveredPackagesSent}\n" +
                    $"Number of packages received: {NumPackagesReceived}" +
                    $"Number of packages expecting to receive: {NumPackagesExpected}";
            }
        }
    }
}