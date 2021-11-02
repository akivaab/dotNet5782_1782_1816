using System;
using System.Collections.Generic;

namespace IBL
{
    namespace BO
    {
        public class Drone
        {
            public int ID { get; set; }
            public string Model { get; set; }
            public Enums.WeightCategories MaxWeight { get; set; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Model: {Model}, Max. Weight: {MaxWeight}";
            }
        }
    }
}
