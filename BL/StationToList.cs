
namespace IBL
{
    namespace BO
    {
        public class StationToList
        {
            public int ID { get; set; }
            public int Name { get; set; }
            public int NumAvailableChargeSlots { get; set; }
            public int NumOccupiedChargeSlots { get; set; }
            public override string ToString()
            {
                return $"Station ID: {ID}, Name: {Name}\n" +
                        $"Number of charging slots available: {NumAvailableChargeSlots}\n" +
                        $"Number of charging slota occupied: {NumOccupiedChargeSlots}";
            }
        }
    }
}