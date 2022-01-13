using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DO;

namespace DalXml
{
    partial class DalXml : DalApi.IDal
    {
        #region Add Methods
        public void AddDrone(int id, string model, Enums.WeightCategories maxWeight)
        {
            IEnumerable<Drone> drones = loadDronesList();
            
            if (drones.Where(drone => drone.ID == id && drone.Active).Any())
            {
                throw new NonUniqueIdException("The given drone ID is not unique.");
            }

            Drone drone = new();
            drone.ID = id;
            drone.Model = model;
            drone.MaxWeight = maxWeight;
            drone.Active = true;
            droneRoot.Add(drone);

            saveDrones();
        }
        #endregion
    }
}
