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
            public Station(int id, int name, Location location, int chargeSlots, List<DroneCharging> dronesCharging)
            {
                ID = id;
                Name = name;
                Location = location;
                AvailableChargeSlots = chargeSlots;
                DronesCharging = dronesCharging;
            }
            public override string ToString()
            {
                return $"Station ID: {ID}, Name: {Name}, Location {Location}, Available Charging Slots: {AvailableChargeSlots}\n" +
                    $"Charging Drones: {string.Join(";", DronesCharging)}";
            }
        }
    }
}
