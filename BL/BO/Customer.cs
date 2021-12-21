using System.Collections.Generic;

namespace BO
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Location Location { get; set; }
        public IEnumerable<PackageForCustomer> PackagesToSend { get; set; }
        public IEnumerable<PackageForCustomer> PackagesToReceive { get; set; }
        public Customer(int id, string name, string phone, Location location, IEnumerable<PackageForCustomer> packagesToSend, IEnumerable<PackageForCustomer> packagesToReceive)
        {
            ID = id;
            Name = name;
            Phone = phone;
            Location = location;
            PackagesToSend = packagesToSend;
            PackagesToReceive = packagesToReceive;
        }
        public override string ToString()
        {
            return $"Customer ID: {ID}, Name: {Name}, Phone Number: {Phone}, Location: {Location}\n" +
                $"Sending Packages: {string.Join(";\t", PackagesToSend)}\n" +
                $"Receiving Packages: {string.Join(";\t", PackagesToReceive)}";
        }
    }
}