using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class Station
        {
            public int ID { get; }
            public int Name { get; }
            public Location Location { get; }
            public int AvailableChargeSlots { get; }
            public List<DroneCharging> DronesCharging { get; }
            public override string ToString()
            {
                return $"Station ID: {ID}, Name: {Name}, Location {Location}, Available Charging Slots: {AvailableChargeSlots}\n" +
                    $"Charging Drones: {DronesCharging}";
            }
        }
    }
}
