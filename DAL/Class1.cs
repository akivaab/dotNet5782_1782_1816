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
        }
        public struct DroneCharge
        {
            public int DroneID { get; set; }
            public int StationID { get; set; }
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
