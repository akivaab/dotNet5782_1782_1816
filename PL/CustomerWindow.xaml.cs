using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window, IRefreshable
    {
        #region Fields
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The customer we enabling the user to update.
        /// </summary>
        private PO.Customer customer;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool allowClose = false;
        #endregion

        #region Constructors
        /// <summary>
        /// CustomerWindow constructor for adding a customer.
        /// </summary>
        /// <param name="bl">A BL instance.</param>
        public CustomerWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;

            //make only the features needed for adding a customer visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// CustomerWindow constructor for performing actions on a customer.
        /// </summary>
        /// <param name="bl">A BL instance.</param>
        /// <param name="customerToList">The customer being acted upon.</param>
        public CustomerWindow(BlApi.IBL bl, BO.CustomerToList customerToList)
        {
            InitializeComponent();
            try
            {
                this.bl = bl;
                customer = new(this.bl.GetCustomer(customerToList.ID));
                DataContext = customer;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This drone is already removed from the system.\nTry closing this window and refreshing the list.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }

            //make only the features needed for perfroming actions on a station visible in the window. 
            add.Visibility = Visibility.Collapsed;
            actions.Visibility = Visibility.Visible;
        }
        #endregion

        #region Add
        /// <summary>
        /// Add a customer to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            int id;
            int phoneNumber;
            double latitude;
            double longitude;
            bool isInteger1 = int.TryParse(add_CustomerID.Text, out id);
            bool isInteger2 = int.TryParse(add_PhoneNumber.Text, out phoneNumber);           
            bool isDouble1 = double.TryParse(add_Latitude.Text, out latitude);
            bool isDouble2 = double.TryParse(add_Longitude.Text, out longitude);

            if (isInteger1 && isInteger2 && isDouble1 && isDouble2 && id > 0)
            {
                try
                {
                    bl.AddCustomer(id, add_Name.Text, add_PhoneNumber.Text, new(latitude, longitude));
                    MessageBox.Show("Customer successfully added.");

                    allowClose = true;
                    Close();
                }
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\n" +
                        "Suggestions:\n" +
                        "The latitude must be a floating point value between -1 and 1.\n" +
                        "The longitude must be a floating point value between 0 and 2.\n" +
                        "The phone number must be 9 digits long.");
                }
                catch (BO.NonUniqueIdException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease enter a different ID.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter other information.\n" +
                    "Suggestions:\n" +
                        "The ID must consist only of numbers.\n" +
                        "The latitude must be a floating point value between -1 and 1.\n" +
                        "The longitude must be a floating point value between 0 and 2.\n" +
                        "The phone number must be 9 digits long.");
            }
        }
        #endregion

        #region Action
        /// <summary>
        /// Update the customer's name and phone number.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {    
            int phoneNumber;            
            bool isInteger = int.TryParse(customer.Phone, out phoneNumber);

            if (isInteger)
            {
                try
                {
                    bl.UpdateCustomer(customer.ID, customer.Name, customer.Phone);
                    MessageBox.Show("Customer successfully updated.");

                    refresh();
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This customer is not in the system.\nTry closing this window and refreshing the list.");
                }    
                catch (BO.IllegalArgumentException)
                {
                    MessageBox.Show("The phone number must be 9 digits long.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
            else
            {
                MessageBox.Show("The phone number must be 9 digits long.");
            }
        }

        /// <summary>
        /// Remove the customer from the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.RemoveCustomer(customer.ID);
                MessageBox.Show("Customer " + customer.ID + " deleted.");

                allowClose = true;
                Close();
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This customer is already removed from the system.\nTry closing this window and refreshing the list.");
            }
            catch (BO.UnableToRemoveException ex)
            {
                MessageBox.Show(ex.Message + "\nIt cannot be removed.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }
        #endregion

        #region Open Window
        /// <summary>
        /// Open a window detailing a customer package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void package_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.PackageForCustomer packageForCustomer = ((FrameworkElement)e.OriginalSource).DataContext as BO.PackageForCustomer;

            try
            {
                if (packageForCustomer != null)
                {
                    bl.GetPackage(packageForCustomer.ID);
                    BO.PackageToList packageToList = bl.FindPackages(p => p.ID == packageForCustomer.ID).Single();
                    new PackageWindow(bl, packageToList).Show();
                }
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("The package has been deleted. Refresh by closing and reopening the window.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
            catch (Exception)
            {
                MessageBox.Show("Critical Error: Cannot resolve a package with this ID.");
            }
        }
        #endregion

        #region Refresh
        public void refresh()
        {
            if (actions.Visibility == Visibility.Visible)
            {
                try
                {
                    BO.Customer customer = bl.GetCustomer(this.customer.ID);
                    this.customer.Name = customer.Name;
                    this.customer.Phone = customer.Phone;
                    this.customer.PackagesToSend = new ObservableCollection<BO.PackageForCustomer>(customer.PackagesToSend);
                    this.customer.PackagesToReceive = new ObservableCollection<BO.PackageForCustomer>(customer.PackagesToReceive);
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This customer is not in the system.\nTry closing this window and refreshing the list.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
        }
        #endregion

        #region Close
        /// <summary>
        /// Close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            allowClose = true;
            Close();
        }

        /// <summary>
        /// Prevent the window from being closed by force via the X button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the " + (add.Visibility == Visibility.Visible ? "Cancel" : "Close") + " button on the lower right.");
            }
        }
        #endregion
    }
}
