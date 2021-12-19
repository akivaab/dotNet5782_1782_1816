using System;

namespace DO
{
    public struct DroneCharge
    {
        public int DroneID { get; set; }
        public int StationID { get; set; }
        public DateTime BeganCharge { get; set; }
        public override string ToString()
        {
            return $"Drone ID: {DroneID}, Station ID: {StationID}";
        }
    }
}
