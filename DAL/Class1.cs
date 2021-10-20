using System;

namespace IDAL
{
    namespace DO
    {
        public struct Station
        {
            public int ID { get; set; }
            public int Name { get; set; }
            public int NumChargeSlots { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public override string ToString()
            {
                return $"Station Name: {Name}, ID:{ID} Position: {Latitude}, {Longitude}";
            }
        }
        public struct Drone
        {
            public int ID { get; set; }
            public string Model { get; set; }
            public Enums.WeightCategories MaxWeight { get; set; }
            public Enums.DroneStatuses Status { get; set; }
            public double BatteryLevel { get; set; }
            public override string ToString()
            {
                return $"Drone ID:{ID}, Model: {Model}, Status: {Status}, Battery Level: {BatteryLevel}, Max. Weight: {MaxWeight}";
            }
        }
        public struct Customer
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public override string ToString()
            {
                return $"Customer: {Name} , ID: {ID} Phone: {Phone}, Location: {Latitude}, {Longitude}";
            }

        }
        public struct Package
        {
            public int ID { get; set; }
            public int SenderID { get; set; }
            public int ReceiverID { get; set; }
            public Enums.WeightCategories Weight { get; set; }
            public Enums.Priorities Priority { get; set; }
            public int DroneID { get; set; }
            public DateTime Requested { get; set; }
            public DateTime Scheduled { get; set; }
            public DateTime PickedUp { get; set; }
            public DateTime Delivered { get; set; }
            public override string ToString()
            {
                return $"Package ID:{ID}, senderID: {SenderID}, ReceiverID: {ReceiverID}, Weight: {Weight}, Priority: {Priority}, Drone ID: {DroneID}," +
                    $" Time requested: {Requested}, Time scheduled {Scheduled}, Time picked up: {PickedUp}, Time delivered{Delivered}" ;
            }

        }
        public struct DroneCharge
        {
            public int DroneID { get; set; }
            public int StationID { get; set; }

            public override string ToString()
            {
                return $"Drone ID:{DroneID}, Station ID: {StationID}"; 
            }
        }
        public class Enums
        {
            public enum WeightCategories
            {
                light, medium, heavy
            }
            public enum Priorities
            {
                regular, fast, emergency
            }
            public enum DroneStatuses
            {
                free, maintenance, delivery
            }
        }
    }
    
}

