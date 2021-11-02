using System;
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class Package
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
                return $"Package ID: {ID}, Sender ID: {SenderID}, Receiver ID: {ReceiverID}, Weight: {Weight}, Priority: {Priority}, Drone ID: {DroneID}, " +
                       $"Time requested: {Requested}, Time scheduled: {Scheduled}, Time picked up: {PickedUp}, Time delivered: {Delivered}";
            }
        }
    }
}
