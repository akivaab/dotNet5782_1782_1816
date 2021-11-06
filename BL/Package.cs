using System;

namespace IBL
{
    namespace BO
    {
        public class Package
        {
            public int ID { get; }
            public CustomerForPackage Sender { get; }
            public CustomerForPackage Receiver { get; }
            public Enums.WeightCategories Weight { get; }
            public Enums.Priorities Priority { get; }
            public DroneDelivering DroneDelivering { get; }
            public DateTime RequestTime { get; }
            public DateTime SchedulingTime { get; }
            public DateTime CollectingTime { get; }
            public DateTime DeliveringTime { get; }
            public override string ToString()
            {
                return $"Package ID: {ID}, Weight {Weight}, Priority: {Priority}\n" +
                    $"Send from {Sender} to {Receiver} via drone {DroneDelivering}\n" +
                    $"Package requested at {RequestTime}, assigned to drone at {SchedulingTime}, picked up at {CollectingTime}, delivered at {DeliveringTime}";
            }
        }
    }
}