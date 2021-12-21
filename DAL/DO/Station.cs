
namespace DO
{
    public struct Station
    {
        public int ID { get; set; }
        public int Name { get; set; }
        public int AvailableChargeSlots { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public override string ToString()
        {
            return $"Station Name: {Name}, ID: {ID}, Number of charger slots: {AvailableChargeSlots}, Position: {Latitude}, {Longitude}";
        }
    }
}
