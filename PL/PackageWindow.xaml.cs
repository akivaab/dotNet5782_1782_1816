using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for PackageWindow.xaml
    /// </summary>
    public partial class PackageWindow : Window, IRefreshable
    {
        #region Fields
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The package we enabling the user to update.
        /// </summary>
        private PO.Package package;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool allowClose = false;
        #endregion

        #region Constructors
        /// <summary>
        /// PackageWindow constructor for adding a package.
        /// </summary>
        /// <param name="bl">The BL instance.</param>
        public PackageWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;

            //make only the features needed for adding a drone visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;

            try
            {
                IEnumerable<int> customerIDs = from BO.CustomerToList customer in this.bl.GetCustomersList()
                                               select customer.ID;
                add_SenderID.ItemsSource = customerIDs;
                add_ReceiverID.ItemsSource = customerIDs;
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }

            //remove "free" from the possible MaxWeights to choose from
            List<BO.Enums.WeightCategories> weights = new((BO.Enums.WeightCategories[])Enum.GetValues(typeof(BO.Enums.WeightCategories)));
            weights.Remove(BO.Enums.WeightCategories.free);
            add_Weight.ItemsSource = weights;

            add_Priority.ItemsSource = (BO.Enums.Priorities[])Enum.GetValues(typeof(BO.Enums.Priorities));
        }

        /// <summary>
        /// PackageWindow constructor for adding a package when done through the customer interface.
        /// </summary>
        /// <param name="bl">The BL instance.</param>
        /// <param name="customerID">The ID of the customer requesting a package to send.</param>
        public PackageWindow(BlApi.IBL bl, int customerID) : this(bl)
        {
            add_SenderID.SelectedItem = customerID;
            add_SenderID.IsEnabled = false;
        }

        /// <summary>
        /// PackageWindow constructor for performing actions on a package.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        /// <param name="packageToList">The package being acted upon.</param>
        public PackageWindow(BlApi.IBL bl, BO.PackageToList packageToList)
        {
            InitializeComponent();
            try
            {
                this.bl = bl;
                package = new(this.bl.GetPackage(packageToList.ID));
                DataContext = package;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This package has been removed from the system.\nTry closing this window and refreshing the list.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }

            //make only the features needed for perfroming actions on a drone visible in the window. 
            add.Visibility = Visibility.Collapsed;
            actions.Visibility = Visibility.Visible;
        }
        #endregion

        #region Add
        /// <summary>
        /// Add a package to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (add_SenderID.SelectedItem != null && add_ReceiverID.SelectedItem != null && add_Weight.SelectedItem != null && add_Priority.SelectedItem != null)
            {
                try
                {
                    BO.Package newPackage = bl.AddPackage((int)add_SenderID.SelectedItem, (int)add_ReceiverID.SelectedItem, (BO.Enums.WeightCategories)add_Weight.SelectedItem, (BO.Enums.Priorities)add_Priority.SelectedItem);
                    MessageBox.Show("Package " + newPackage.ID + " successfully added.");

                    allowClose = true;
                    Close();
                }
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease select a different option.");
                }
                catch (BO.UndefinedObjectException ex)
                {
                    MessageBox.Show("Either the sender, receiver, or both have been removed from the system.\nPlease try selecting a different option.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
            else
            {
                MessageBox.Show("A choice must be selected for every field.");
            }
        }
        #endregion

        #region Action
        /// <summary>
        /// Remove the package from the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.RemovePackage(package.ID);
                MessageBox.Show("Package " + package.ID + " deleted.");

                allowClose = true;
                Close();
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This package is already removed from the system.\nTry closing this window and refreshing the list.");
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
        /// Open a more detailed DroneWindow for the drone delivering the package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drone_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.DroneDelivering droneDelivering = ((FrameworkElement)e.OriginalSource).DataContext as BO.DroneDelivering;
            
            try
            {
                if (droneDelivering != null)
                {
                    bl.GetDrone(droneDelivering.ID);
                    BO.DroneToList droneToList = bl.FindDrones(d => d.ID == droneDelivering.ID).Single();
                    new DroneWindow(bl, droneToList).Show();
                }
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("The drone has been deleted. Refresh by closing and reopening the window.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }

        /// <summary>
        /// Open a more detailed CustomerWindow for the sender of receiver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.CustomerForPackage customerForPackage = ((FrameworkElement)e.OriginalSource).DataContext as BO.CustomerForPackage;
            
            try
            {
                if (customerForPackage != null)
                {
                    bl.GetCustomer(customerForPackage.ID);
                    BO.CustomerToList customerToList = bl.FindCustomers(c => c.ID == customerForPackage.ID).Single();
                    new CustomerWindow(bl, customerToList).Show();
                }
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("The customer has been deleted. Refresh by closing and reopening the window.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
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
                    BO.Package package = bl.GetPackage(this.package.ID);
                    this.package.DroneDelivering = package.DroneDelivering;
                    this.package.RequestTime = package.RequestTime;
                    this.package.AssigningTime = package.AssigningTime;
                    this.package.CollectingTime = package.CollectingTime;
                    this.package.DeliveringTime = package.DeliveringTime;
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This package is not in the system.\nTry closing this window and refreshing the list.");
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
        private void packageWindow_Closing(object sender, CancelEventArgs e)
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
