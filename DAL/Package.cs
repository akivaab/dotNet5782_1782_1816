using System;

namespace IDAL
{
    namespace DO
    {
        public struct Package
        {
            public int ID { get; set; }
            public int SenderID { get; set; }
            public int ReceiverID { get; set; }
            public Enums.WeightCategories Weight { get; set; }
            public Enums.Priorities Priority { get; set; }
            public int? DroneID { get; set; }
            public DateTime? Requested { get; set; }
            public DateTime? Assigned { get; set; }
            public DateTime? Collected { get; set; }
            public DateTime? Delivered { get; set; }
            public override string ToString()
            {
                return $"Package ID: {ID}, Sender ID: {SenderID}, Receiver ID: {ReceiverID}, Weight: {Weight}, Priority: {Priority}, Drone ID: {DroneID}, " +
                       $"Time requested: {Requested}, Time scheduled: {Assigned}, Time picked up: {Collected}, Time delivered: {Delivered}";
            }
        }
    }
}
