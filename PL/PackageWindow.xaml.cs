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
    /// Interaction logic for PackageWindow.xaml
    /// </summary>
    public partial class PackageWindow : Window
    {
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The package we enabling the user to update.
        /// </summary>
        private BO.PackageToList package;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool allowClose;

        /// <summary>
        /// PackageWindow constructor for adding a package.
        /// </summary>
        /// <param name="bl"></param>
        public PackageWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            allowClose = false;

            //make only the features needed for adding a drone visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;

            IEnumerable<int> customerIDs = from BO.CustomerToList customer in bl.GetCustomersList()
                                           select customer.ID;
            add_SenderID.ItemsSource = customerIDs;
            add_ReceiverID.ItemsSource = customerIDs;

            //remove "free" from the possible MaxWeights to choose from
            List<BO.Enums.WeightCategories> weights = new((BO.Enums.WeightCategories[])Enum.GetValues(typeof(BO.Enums.WeightCategories)));
            weights.Remove(BO.Enums.WeightCategories.free);
            add_Weight.ItemsSource = weights;

            add_Priority.ItemsSource = (BO.Enums.Priorities[])Enum.GetValues(typeof(BO.Enums.Priorities));
        }

        /// <summary>
        /// PackageWindow constructor for performing actions on a package.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        /// <param name="package">The package being acted upon.</param>
        public PackageWindow(BlApi.IBL bl, BO.PackageToList package)
        {
            InitializeComponent();
            this.bl = bl;
            this.package = package;
            allowClose = false;

            DataContext = package;

            //make only the features needed for perfroming actions on a drone visible in the window. 
            add.Visibility = Visibility.Collapsed;
            actions.Visibility = Visibility.Visible;

            //load the package information displayed in the window
            loadPackageData();
        }

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
                    MessageBox.Show(ex.Message + "\nPlease try selecting a different option.");
                }
            }
            else
            {
                MessageBox.Show("Please fill in all necessary information.");
            }
        }

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
        }

        /// <summary>
        /// Load the data of the package to be displayed in the window.
        /// </summary>
        private void loadPackageData()
        {
            try
            {
                BO.Package packageEntity = bl.GetPackage(package.ID);

                actions_PackageID.Content = packageEntity.ID;
                actions_Sender.Content = packageEntity.Sender;
                actions_Receiver.Content = packageEntity.Receiver;
                actions_Weight.Content = packageEntity.Weight;
                actions_Priority.Content = package.Priority;
                actions_DroneDelivering.Content = packageEntity.DroneDelivering;
                actions_CreationTime.Content = packageEntity.RequestTime;
                actions_AssignmentTime.Content = packageEntity.AssigningTime;
                actions_CollectionTime.Content = packageEntity.CollectingTime;
                actions_DeliveryTime.Content = packageEntity.DeliveringTime;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This drone is not in the system.\nTry closing this window and refreshing the list.");
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
        private void packageWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the " + (add.Visibility == Visibility.Visible ? "Cancel" : "Close") + " button on the lower right.");
            }
        }
    }
}
