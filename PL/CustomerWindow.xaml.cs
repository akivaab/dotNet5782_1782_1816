using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
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
        private bool allowClose;

        #endregion

        #region Constructors

        /// <summary>
        /// CustomerWindow constructor for adding a customer.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        public CustomerWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            allowClose = false;

            //make only the features needed for adding a customer visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// CustomerWindow constructor for performing actions on a customer.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        /// <param name="customerToList">The station being acted upon.</param>
        public CustomerWindow(BlApi.IBL bl, BO.CustomerToList customerToList)
        {
            InitializeComponent();
            try
            {
                this.customer = new(bl.GetCustomer(customerToList.ID));
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This drone is already removed from the system.\nTry closing this window and refreshing the list.");
            }

            this.bl = bl;
            allowClose = false;

            DataContext = this.customer;

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
                    MessageBox.Show(ex.Message + "\nThe latitude must be between -1 and 1, the longitude between 0 and 2, and the phone number 9 digits long.");
                }
                catch (BO.NonUniqueIdException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease enter a different ID.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter other information." +
                    (isInteger1 || isInteger2 ? "" : "\n(Are the ID and phone number numbers?)") +
                    (isDouble1 || isDouble2 ? "" : "\n(Are the latitude/longitude of the location floating point values?)"));
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

                    reloadCustomerData();
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This customer is not in the system.\nTry closing this window and refreshing the list.");
                }    
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\nIt must be 9 digits long.");
                }
            }
            else
            {
                MessageBox.Show("Please provide a valid phone number");
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
        }

        #endregion

        #region Open Window

        /// <summary>
        /// Open a package detailing a customer package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void package_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.PackageToList packageToList = bl.FindPackages(p => p.ID == (((FrameworkElement)e.OriginalSource).DataContext as BO.PackageForCustomer).ID).Single();
            new PackageWindow(bl, packageToList).Show();
        }

        #endregion

        #region Reload

        /// <summary>
        /// Reload the data of the customer to be displayed in the window.
        /// </summary>
        private void reloadCustomerData()
        {
            try
            {
                BO.Customer customer = bl.GetCustomer(this.customer.ID);
                this.customer.Name = customer.Name;
                this.customer.Phone = customer.Phone;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This customer is not in the system.\nTry closing this window and refreshing the list.");
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
        private void customerWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
