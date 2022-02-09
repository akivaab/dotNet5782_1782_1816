using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PO
{
    /// <summary>
    /// Copy of the BO.Customer used for Data Binding in the PL.
    /// </summary>
    class Customer : INotifyPropertyChanged
    {
        #region Fields
        /// <summary>
        /// The customer ID.
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
        /// The customer's name.
        /// </summary>
        private string name;
        public string Name
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
        /// The customer's phone number.
        /// </summary>
        private string phone;
        public string Phone
        {
            get => phone;
            set
            {
                if (phone != value)
                {
                    phone = value;
                    onPropertyChanged("Phone");
                }
            }
        }

        /// <summary>
        /// The customer location.
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
        /// A collection of PackageForCustomer entities that the customer is sending.
        /// </summary>
        private ObservableCollection<BO.PackageForCustomer> packagesToSend;
        public ObservableCollection<BO.PackageForCustomer> PackagesToSend
        {
            get => packagesToSend;
            set
            {
                if (packagesToSend != value)
                {
                    packagesToSend = value;
                    onPropertyChanged("PackagesToSend");
                }
            }
        }

        /// <summary>
        /// A collection of PackageForCustomer entities that the customer is receiving.
        /// </summary>
        private ObservableCollection<BO.PackageForCustomer> packagesToReceive;
        public ObservableCollection<BO.PackageForCustomer> PackagesToReceive
        {
            get => packagesToReceive;
            set
            {
                if (packagesToReceive != value)
                {
                    packagesToReceive = value;
                    onPropertyChanged("PackagesToReceive");
                }
            }
        }
        #endregion

        #region PropertyChanged
        /// <summary>
        /// An event to implement INotifyPropertyChanged interface.
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
        /// Construct PO.Customer from a BO.Customer.
        /// </summary>
        /// <param name="blCustomer">A BO.Customer from the BL.</param>
        public Customer(BO.Customer blCustomer)
        {
            ID = blCustomer.ID;
            Name = blCustomer.Name;
            Phone = blCustomer.Phone;
            Location = blCustomer.Location;
            PackagesToSend = new ObservableCollection<BO.PackageForCustomer>(blCustomer.PackagesToSend);
            PackagesToReceive = new ObservableCollection<BO.PackageForCustomer>(blCustomer.PackagesToReceive);
        }
        #endregion
    }
}
