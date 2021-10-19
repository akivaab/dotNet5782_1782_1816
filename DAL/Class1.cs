using System;

namespace IDAL
{
    namespace DO
    {
        public class Class1
        {

        }
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
                return $"Drone ID:{DroneID}, Station ID{StationID}"; 
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

            static int PackageId = 1;
        }

        public static void Initialize() {

            string[] RandomNames = new string[100] { "James", "Robert", "John", "Michael", "William", "David", "Richard", "Joseph", "Thomas", "Charles", "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen", "Christopher", "Daniel", "Matthew", "Anthony", "Mark", "Donald", "Steven", "Paul", "Andrew", "Joshua", "Nancy", "Lisa", "Betty", "Margaret", "Sandra", "Ashley", "Kimberly", "Emily", "Donna", "Michelle", "Kenneth", "Kevin", "Brian", "George", "Edward", "Ronald", "Timothy", "Jason", "Jeffrey", "Ryan", "Dorothy", "Carol", "Amanda", "Melissa", "Deborah", "Stephanie", "Rebecca", "Sharon", "Laura", "Cynthia", "Jacob", "Gary", "Nicholas", "Eric", "Jonathan", "Stephen", "Larry", "Justin", "Scott", "Brandon", "Kathleen", "Amy", "Shirley", "Angela", "Helen", "Anna", "Brenda", "Pamela", "Nicole", "Emma", "Benjamin", "Samuel", "Gregory", "Frank", "Alexander", "Raymond", "Patrick", "Jack", "Dennis", "Samantha", "Katherine", "Christine", "Debra", "Rachel", "Catherine", "Carolyn", "Janet", "Ruth", "Jerry", "Maria" };

            while (Config.NextStation < 2)
            {
                Stations[Config.NextStation].ID = 1;
                Stations[Config.NextStation].Name = 123;
                Stations[Config.NextStation].NumChargeSlots = 5;
                Stations[Config.NextStation].Latitude = 1000.0;
                Stations[Config.NextStation].Longitude = 1000.0;
                Config.NextStation++;
                }

            while(Config.NextDrone < 5)
            {
                Drones[Config.NextDrone].ID = 2;
                Drones[Config.NextDrone].Model = "MX2";
                Drones[Config.NextDrone].MaxWeight = Enums.WeightCategories.light;
                Drones[Config.NextDrone].Status = Enums.DroneStatuses.free;
                Drones[Config.NextDrone].BatteryLevel = 100.0;
                Config.NextDrone++;
            }

            while (Config.NextCustomer < 10)
            {
                Customers[Config.NextCustomer].ID = 291816;
                Customers[Config.NextCustomer].Name = RandomNames[Config.NextCustomer];
                Customers[Config.NextCustomer].Phone = "123456789";
                Customers[Config.NextCustomer].Latitude = 1000.0;
                Customers[Config.NextCustomer].Longitude = 1000.0;
                Config.NextCustomer++;
            }
            while(Config.NextPackage < 10)
            {
                Packages[Config.NextPackage].ID = 123;
                Packages[Config.NextPackage].SenderID = 123;
                Packages[Config.NextPackage].ReceiverID = 123;
                Packages[Config.NextPackage].Weight = Enums.WeightCategories.light;
                Packages[Config.NextPackage].Priority = Enums.Priorities.regular;
                Packages[Config.NextPackage].DroneID = 0;
                Packages[Config.NextPackage].Requested = DateTime.Now;
                Packages[Config.NextPackage].Scheduled = DateTime.UtcNow;
                Packages[Config.NextPackage].PickedUp = DateTime.Today;
                Packages[Config.NextPackage].Delivered = DateTime.UnixEpoch;
                Config.NextPackage++;
            }

        }
    }

    public class DalObject
    {
        public DalObject()
        {
            DataSource.Initialize();
        }
        //      adding base station to the stations list
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
        //    • adding a drone to the existing drones list
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
        //    • adding a new customer to the customers list
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
        //    • receiving a package to deliver
        public void AddPackage(int id, int senderID, int receiverID, Enums.WeightCategories weight, Enums.Priorities priority, DateTime requested , int droneID = 0)
        {
            if(DataSource.Config.NextPackage < 1000)
            {
                DataSource.Packages[DataSource.Config.NextPackage].ID = id;
                DataSource.Packages[DataSource.Config.NextPackage].SenderID = senderID;
                DataSource.Packages[DataSource.Config.NextPackage].ReceiverID = receiverID;
                DataSource.Packages[DataSource.Config.NextPackage].Weight = weight;
                DataSource.Packages[DataSource.Config.NextPackage].Priority = priority;
                DataSource.Packages[DataSource.Config.NextPackage].DroneID = droneID;
                DataSource.Packages[DataSource.Config.NextPackage].Requested = requested;//DateTime.Now;
                /*
                DataSource.Packages[DataSource.Config.NextPackage].Scheduled = scheduled;
                DataSource.Packages[DataSource.Config.NextPackage].PickedUp = pickedUp;
                DataSource.Packages[DataSource.Config.NextPackage].Delivered = delivered;
                */
                DataSource.Config.NextPackage++;
                Console.WriteLine("Success");
            }
            else
            {
                Console.WriteLine("ERROR: Max number of packages reached.");
            }
        }
        //2. Updating options
        //public Drone? ScanDrones(int droneID)
        //{
        //    foreach (Drone drone in DataSource.Drones)
        //    {
        //        if (drone.ID == droneID)
        //        {
        //            return drone;
        //        }
        //    }
        //    return null;
        //}

        //public Package? ScanPackages(int packageID)
        //{
        //    foreach (Package package in DataSource.Packages)
        //    {
        //        if (package.ID == packageID)
        //        {
        //            return package;
        //        }
        //    }
        //    return null;
        //}
        //    • assigning a package to a drone
        public void AssignPackage(int packageID, int droneID)
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
                            Console.WriteLine("Success");
                            break;
                        }
                    }
                }
            }
        }
        //    • collecting a package by a drone
        public void CollectPackage(int packageID, int droneID)
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
                                Console.WriteLine("Success");
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("ERROR: Drone unavailable.");
                        break;
                    }
                }
            }
        }
        //    • providing a package to a customer
        public void DeliverPackage(int packageID, int droneID)
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
                            Console.WriteLine("Success");
                            break;
                        }
                    }
                }
            }
        }
        //    • sending a drone to a charge in a base station
        //    - by changing the drone’s status and adding a record(instance) of
        //    a drone battery charger entity
        //    - the station is selected by the user in the main menu(It is 
        //    recommended to provide a list of stations to the user)      
        //   •   releasing a drone from charging in a base station 
        //3. Display options(all chosen by a number):
        //    • Display a base station
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
        //    • Display a drone
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
        //    • Display a customer
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
        //    • Display a package
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
        //4. List display options
        //    • Displaying base stations list
        public void DisplayStationsList()
        {
            for (int i = 0; i < DataSource.Config.NextStation; i++)
            {
                Console.WriteLine(DataSource.Stations[i].ToString());
            }
        }
        //    • Displaying drones list
        public void DisplayDronesList()
        {
            for (int i = 0; i < DataSource.Config.NextDrone; i++)
            {
                Console.WriteLine(DataSource.Drones[i].ToString());
            }
        }
        //    • Displaying customers list
        public void DisplayCustomersList()
        {
            for (int i = 0; i < DataSource.Config.NextCustomer; i++)
            {
                Console.WriteLine(DataSource.Customers[i].ToString());
            }
        }
        //    • Displaying packages list
        public void DisplayPackagesList()
        {
            for (int i = 0; i < DataSource.Config.NextPackage; i++)
            {
                Console.WriteLine(DataSource.Packages[i].ToString());
            }
        }
        //    • Displaying packages not assigned yet to a drone
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
        //    • Displaying base stations with unoccupied charging station*/
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
