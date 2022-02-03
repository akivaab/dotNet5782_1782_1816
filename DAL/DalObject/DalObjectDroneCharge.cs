using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using DO;

namespace DalObject
{
    /// <summary>
    /// DroneCharge-related functionality of the Data Layer.
    /// </summary>
    partial class DalObject : DalApi.IDal
    {
        #region Getter Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public DateTime GetTimeChargeBegan(int droneID)
        {
            int droneChargeIndex = DataSource.droneCharges.FindIndex(droneCharge => droneCharge.DroneID == droneID && droneCharge.Active);

            if (droneChargeIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID currently charging.");
            }

            return DataSource.droneCharges[droneChargeIndex].Clone().BeganCharge;
        }
        #endregion

        #region Find Methods
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<DroneCharge> FindDroneCharges(Predicate<DroneCharge> predicate)
        {
            return from droneCharge in DataSource.droneCharges
                   where predicate(droneCharge) && droneCharge.Active
                   select droneCharge.Clone();
        }
        #endregion
    }
}
