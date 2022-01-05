using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PO
{
    /// <summary>
    /// Copy of the BO.Station used for Data Binding in the PL.
    /// </summary>
    class Station : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The station ID.
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
        /// The station name.
        /// </summary>
        private int name;
        public int Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    onPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// The station location.
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

        /// <summary>
        /// The number of charging slots available at the station.
        /// </summary>
        private int availableChargeSlots;
        public int AvailableChargeSlots
        {
            get => availableChargeSlots;
            set
            {
                if (availableChargeSlots != value)
                {
                    availableChargeSlots = value;
                    onPropertyChanged("AvailableChargeSlots");
                }
            }
        }

        /// <summary>
        /// A collection of the drones charging at the station.
        /// </summary>
        private ObservableCollection<BO.DroneCharging> dronesCharging;
        public ObservableCollection<BO.DroneCharging> DronesCharging
        {
            get => dronesCharging;
            set
            {
                if (dronesCharging != value)
                {
                    dronesCharging = value;
                    onPropertyChanged("DronesCharging");
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
        /// Construct PO.Station from a BO.Station.
        /// </summary>
        /// <param name="blStation">A BO.Station from the BL.</param>
        public Station(BO.Station blStation)
        {
            ID = blStation.ID;
            Name = blStation.Name;
            Location = blStation.Location;
            AvailableChargeSlots = blStation.AvailableChargeSlots;
            DronesCharging = new ObservableCollection<BO.DroneCharging>(blStation.DronesCharging);
        }

        #endregion
    }
}
