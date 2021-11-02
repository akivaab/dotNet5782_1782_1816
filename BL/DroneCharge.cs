using System;
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class DroneCharge
        {
            public int DroneID { get; set; }
            public int StationID { get; set; }
            public override string ToString()
            {
                return $"Drone ID: {DroneID}, Station ID: {StationID}";
            }
        }
    }
}
