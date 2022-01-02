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
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The customer we enabling the user to update.
        /// </summary>
        private BO.CustomerToList customer;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool allowClose;

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
        /// <param name="customer">The station being acted upon.</param>
        public CustomerWindow(BlApi.IBL bl, BO.CustomerToList customer)
        {
            InitializeComponent();
            this.bl = bl;
            this.customer = customer;
            allowClose = false;

            DataContext = customer;

            //make only the features needed for perfroming actions on a station visible in the window. 
            add.Visibility = Visibility.Collapsed;
            actions.Visibility = Visibility.Visible;

            loadCustomerData();
        }

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

            if (isInteger1 && isInteger2 && isDouble1 && isDouble2 && id > 0 && phoneNumber.ToString().Length == 9)
            {
                try
                {
                    bl.AddCustomer(id, add_Name.Text, phoneNumber.ToString() , new(latitude, longitude));
                    MessageBox.Show("Customer successfully added.");

                    allowClose = true;
                    Close();
                }
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\nThe latitude must be between -1 and 1 and the longitude between 0 and 2.");
                }
                catch (BO.NonUniqueIdException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease enter a different ID.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter other information." +
                    (isInteger1 || isInteger2 ? "" : "\n(Are the ID and PhoneNumbers numbers)") +
                    (isDouble1 || isDouble2 ? "" : "\n(Are the latitude/longitude of the location floating point values?)") +
                    (phoneNumber.ToString().Length == 9 ? "" : "\n(Please provide a valid phone number)"));
            }
        }

        /// <summary>
        /// Update the name of the station and the total number of charging slots it has.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {    
            int phoneNumber;            
            bool isInteger = int.TryParse(actions_PhoneNumber.Text, out phoneNumber);

            if (isInteger && phoneNumber.ToString().Length == 9)
            {
                try
                {
                    bl.UpdateCustomer(customer.ID, actions_Name.Text , phoneNumber.ToString());
                    MessageBox.Show("Customer successfully updated.");
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This customer is not in the system.\nTry closing this window and refreshing the list.");
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
        }

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

        /// <summary>
        /// Load the data of the customer to be displayed in the window.
        /// </summary>
        private void loadCustomerData()
        {
            try
            {
                BO.Customer customerEntity = bl.GetCustomer(customer.ID);

                actions_CustomerID.Content = customerEntity.ID;
                actions_Name.Text = customerEntity.Name;
                actions_PhoneNumber.Text = customerEntity.Phone;
                actions_Location.Content = customerEntity.Location;
                actions_PackageToSend.ItemsSource = customerEntity.PackagesToSend;
                actions_PackageToReceive.ItemsSource = customerEntity.PackagesToReceive;

            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This customer is not in the system.\nTry closing this window and refreshing the list.");
            }
        }

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
    }


}
