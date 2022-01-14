using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DalXml
{
    /// <summary>
    /// DroneCharge-related functionality of the Data Layer.
    /// </summary>
    partial class DalXml : DalApi.IDal
    {
        #region Getter Methods
        public DateTime GetTimeChargeBegan(int droneID)
        {
            List<DroneCharge> droneCharges = loadListFromXMLSerializer<DroneCharge>(droneChargeXmlPath);

            int droneChargeIndex = droneCharges.FindIndex(droneCharge => droneCharge.DroneID == droneID && droneCharge.Active);
            if (droneChargeIndex == -1)
            {
                throw new UndefinedObjectException("There is no drone with the given ID currently charging.");
            }

            return droneCharges[droneChargeIndex].BeganCharge;
        }
        #endregion

        #region Find Methods
        public IEnumerable<DroneCharge> FindDroneCharges(Predicate<DroneCharge> predicate)
        {
            List<DroneCharge> droneCharges = loadListFromXMLSerializer<DroneCharge>(droneChargeXmlPath);

            return from droneCharge in droneCharges
                   where predicate(droneCharge) && droneCharge.Active
                   select droneCharge;
        }
        #endregion
    }
}
