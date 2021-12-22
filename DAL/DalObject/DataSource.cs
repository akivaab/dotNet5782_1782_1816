using System;
using System.Collections.Generic;
using DO;

namespace DalObject
{
    class DataSource
    {
        internal static List<Station> Stations = new();
        internal static List<Drone> Drones = new();
        internal static List<Customer> Customers = new();
        internal static List<Package> Packages = new();
        internal static List<DroneCharge> DroneCharges = new();

        internal class Config
        {
            internal static int PackageID = 1;
            internal static double Free = 0.01;
            internal static double LightWeight = 0.05;
            internal static double MidWeight = 0.1;
            internal static double HeavyWeight = 0.15;
            internal static double ChargingRate = 20.0;
        }

        /// <summary>
        /// Initialize all entity arrays with random variables
        /// </summary>/
        public static void Initialize()
        {
            Random random = new Random();
            string[] randomNames = new string[100] { "James", "Robert", "John", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Nancy", "Lisa", "Betty", "Margaret", "Sandra", "Ashley", "Kimberly", "Emily", "Donna", "Michelle", "Kenneth", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Dorothy", "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Sharon", "Laura", "Cynthia", "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon", "Kathleen", "Amy", "Shirley", "Angela", "Helen", "Anna", "Brenda", "Pamela", "Nicole", "Emma", "Benjamin", "Samuel", "Gregory", "Frank", "Alexander", "Raymond", "Patrick", "Jack", "Dennis", "Samantha", "Katherine", "Christine", "Debra", "Rachel", "Catherine", "Carolyn", "Janet", "Ruth", "Jerry", "Maria" };

            int randomStation = random.Next(2, 5);
            while (Stations.Count < randomStation)
            {
                Station station = new();
                station.ID = Stations.Count + 1;
                station.Name = (Stations.Count + 1) * 32;
                station.AvailableChargeSlots = 5;
                // get a random double between 0-1, then randomly multiply by +/- 1 (limited coordinate field)
                station.Latitude = (random.NextDouble() * 1) * (random.Next(0, 2) * 2 - 1);
                // longitude is always a positive double between 0-2 (limited coordinate field)
                station.Longitude = random.NextDouble() * 2;
                Stations.Add(station);
            }

            int randomDrones = random.Next(5, 10);
            while (Drones.Count < randomDrones)
            {
                Drone drone = new();
                drone.ID = Drones.Count + 1;
                drone.Model = $"MX{random.Next(1, 6)}";
                drone.MaxWeight = (Enums.WeightCategories)random.Next(1, 4);
                Drones.Add(drone);
            }

            int randomCustomers = random.Next(10, 100);
            while (Customers.Count < randomCustomers)
            {
                Customer customer = new();
                customer.ID = Customers.Count + 100000;
                customer.Name = randomNames[Customers.Count];
                customer.Phone = random.Next(100000000, 1000000000).ToString();
                // get a random double between 0-1, then randomly multiply by +/- 1 (limited coordinate field)
                customer.Latitude = (random.NextDouble() * 1) * (random.Next(0, 2) * 2 - 1);
                // longitude is always a positive double between 0-2 (limited coordinate field)
                customer.Longitude = random.NextDouble() * 2;
                Customers.Add(customer);
            }

            int randomPackages = random.Next(10, 1000);
            while (Packages.Count < randomPackages)
            {
                Package package = new();
                package.ID = Config.PackageID;
                // Choose ID from a random customer based on the size of our Customers array
                package.SenderID = Customers[random.Next(0, randomCustomers)].ID;
                package.ReceiverID = Customers[random.Next(0, randomCustomers)].ID;
                package.Weight = (Enums.WeightCategories)random.Next(1, 4);
                package.Priority = (Enums.Priorities)random.Next(0, 3);
                package.Requested = DateTime.Now;

                List<DateTime?> dateTimes = new() { DateTime.Now, null };
                List<int?> droneIDs = new();
                droneIDs.Add(null);
                foreach (Drone drone in Drones)
                {
                    droneIDs.Add(drone.ID);
                }

                package.Assigned = dateTimes[random.Next(2)];
                package.Collected = dateTimes[random.Next(2)];
                package.Delivered = dateTimes[random.Next(2)];
                package.DroneID = droneIDs[random.Next(Drones.Count)];

                Packages.Add(package);
                Config.PackageID++;
            }
        }
    }
}
