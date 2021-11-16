using System;

namespace IBL
{
    namespace BO
    {
        public class Package
        {
            public int ID { get; set; }
            public CustomerForPackage Sender { get; set; }
            public CustomerForPackage Receiver { get; set; }
            public Enums.WeightCategories Weight { get; set; }
            public Enums.Priorities Priority { get; set; }
            public DroneDelivering DroneDelivering { get; set; }
            public DateTime RequestTime { get; set; }
            public DateTime AssigningTime { get; set; }
            public DateTime CollectingTime { get; set; }
            public DateTime DeliveringTime { get; set; }

            public Package(int id, CustomerForPackage sender, CustomerForPackage receiver, Enums.WeightCategories weight, Enums.Priorities priority, DroneDelivering droneDelivering, DateTime requestTime, DateTime assigningTime, DateTime collectingTime, DateTime deliveringTime)
            {
                ID = id;
                Sender = sender;
                Receiver = receiver;
                Weight = weight;
                Priority = priority;
                DroneDelivering = droneDelivering;
                RequestTime = requestTime;
                AssigningTime = assigningTime;
                CollectingTime = collectingTime;
                DeliveringTime = deliveringTime;
            }
            public override string ToString()
            {
                return $"Package ID: {ID}, Weight {Weight}, Priority: {Priority}\n" +
                    $"Send from {Sender} to {Receiver} via drone {DroneDelivering}\n" +
                    $"Package requested at {RequestTime}, assigned to drone at {AssigningTime}, picked up at {CollectingTime}, delivered at {DeliveringTime}";
            }
        }
    }
}