using System;
using System.Collections.Generic;
using DO;

namespace DalObject
{
    /// <summary>
    /// Constitutes the storing location of the entities in the system, and their initialization.
    /// </summary>
    class DataSource
    {
        #region Entity Storage
        /// <summary>
        /// Lists storing the system entity information.
        /// </summary>
        internal static List<Station> stations = new();
        internal static List<Drone> drones = new();
        internal static List<Customer> customers = new();
        internal static List<Package> packages = new();
        internal static List<DroneCharge> droneCharges = new();
        #endregion

        #region Config Class
        /// <summary>
        /// Subclass maintaining values key to configuration of the system.
        /// </summary>
        internal class Config
        {
            /// <summary>
            /// Running number giving each added package a new ID. 
            /// </summary>
            internal static int packageID = 1;

            /// <summary>
            /// Variables representing how much power a drone uses per kilometer when carrying different weights.
            /// </summary>
            internal static double free = 0.01;
            internal static double lightWeight = 0.05;
            internal static double midWeight = 0.1;
            internal static double heavyWeight = 0.15;

            /// <summary>
            /// Represents how much percent a drone charges per hour.
            /// </summary>
            internal static double chargingRate = 20.0;
        }
        #endregion

        #region Data Initialization
        /// <summary>
        /// Initialize all entity listss with random variables.
        /// </summary>/
        public static void Initialize()
        {
            Random random = new Random();
            string[] randomNames = new string[100] { "James", "Robert", "John", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Nancy", "Lisa", "Betty", "Margaret", "Sandra", "Ashley", "Kimberly", "Emily", "Donna", "Michelle", "Kenneth", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Dorothy", "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Sharon", "Laura", "Cynthia", "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon", "Kathleen", "Amy", "Shirley", "Angela", "Helen", "Anna", "Brenda", "Pamela", "Nicole", "Emma", "Benjamin", "Samuel", "Gregory", "Frank", "Alexander", "Raymond", "Patrick", "Jack", "Dennis", "Samantha", "Katherine", "Christine", "Debra", "Rachel", "Catherine", "Carolyn", "Janet", "Ruth", "Jerry", "Maria" };

            int randomStation = random.Next(2, 5);
            while (stations.Count < randomStation)
            {
                Station station = new();
                station.ID = stations.Count + 1;
                station.Name = (stations.Count + 1) * 32;
                station.AvailableChargeSlots = 5;
                // get a random double between 0-1, then randomly multiply by +/- 1 (limited coordinate field)
                station.Latitude = (random.NextDouble() * 1) * (random.Next(0, 2) * 2 - 1);
                // longitude is always a positive double between 0-2 (limited coordinate field)
                station.Longitude = random.NextDouble() * 2;
                station.Active = true;
                stations.Add(station);
            }

            int randomDrones = random.Next(5, 10);
            while (drones.Count < randomDrones)
            {
                Drone drone = new();
                drone.ID = drones.Count + 1;
                drone.Model = $"MX{random.Next(1, 6)}";
                drone.MaxWeight = (Enums.WeightCategories)random.Next(1, 4);
                drone.Active = true;
                drones.Add(drone);
            }

            int randomCustomers = random.Next(10, 100);
            while (customers.Count < randomCustomers)
            {
                Customer customer = new();
                customer.ID = customers.Count + 100000;
                customer.Name = randomNames[customers.Count];
                customer.Phone = random.Next(100000000, 1000000000).ToString();
                // get a random double between 0-1, then randomly multiply by +/- 1 (limited coordinate field)
                customer.Latitude = (random.NextDouble() * 1) * (random.Next(0, 2) * 2 - 1);
                // longitude is always a positive double between 0-2 (limited coordinate field)
                customer.Longitude = random.NextDouble() * 2;
                customer.Active = true;
                customer.Password = customer.ID.ToString();
                customers.Add(customer);
            }

            int randomPackages = random.Next(10, 1000);
            while (packages.Count < randomPackages)
            {
                Package package = new();
                package.ID = Config.packageID;
                // Choose ID from a random customer based on the size of our customers array
                package.SenderID = customers[random.Next(0, randomCustomers)].ID;
                package.ReceiverID = customers[random.Next(0, randomCustomers)].ID;
                package.Weight = (Enums.WeightCategories)random.Next(1, 4);
                package.Priority = (Enums.Priorities)random.Next(0, 3);
                package.Requested = DateTime.Now;

                List<DateTime?> dateTimes = new() { DateTime.Now, null };
                List<int?> droneIDs = new();
                droneIDs.Add(null);
                foreach (Drone drone in drones)
                {
                    droneIDs.Add(drone.ID);
                }

                package.Assigned = dateTimes[random.Next(2)];
                package.Collected = dateTimes[random.Next(2)];
                package.Delivered = dateTimes[random.Next(2)];
                package.DroneID = droneIDs[random.Next(drones.Count)];
                
                package.Active = true;
                packages.Add(package);
                Config.packageID++;
            }
        }
        #endregion
    }
}
