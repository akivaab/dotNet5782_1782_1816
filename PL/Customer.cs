using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    class Customer : INotifyPropertyChanged
    {
        /// <summary>
        /// The customer's ID.
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
        /// The customer's location.
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Customer(BO.Customer blCustomer)
        {
            ID = blCustomer.ID;
            Name = blCustomer.Name;
            Phone = blCustomer.Phone;
            Location = blCustomer.Location;
            PackagesToSend = new ObservableCollection<BO.PackageForCustomer>(blCustomer.PackagesToSend);
            PackagesToReceive = new ObservableCollection<BO.PackageForCustomer>(blCustomer.PackagesToReceive);
        }
    }
}
