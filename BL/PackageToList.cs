using System;

namespace IBL
{
    namespace BO
    {
        public class PackageToList
        {
            public int ID { get; }
            public string SenderName { get; }
            public string ReceiverName { get; }
            public Enums.WeightCategories Weight { get; }
            public Enums.Priorities Priority { get; }
            public Enums.PackageStatus Status { get; }
            public override string ToString()
            {
                return $"Package ID: {ID}, Weight {Weight}, Priority: {Priority}, Status: {Status}\n" +
                    $"Send from {SenderName} to {ReceiverName}";
            }
        }
    }
}