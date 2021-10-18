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

        public static void Init() {

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
            }

            while (Config.NextCustomer < 10)
            {
                Customers[Config.NextCustomer].ID = 291816;
                Customers[Config.NextCustomer].Name = "aMx2";
                Customers[Config.NextCustomer].Phone = "123456789";
                Customers[Config.NextCustomer].Latitude = 1000.0;
                Customers[Config.NextCustomer].Longitude = 1000.0;
            }
            while(Config.NextPackage < 10)
            {
                Packages[Config.NextPackage].ID = 123;
                Packages[Config.NextPackage].SenderID = 123;
                Packages[Config.NextPackage].ReceiverID = 123;
                Packages[Config.NextPackage].Weight = Enums.WeightCategories.light;
                Packages[Config.NextPackage].Priority = Enums.Priorities.regular;
                Packages[Config.NextPackage].Requested = DateTime.Now;
                Packages[Config.NextPackage].Scheduled = DateTime.UtcNow;
                Packages[Config.NextPackage].PickedUp = DateTime.Today;
                Packages[Config.NextPackage].Delivered = DateTime.UnixEpoch;
            }

        }
    }
}
