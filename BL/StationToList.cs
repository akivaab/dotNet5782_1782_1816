
namespace IBL
{
    namespace BO
    {
        public class StationToList
        {
            public int ID { get; }
            public int Name { get; }
            public int NumAvailableChargeSlots { get; }
            public int NumOccupiedChargeSlots { get; }
            public override string ToString()
            {
                return $"Station ID: {ID}, Name: {Name}\n" +
                        $"Number of charging slots available: {NumAvailableChargeSlots}\n" +
                        $"Number of charging slota occupied: {NumOccupiedChargeSlots}";
            }
        }
    }
}