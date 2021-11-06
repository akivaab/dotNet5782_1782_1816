using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL
{
    namespace BO
    {
        public class Drone
        {
            public int ID { get; }
            public string Model { get; }
            public Enums.WeightCategories Weight { get; }
            public double Battery { get; }
            public Enums.DroneStatus Status { get; }
            public PackageInTransfer PackageInTransfer { get; }
            public Location Location { get; }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Model: {Model}, Weight: {Weight}, Status: {Status}, Location {Location}\n" +
                    $"Current transferring package {PackageInTransfer}";
            }
        }
    }
}