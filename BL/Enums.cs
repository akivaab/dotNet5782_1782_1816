using System;
using System.Collections.Generic;

namespace BO
{
    public class Enums
    {
        public enum WeightCategories
        {
            free, light, medium, heavy
        }
        public enum Priorities
        {
            regular, fast, emergency
        }
        public enum DroneStatus
        {
            available, maintenance, delivery
        }
        public enum PackageStatus
        {
            created, assigned, collected, delivered
        }
    }
}
