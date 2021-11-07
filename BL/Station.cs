using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class Station
        {
            public int ID { get; set; }
            public int Name { get; set; }
            public Location Location { get; set; }
            public int AvailableChargeSlots { get; set; }
            public List<DroneCharging> DronesCharging { get; set; }
            public override string ToString()
            {
                return $"Station ID: {ID}, Name: {Name}, Location {Location}, Available Charging Slots: {AvailableChargeSlots}\n" +
                    $"Charging Drones: {DronesCharging}";
            }
        }
    }
}
