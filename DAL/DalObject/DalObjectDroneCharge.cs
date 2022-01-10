using System;
using System.Collections.Generic;
using System.Linq;
using DO;

namespace DalObject
{
    /// <summary>
    /// DroneCharge-related functionality of the Data Layer.
    /// </summary>
    partial class DalObject : DalApi.IDal
    {
        #region Find Methods
        public IEnumerable<DroneCharge> FindDroneCharges(Predicate<DroneCharge> predicate)
        {
            return from droneCharge in DataSource.droneCharges
                   where predicate(droneCharge) && droneCharge.Active
                   select droneCharge;
        }
        #endregion
    }
}
