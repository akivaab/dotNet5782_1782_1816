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
    /// Copy of the BO.Package used for Data Binding in the PL.
    /// </summary>
    class Package : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The package ID.
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
        /// The customer sending the package.
        /// </summary>
        private BO.CustomerForPackage sender;
        public BO.CustomerForPackage Sender
        {
            get => sender;
            set
            {
                if (sender != value)
                {
                    sender = value;
                    onPropertyChanged("Sender");
                }
            }
        }

        /// <summary>
        /// The customer receiving the package.
        /// </summary>
        private BO.CustomerForPackage receiver;
        public BO.CustomerForPackage Receiver
        {
            get => receiver;
            set
            {
                if (receiver != value)
                {
                    receiver = value;
                    onPropertyChanged("Receiver");
                }
            }
        }

        /// <summary>
        /// The weight of the package.
        /// </summary>
        private BO.Enums.WeightCategories weight;
        public BO.Enums.WeightCategories Weight
        {
            get => weight;
            set
            {
                if (weight != value)
                {
                    weight = value;
                    onPropertyChanged("Weight");
                }
            }
        }

        /// <summary>
        /// The priority of the package (regular, fast, emergency).
        /// </summary>
        private BO.Enums.Priorities priority;
        public BO.Enums.Priorities Priority
        {
            get => priority;
            set
            {
                if (priority != value)
                {
                    priority = value;
                    onPropertyChanged("Priority");
                }
            }
        }

        /// <summary>
        /// The drone delivering the package.
        /// </summary>
        private BO.DroneDelivering droneDelivering;
        public BO.DroneDelivering DroneDelivering
        {
            get => droneDelivering;
            set
            {
                if (droneDelivering != value)
                {
                    droneDelivering = value;
                    onPropertyChanged("DroneDelivering");
                }
            }
        }

        /// <summary>
        /// The time the package was requested/created.
        /// </summary>
        private DateTime? requestTime;
        public DateTime? RequestTime
        {
            get => requestTime;
            set
            {
                if (requestTime != value)
                {
                    requestTime = value;
                    onPropertyChanged("RequestTime");
                }
            }
        }

        /// <summary>
        /// The time the package was assigned to a drone.
        /// </summary>
        private DateTime? assigningTime;
        public DateTime? AssigningTime
        {
            get => assigningTime;
            set
            {
                if (assigningTime != value)
                {
                    assigningTime = value;
                    onPropertyChanged("AssigningTime");
                }
            }
        }

        /// <summary>
        /// The time a package was collected by a drone.
        /// </summary>
        private DateTime? collectingTime;
        public DateTime? CollectingTime
        {
            get => collectingTime;
            set
            {
                if (collectingTime != value)
                {
                    collectingTime = value;
                    onPropertyChanged("CollectingTime");
                }
            }
        }

        /// <summary>
        /// The time a package was delivered by a drone.
        /// </summary>
        private DateTime? deliveringTime;
        public DateTime? DeliveringTime
        {
            get => deliveringTime;
            set
            {
                if (deliveringTime != value)
                {
                    deliveringTime = value;
                    onPropertyChanged("DeliveringTime");
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
        /// Construct PO.Package from a BO.Package.
        /// </summary>
        /// <param name="blPackage">A BO.Package from the BL.</param>
        public Package(BO.Package blPackage)
        {
            ID = blPackage.ID;
            Sender = blPackage.Sender;
            Receiver = blPackage.Receiver;
            Weight = blPackage.Weight;
            Priority = blPackage.Priority;
            DroneDelivering = blPackage.DroneDelivering;
            RequestTime = blPackage.RequestTime;
            AssigningTime = blPackage.AssigningTime;
            CollectingTime = blPackage.CollectingTime;
            DeliveringTime = blPackage.DeliveringTime;
        }

        #endregion
    }
}
