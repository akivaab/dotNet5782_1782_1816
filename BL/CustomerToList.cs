
namespace IBL
{
    namespace BO
    {
        public class CustomerToList
        {
            public int ID { get; }
            public string Name { get; }
            public string Phone { get; }
            public int NumDeliveredPackagesSent { get; }
            public int NumUndeliveredPackagesSent { get; }
            public int NumPackagesReceived { get; }
            public int NumPackagesExpected { get; }

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