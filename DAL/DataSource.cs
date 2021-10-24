using System;
using IDAL.DO;

namespace DalObject
{
    public class DataSource
    {
        internal static Station[] Stations = new Station[5];
        internal static Drone[] Drones = new Drone[10];
        internal static Customer[] Customers = new Customer[100];
        internal static Package[] Packages = new Package[1000];

        internal class Config
        {
            internal static int NextStation = 0;
            internal static int NextDrone = 0;
            internal static int NextCustomer = 0;
            internal static int NextPackage = 0;

            internal static int PackageID = 1;
        }

        /// <summary>
        /// Initialize all entity arrays with random variables
        /// </summary>/
        public static void Initialize()
        {
            Random random = new Random();
            string[] randomNames = new string[100] { "James", "Robert", "John", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Nancy", "Lisa", "Betty", "Margaret", "Sandra", "Ashley", "Kimberly", "Emily", "Donna", "Michelle", "Kenneth", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Dorothy", "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Sharon", "Laura", "Cynthia", "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon", "Kathleen", "Amy", "Shirley", "Angela", "Helen", "Anna", "Brenda", "Pamela", "Nicole", "Emma", "Benjamin", "Samuel", "Gregory", "Frank", "Alexander", "Raymond", "Patrick", "Jack", "Dennis", "Samantha", "Katherine", "Christine", "Debra", "Rachel", "Catherine", "Carolyn", "Janet", "Ruth", "Jerry", "Maria" };

            int randomStation = random.Next(2, 5);
            while (Config.NextStation < randomStation)
            {
                Stations[Config.NextStation].ID = Config.NextStation + 1;
                Stations[Config.NextStation].Name = (Config.NextStation + 1) * 32;
                Stations[Config.NextStation].NumChargeSlots = 5;
                // get a random double between 0-90, then randomly multiply by +/- 1 
                Stations[Config.NextStation].Latitude = (random.NextDouble() * 90) * (random.Next(0, 2) * 2 - 1);
                // longitude is always a positive double between 0-180
                Stations[Config.NextStation].Longitude = random.NextDouble() * 180;
                Config.NextStation++;
            }

            int randomDrones = random.Next(5, 10);
            while (Config.NextDrone < randomDrones)
            {
                Drones[Config.NextDrone].ID = Config.NextDrone + 1;
                Drones[Config.NextDrone].Model = $"MX{random.Next(1, 6)}";
                Drones[Config.NextDrone].MaxWeight = (Enums.WeightCategories)random.Next(0, 3);
                Drones[Config.NextDrone].Status = Enums.DroneStatuses.free;
                Drones[Config.NextDrone].BatteryLevel = 100.0;
                Config.NextDrone++;
            }

            int randomCustomers = random.Next(10, 100);
            while (Config.NextCustomer < randomCustomers)
            {
                Customers[Config.NextCustomer].ID = Config.NextCustomer + 100000;
                Customers[Config.NextCustomer].Name = randomNames[Config.NextCustomer];
                Customers[Config.NextCustomer].Phone = random.Next(100000000, 1000000000).ToString();
                // get a random double between 0-90, then randomly multiply by +/- 1 
                Customers[Config.NextCustomer].Latitude = (random.NextDouble() * 90) * (random.Next(0, 2) * 2 - 1);
                // longitude is always a positive double between 0-180
                Customers[Config.NextCustomer].Longitude = random.NextDouble() * 180;
                Config.NextCustomer++;
            }

            int randomPackages = random.Next(10, 1000);
            while (Config.NextPackage < randomPackages)
            {
                Packages[Config.NextPackage].ID = Config.PackageID;
                // Choose ID from a random customer based on the size of our Customers array
                Packages[Config.NextPackage].SenderID = Customers[random.Next(0, randomCustomers)].ID;
                Packages[Config.NextPackage].ReceiverID = Customers[random.Next(0, randomCustomers)].ID;
                Packages[Config.NextPackage].Weight = (Enums.WeightCategories)random.Next(0, 3);
                Packages[Config.NextPackage].Priority = (Enums.Priorities)random.Next(0, 3);
                Packages[Config.NextPackage].DroneID = 0;
                Packages[Config.NextPackage].Requested = DateTime.Now;
                Config.PackageID++;
                Config.NextPackage++;
            }
        }
    }
}
