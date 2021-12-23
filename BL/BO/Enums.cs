
namespace BO
{
    /// <summary>
    /// Definitions of various enums used throughout the project.
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// The possible weights of a package a drone can be carrying.
        /// </summary>
        public enum WeightCategories
        {
            free, light, medium, heavy
        }

        /// <summary>
        /// The possible priorities of a package.
        /// </summary>
        public enum Priorities
        {
            regular, fast, emergency
        }

        /// <summary>
        /// The possible statuses of a drone.
        /// </summary>
        public enum DroneStatus
        {
            available, maintenance, delivery
        }

        /// <summary>
        /// The possible statuses of a package.
        /// </summary>
        public enum PackageStatus
        {
            created, assigned, collected, delivered
        }
    }
}