namespace DalObject
{
    using IDAL.DO;
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
        public static void Initialize() {
            Random random = new Random();
            string[] RandomNames = new string[100] { "James", "Robert", "John", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Nancy", "Lisa", "Betty", "Margaret", "Sandra", "Ashley", "Kimberly", "Emily", "Donna", "Michelle", "Kenneth", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Dorothy", "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Sharon", "Laura", "Cynthia", "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon", "Kathleen", "Amy", "Shirley", "Angela", "Helen", "Anna", "Brenda", "Pamela", "Nicole", "Emma", "Benjamin", "Samuel", "Gregory", "Frank", "Alexander", "Raymond", "Patrick", "Jack", "Dennis", "Samantha", "Katherine", "Christine", "Debra", "Rachel", "Catherine", "Carolyn", "Janet", "Ruth", "Jerry", "Maria" };

            int randomStation = random.Next(2,  6);
            while (Config.NextStation < randomStation)
            {
                Stations[Config.NextStation].ID = Config.NextStation + 1;
                Stations[Config.NextStation].Name = Config.NextStation + 1 * 32;
                Stations[Config.NextStation].NumChargeSlots = 5;
                // get a random double between 0-90 then randomly multiply by +/- 1 
                Stations[Config.NextStation].Latitude = (random.NextDouble() * 90) * (random.Next(0, 2) * 2 - 1);
                // longitude is always positive
                Stations[Config.NextStation].Longitude = random.NextDouble() * 180;
                Config.NextStation++;
            }

            int randomDrones = random.Next(5, 11);
            while(Config.NextDrone < randomDrones)
            {
                Drones[Config.NextDrone].ID = Config.NextDrone + 1;
                Drones[Config.NextDrone].Model = $"MX{random.Next(1,6)}";
                Drones[Config.NextDrone].MaxWeight = (Enums.WeightCategories)random.Next(0, 3);
                Drones[Config.NextDrone].Status = Enums.DroneStatuses.free;
                Drones[Config.NextDrone].BatteryLevel = 100.0;
                Config.NextDrone++;
            }

            int randomCustomers = random.Next(10,  101);
            while (Config.NextCustomer < randomCustomers)
            {
                Customers[Config.NextCustomer].ID = Config.NextCustomer + 100000;
                Customers[Config.NextCustomer].Name = RandomNames[Config.NextCustomer];
                Customers[Config.NextCustomer].Phone = random.Next(100000000, 1000000000).ToString();
                // get a random double between 0-90 then randomly multiply by +/- 1 
                Customers[Config.NextCustomer].Latitude = (random.NextDouble() * 90) * (random.Next(0, 2) * 2 - 1);
                // longitude is always positive
                Customers[Config.NextCustomer].Longitude = random.NextDouble() * 180;
                Config.NextCustomer++;
            }

            int randomPackages = random.Next(10, 1001);
            while(Config.NextPackage < randomPackages)
            {
                Packages[Config.NextPackage].ID = Config.PackageID;
                // Choose ID from a random customer based on the size of our Customers array
                Packages[Config.NextPackage].SenderID = Customers[random.Next(0,randomCustomers)].ID;
                Packages[Config.NextPackage].ReceiverID = Customers[random.Next(0, randomCustomers)].ID;
                Packages[Config.NextPackage].Weight = (Enums.WeightCategories)random.Next(0, 3);
                Packages[Config.NextPackage].Priority = (Enums.Priorities)random.Next(0,3);
                Packages[Config.NextPackage].DroneID = 0;
                Packages[Config.NextPackage].Requested = DateTime.Now;
                Config.PackageID++;
                Config.NextPackage++;
            }
        }
    }

    public class DalObject
    {
        /// <summary>
        /// Constructor adds initial values to the entity arrays
        /// </summary>
        public DalObject()
        {
            DataSource.Initialize();
        }

        /// <summary>
        /// Adding a new station to the array
        /// </summary>
        /// <param name="id">Station id</param>
        /// <param name="name">Station name</param>
        /// <param name="numChargeSlots">Numbers of free charging slots avaliable</param>
        /// <param name="latitude">Station latitude location</param>
        /// <param name="longitude">Station longitude location</param>
        public void AddStation(int id, int name, int numChargeSlots, double latitude, double longitude)
        {
            if (DataSource.Config.NextStation < 5)
            {
                DataSource.Stations[DataSource.Config.NextStation].ID = id;
                DataSource.Stations[DataSource.Config.NextStation].Name = name;
                DataSource.Stations[DataSource.Config.NextStation].NumChargeSlots = numChargeSlots;
                DataSource.Stations[DataSource.Config.NextStation].Latitude = latitude;
                DataSource.Stations[DataSource.Config.NextStation].Longitude = longitude;
                DataSource.Config.NextStation++;
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max numbers of stations reached.");
            }
        }

        /// <summary>
        /// Add a new drone to the drone list
        /// </summary>
        /// <param name="id">Drone ID</param>
        /// <param name="model">Drone model</param>
        /// <param name="maxWeight">Maximum weight drone can handle</param>
        /// <param name="status">Drone status</param>
        /// <param name="batteryLevel">Drone battery level</param>
        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight, Enums.DroneStatuses status, double batteryLevel = 100.0)
        {
            if  (DataSource.Config.NextDrone < 10)
            {
                DataSource.Drones[DataSource.Config.NextDrone].ID = id;
                DataSource.Drones[DataSource.Config.NextDrone].Model = model;
                DataSource.Drones[DataSource.Config.NextDrone].MaxWeight = maxWeight;
                DataSource.Drones[DataSource.Config.NextDrone].Status = status;
                DataSource.Drones[DataSource.Config.NextDrone].BatteryLevel = batteryLevel;
                DataSource.Config.NextDrone++;
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max numbers of drones reached.");
            }
        }

        /// <summary> 
        /// Add a new customer to a list 
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="name">Customer name</param>
        /// <param name="phone">Customer phone number</param>
        /// <param name="latitude">Customer latitude location</param>
        /// <param name="longitude">Customer longitude location</param>
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            if(DataSource.Config.NextCustomer < 100)
            {
                DataSource.Customers[DataSource.Config.NextCustomer].ID = id;
                DataSource.Customers[DataSource.Config.NextCustomer].Name = name;
                DataSource.Customers[DataSource.Config.NextCustomer].Phone = phone;
                DataSource.Customers[DataSource.Config.NextCustomer].Latitude = latitude;
                DataSource.Customers[DataSource.Config.NextCustomer].Longitude = longitude;
                DataSource.Config.NextCustomer++;
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max number of customers reached.");
            }
        }

        /// <summary>
        /// Add a package that needs to be delivered
        /// </summary>
        /// <param name="senderID">Package sender ID</param>
        /// <param name="receiverID">Package receiver ID</param>
        /// <param name="weight">Package weight</param>
        /// <param name="priority">Priority of package delivery</param>
        /// <param name="requested">Time package was added</param>
        /// <param name="droneID">ID of drone delivering package</param>
        public void AddPackage(int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority, int droneID = 0)
        {
            if(DataSource.Config.NextPackage < 1000)
            {
                DataSource.Packages[DataSource.Config.NextPackage].ID = DataSource.Config.PackageID;
                DataSource.Packages[DataSource.Config.NextPackage].SenderID = senderID;
                DataSource.Packages[DataSource.Config.NextPackage].ReceiverID = receiverID;
                DataSource.Packages[DataSource.Config.NextPackage].Weight = weight;
                DataSource.Packages[DataSource.Config.NextPackage].Priority = priority;
                DataSource.Packages[DataSource.Config.NextPackage].DroneID = droneID;
                DataSource.Packages[DataSource.Config.NextPackage].Requested = DateTime.Now;
                /*
                DataSource.Packages[DataSource.Config.NextPackage].Scheduled = scheduled;
                DataSource.Packages[DataSource.Config.NextPackage].PickedUp = pickedUp;
                DataSource.Packages[DataSource.Config.NextPackage].Delivered = delivered;
                */
                DataSource.Config.PackageID++;
                DataSource.Config.NextPackage++;
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max number of packages reached.");
            }
        }

        /// <summary>
        /// assign a package to a drone to deliver
        /// </summary>
        /// <param name="packageID">the Package ID</param>
        /// <param name="droneID">the Drone ID</param>
        /// <returns>true if success, false otherwise</returns>/
        public bool AssignPackage(int packageID, int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                Drone drone = DataSource.Drones[i];
                if (drone.ID == droneID)
                {
                    for (int j = 0; j < DataSource.Config.NextPackage; j++)
                    {
                        Package package = DataSource.Packages[j];
                        if (package.ID == packageID)
                        {
                            package.DroneID = droneID;
                            package.Scheduled = DateTime.Now;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Drone collects the assigned package
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>True if success ,else false</returns>
        public bool CollectPackage(int packageID, int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                Drone drone = DataSource.Drones[i];
                if (drone.ID == droneID)
                {
                    if (drone.Status == Enums.DroneStatuses.free)
                    {
                        for (int j = 0; j < DataSource.Config.NextPackage; j++)
                        {
                            Package package = DataSource.Packages[j];
                            if (package.ID == packageID)
                            {
                                package.PickedUp = DateTime.Now;
                                drone.Status = Enums.DroneStatuses.delivery;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Drone delivers a package to the customer
        /// </summary>
        /// <param name="packageID">Package ID</param>
        /// <param name="droneID">Drone ID</param>
        /// <returns>True if success ,else false</returns>
        public bool DeliverPackage(int packageID, int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                Drone drone = DataSource.Drones[i];
                if (drone.ID == droneID)
                {
                    for (int j = 0; j < DataSource.Config.NextPackage; j++)
                    {
                        Package package = DataSource.Packages[j];
                        if (package.ID == packageID)
                        {
                            package.Delivered = DateTime.Now;
                            package.DroneID = 0;
                            drone.Status = Enums.DroneStatuses.free;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// Send a drone to charge in a base station 
        /// </summary>
        /// <param name="droneID">drone ID</param>
        /// <param name="stationID">station ID</param>
        /// <returns>True if success ,else false</returns>
        public bool ChargeDrone(int droneID, int stationID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    for(int j = 0; j < DataSource.Config.NextStation; j++)
                    {
                        if (DataSource.Stations[j].ID == stationID)
                        {
                            DataSource.Drones[i].Status = Enums.DroneStatuses.maintenance;
                            DataSource.Stations[j].NumChargeSlots--;
                            DroneCharge droneCharge = new DroneCharge();
                            droneCharge.DroneID = droneID;
                            droneCharge.StationID = stationID;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Releasing a drone from a charge station
        /// </summary>
        /// <param name="droneID">Drone ID</param>
        /// <param name="stationID">Station ID</param>
        /// <returns>True if success ,else false</returns>        
        public bool ReleaseDroneFromCharging(int droneID, int stationID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    for (int j = 0; j < DataSource.Config.NextStation; j++)
                    {
                        if (DataSource.Stations[j].ID == stationID)
                        {
                            DataSource.Drones[i].Status = Enums.DroneStatuses.free;
                            DataSource.Stations[j].NumChargeSlots++;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        /// <summary>
        /// Display a specific station to the user
        /// </summary>
        /// <param name="stationID">Station ID</param>
        public void DisplayStation(int stationID)
        {
            for (int i = 0; i < DataSource.Config.NextStation; i++)
            {
                if (DataSource.Stations[i].ID == stationID)
                {
                    Console.WriteLine(DataSource.Stations[i].ToString());
                }
            }
        }

        /// <summary>
        /// Display a specific drone to the user
        /// </summary>
        /// <param name="droneID">Drone ID</param>
        public void DisplayDrone(int droneID)
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                if (DataSource.Drones[i].ID == droneID)
                {
                    Console.WriteLine(DataSource.Drones[i].ToString());
                }
            }
        }

        /// <summary>
        /// Display a specific customer to the user
        /// </summary>
        /// <param name="customerID">Customer ID</param>
        public void DisplayCustomer(int customerID)
        {
            for (int i = 0; i < DataSource.Config.NextCustomer; i++)
            {
                if (DataSource.Customers[i].ID == customerID)
                {
                    Console.WriteLine(DataSource.Customers[i].ToString());
                }
            }
        }

        /// <summary>
        /// Display a specific package to the user
        /// </summary>
        /// <param name="packageID">Package ID</param>
        public void DisplayPackage(int packageID)
        {
            for (int i = 0; i < DataSource.Config.NextPackage; i++)
            {
                if (DataSource.Packages[i].ID == packageID)
                {
                    Console.WriteLine(DataSource.Packages[i].ToString());
                }
            }
        }

        /// <summary>
        /// Display all stations to the user
        /// </summary>
        public void DisplayStationsList()
        {
            for (int i = 0; i < DataSource.Config.NextStation; i++)
            {
                Console.WriteLine(DataSource.Stations[i].ToString());
            }
        }

        /// <summary>
        /// Display all drones to the user
        /// </summary>
        public void DisplayDronesList()
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                Console.WriteLine(DataSource.Drones[i].ToString());
            }
        }

        /// <summary>
        /// Display all customers to the user
        /// </summary>
        public void DisplayCustomersList()
        {
            for (int i = 0; i < DataSource.Config.NextCustomer; i++)
            {
                Console.WriteLine(DataSource.Customers[i].ToString());
            }
        }

        /// <summary>
        /// Display all packages to the user 
        /// </summary>
        public void DisplayPackagesList()
        {
            for (int i = 0; i < DataSource.Config.NextPackage; i++)
            {
                Console.WriteLine(DataSource.Packages[i].ToString());
            }
        }

        /// <summary>
        /// Display all packages not assigned to a drone
        /// </summary>
        public void DisplayUnassignedPackagesList()
        {
            for (int i = 0; i < DataSource.Config.NextPackage; i++)
            {
                if (DataSource.Packages[i].DroneID == 0)
                {
                    Console.WriteLine(DataSource.Packages[i].ToString());
                }
            }
        }
        
        /// <summary>
        /// Display all stations with free charge slots
        /// </summary>
        public void DisplayUnoccupiedStationsList()
        {
            for (int i = 0; i < DataSource.Config.NextStation; i++)
            {
                if (DataSource.Stations[i].NumChargeSlots > 0)
                {
                    Console.WriteLine(DataSource.Stations[i].ToString());
                }
            }
        }
    }
}