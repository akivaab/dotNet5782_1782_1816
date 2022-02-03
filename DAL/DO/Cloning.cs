
namespace DO
{
    /// <summary>
    /// Methods to deep-copy every DAL entity in the DO namespace.
    /// </summary>
    static class Cloning
    {
        /// <summary>
        /// Clone a Customer entity.
        /// </summary>
        /// <param name="original">The Customer being cloned.</param>
        /// <returns>A copy of the Customer.</returns>
        public static Customer Clone(this Customer original)
        {
            Customer target = new Customer();
            target.ID = original.ID;
            target.Name = original.Name;
            target.Phone = original.Phone;
            target.Latitude = original.Latitude;
            target.Longitude = original.Longitude;
            target.Active = original.Active;
            target.Password = original.Password;
            return target;
        }

        /// <summary>
        /// Clone a Drone entity.
        /// </summary>
        /// <param name="original">The Drone being cloned.</param>
        /// <returns>A copy of the Drone.</returns>
        public static Drone Clone(this Drone original)
        {
            Drone target = new Drone();
            target.ID = original.ID;
            target.Model = original.Model;
            target.MaxWeight = original.MaxWeight;
            target.Active = original.Active;
            return target;
        }

        /// <summary>
        /// Clone a DroneCharge entity.
        /// </summary>
        /// <param name="original">The DroneCharge being cloned.</param>
        /// <returns>A copy of the DroneCharge.</returns>
        public static DroneCharge Clone(this DroneCharge original)
        {
            DroneCharge target = new DroneCharge();
            target.DroneID = original.DroneID;
            target.StationID = original.StationID;
            target.BeganCharge = original.BeganCharge;
            target.Active = original.Active;
            return target;
        }

        /// <summary>
        /// Clone a Package entity.
        /// </summary>
        /// <param name="original">The Package being cloned.</param>
        /// <returns>A copy of the Package.</returns>
        public static Package Clone(this Package original)
        {
            Package target = new Package();
            target.ID = original.ID;
            target.SenderID = original.SenderID;
            target.ReceiverID = original.ReceiverID;
            target.Weight = original.Weight;
            target.Priority = original.Priority;
            target.DroneID = original.DroneID;
            target.Requested = original.Requested;
            target.Assigned = original.Assigned;
            target.Collected = original.Collected;
            target.Delivered = original.Delivered;
            target.Active = original.Active;
            return target;
        }

        /// <summary>
        /// Clone a Station entity.
        /// </summary>
        /// <param name="original">The Station being cloned.</param>
        /// <returns>A copy of the Station.</returns>
        public static Station Clone(this Station original)
        {
            Station target = new Station();
            target.ID = original.ID;
            target.Name = original.Name;
            target.AvailableChargeSlots = original.AvailableChargeSlots;
            target.Latitude = original.Latitude;
            target.Longitude = original.Longitude;
            target.Active = original.Active;
            return target;
        }
    }
}
