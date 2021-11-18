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
            public PackageToList(int id, string senderName, string receiverName, Enums.WeightCategories weight, Enums.Priorities priority, Enums.PackageStatus status)
            {
                ID = id;
                SenderName = senderName;
                ReceiverName = receiverName;
                Weight = weight;
                Priority = priority;
                Status = status;
            }
            public override string ToString()
            {
                return $"Package ID: {ID}, Weight: {Weight}, Priority: {Priority}, Status: {Status}\n" +
                    $"Send from {SenderName} to {ReceiverName}";
            }
        }
    }
}