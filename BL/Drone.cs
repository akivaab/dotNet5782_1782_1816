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
            public int ID { get; set; }
            public string Model { get; set; }
            public Enums.WeightCategories MaxWeight { get; set; }
            public double Battery { get; set; }
            public Enums.DroneStatus Status { get; set; }
            public PackageInTransfer PackageInTransfer { get; set; }
            public Location Location { get; set; }
            public Drone(int id, string model, Enums.WeightCategories maxWeight, double battery, Enums.DroneStatus status, PackageInTransfer packageInTransfer, Location location)
            {
                ID = id;
                Model = model;
                MaxWeight = maxWeight;
                Battery = battery;
                Status = status;
                PackageInTransfer = packageInTransfer;
                Location = location;
            }
            public override string ToString()
            {
                return $"Drone ID: {ID}, Model: {Model}, Max. Weight: {MaxWeight}, Status: {Status}, Location: {Location}\n" +
                    $"Currently transferring package: {PackageInTransfer}";
            }
        }
    }
}