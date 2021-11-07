using System;

namespace IBL
{
    namespace BO
    {
        public class PackageToList
        {
            public int ID { get; set; }
            public string SenderName { get; set; }
            public string ReceiverName { get; set; }
            public Enums.WeightCategories Weight { get; set; }
            public Enums.Priorities Priority { get; set; }
            public Enums.PackageStatus Status { get; set; }
            public override string ToString()
            {
                return $"Package ID: {ID}, Weight {Weight}, Priority: {Priority}, Status: {Status}\n" +
                    $"Send from {SenderName} to {ReceiverName}";
            }
        }
    }
}