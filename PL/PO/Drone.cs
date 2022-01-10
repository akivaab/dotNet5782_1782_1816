using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PO
{
    /// <summary>
    /// Copy of the BO.Drone used for Data Binding in the PL.
    /// </summary>
    class Drone : INotifyPropertyChanged
    {
        #region Fields
        /// <summary>
        /// The drone ID.
        /// </summary>
        private int id;
        public int ID 
        { 
            get => id;
            set
            {
                if (id != value)
                {
                    id = value;
                    onPropertyChanged("ID");
                }
            }
        }

        /// <summary>
        /// The drone model.
        /// </summary>
        private string model;
        public string Model 
        { 
            get => model; 
            set
            {
                if (model != value)
                {
                    model = value;
                    onPropertyChanged("Model");
                }
            }
        }

        /// <summary>
        /// The maximum weight the drone can carry.
        /// </summary>
        private BO.Enums.WeightCategories maxWeight;
        public BO.Enums.WeightCategories MaxWeight
        {
            get => maxWeight;
            set
            {
                if (maxWeight != value)
                {
                    maxWeight = value;
                    onPropertyChanged("MaxWeight");
                }
            }
        }

        /// <summary>
        /// The battery level of the drone.
        /// </summary>
        private double battery;
        public double Battery
        {
            get => battery;
            set
            {
                if (battery != value)
                {
                    battery = value;
                    onPropertyChanged("Battery");
                }
            }
        }

        /// <summary>
        /// The status of the drone (available, maintenance, delivery).
        /// </summary>
        private BO.Enums.DroneStatus status;
        public BO.Enums.DroneStatus Status
        {
            get => status;
            set
            {
                if (status != value)
                {
                    status = value;
                    onPropertyChanged("Status");
                }
            }
        }

        /// <summary>
        /// The package the drone is currently transferring.
        /// </summary>
        private BO.PackageInTransfer packageInTransfer;
        public BO.PackageInTransfer PackageInTransfer 
        { 
            get => packageInTransfer; 
            set
            {
                if (packageInTransfer != value)
                {
                    packageInTransfer = value;
                    onPropertyChanged("PackageInTransfer");
                }
            }
        }

        /// <summary>
        /// The drone location.
        /// </summary>
        private BO.Location location;
        public BO.Location Location
        {
            get => location;
            set
            {
                if (location != value)
                {
                    location = value;
                    onPropertyChanged("Location");
                }
            }
        }
        #endregion

        #region PropertyChanged
        /// <summary>
        /// An event to implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fire the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Construct PO.Drone from a BO.Drone.
        /// </summary>
        /// <param name="blDrone">A BO.Drone from the BL.</param>
        public Drone(BO.Drone blDrone)
        {
            ID = blDrone.ID;
            Model = blDrone.Model;
            MaxWeight = blDrone.MaxWeight;
            Battery = blDrone.Battery;
            Status = blDrone.Status;
            PackageInTransfer = blDrone.PackageInTransfer;
            Location = blDrone.Location;
        }
        #endregion
    }
}
