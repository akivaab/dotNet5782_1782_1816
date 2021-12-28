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
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The drone we enabling the user to update.
        /// </summary>
        private DroneToList drone;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool closeButtonClicked;

        /// <summary>
        /// DroneWindow constructor for adding a drone.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        public DroneWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            closeButtonClicked = false;

            //make only the features needed for adding a drone visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;

            //remove "free" from the possible MaxWeights to choose from
            List<Enums.WeightCategories> weights = new((Enums.WeightCategories[])Enum.GetValues(typeof(Enums.WeightCategories)));
            weights.Remove(Enums.WeightCategories.free);
            add_MaxWeight.ItemsSource = weights;

            //initialize existing station IDs to choose from
            add_StationID.ItemsSource = from station in bl.GetStationsList()
                                        select station.ID;
        }

        /// <summary>
        /// DroneWindow constructor for performing actions on a drone.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        /// <param name="drone">The drone being updated/acted upon.</param>
        public DroneWindow(BlApi.IBL bl, DroneToList drone)
        {
            InitializeComponent();
            this.bl = bl;
            this.drone = drone;
            closeButtonClicked = false;
            DataContext = drone;

            //make only the features needed for perfroming actions on a drone visible in the window. 
            add.Visibility = Visibility.Collapsed;
            actions.Visibility = Visibility.Visible;

            //load the drone information displayed in the window
            loadDroneData();
        }

        /// <summary>
        /// Add a drone to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            int id;
            bool idIsInteger = int.TryParse(add_DroneID.Text, out id);

            //ascertain that proper data has been entered
            if (idIsInteger && id > 0 && add_Model.Text.Length > 0 && add_MaxWeight.SelectedItem != null && add_StationID.SelectedItem != null)
            {
                try
                {
                    bl.AddDrone(id, add_Model.Text, (Enums.WeightCategories)add_MaxWeight.SelectedItem, (int)add_StationID.SelectedItem);
                    MessageBox.Show("Drone successfully added.");
                    
                    refreshDroneListWindowView();

                    //even though no button was clicked, allow this window to close
                    closeButtonClicked = true;
                    Close();
                }
                catch (NonUniqueIdException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease enter a different ID.");
                }
                catch (UndefinedObjectException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease try a different station.");
                }
                catch (UnableToChargeException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease select a different station.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter other information." +
                    (idIsInteger ? "" : "\n(Is your Drone ID a number?)"));
            }
        }

        /// <summary>
        /// Update the drone model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (actions_Model.Text.Length > 0)
            {
                try
                {
                    bl.UpdateDroneModel(drone.ID, actions_Model.Text);
                    MessageBox.Show("Model name successfully updated.");

                    //reload this window and refresh the parent DroneListWindow
                    loadDroneData();
                    refreshDroneListWindowView();
                }
                catch (UndefinedObjectException)
                {
                    MessageBox.Show("An error has occured in the system. The relevant station does not exist.");
                }
            }
            else
            {
                MessageBox.Show("The model name supplied is invalid.");
            }
        }

        /// <summary>
        /// Send the drone to, or release it from, a charging station.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chargeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((Enums.DroneStatus)actions_Status.Content == Enums.DroneStatus.available)
                {
                    bl.SendDroneToCharge(drone.ID);
                    MessageBox.Show("Drone successfully sent to station to charge.");
                }
                else if ((Enums.DroneStatus)actions_Status.Content == Enums.DroneStatus.maintenance)
                {
                    DateTime beganCharging = bl.GetTimeChargeBegan(drone.ID);
                    bl.ReleaseFromCharge(drone.ID, (DateTime.Now - beganCharging).TotalHours);
                    MessageBox.Show("Drone successfully released from station.");
                }
                else
                {
                    MessageBox.Show("The drone is currently delivering and cannot be sent to charge.");
                }

                //reload this window and refresh the parent DroneListWindow
                loadDroneData();
                refreshDroneListWindowView();
            }
            catch (UnableToChargeException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone available?)");
            }
            catch (UnableToReleaseException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone in maintenance?)");
            }
            catch (UndefinedObjectException)
            {
                MessageBox.Show("An error has occured in the system. The relevant station does not exist.");
            }
            catch (EmptyListException)
            {
                MessageBox.Show("An error has occured in the system. There are no stations.");
            }
        }

        /// <summary>
        /// Depending on the drone's status, either assign it a package, instruct it to collect, or instruct it to deliver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deliverButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((Enums.DroneStatus)actions_Status.Content == Enums.DroneStatus.available)
                {
                    bl.AssignPackage(drone.ID);
                    MessageBox.Show("Package successfully assigned to drone.");
                }
                else if ((Enums.DroneStatus)actions_Status.Content == Enums.DroneStatus.delivery)
                {
                    Package package = bl.GetPackage((int)drone.PackageID);
                    
                    if (package.CollectingTime == null)
                    {
                        bl.CollectPackage(drone.ID);
                        MessageBox.Show("Package successfully collected.");
                    }
                    else if (package.DeliveringTime == null)
                    {
                        bl.DeliverPackage(drone.ID);
                        MessageBox.Show("Package successfully delivered.");
                    }
                    else
                    {
                        MessageBox.Show("An error has occured in the system. The drone has completed its delivery but is not considered available.");
                    }
                }
                else
                {
                    MessageBox.Show("The drone is currently unavailable for delivering.\n(Is the drone available?)");
                }

                //reload this window and refresh the parent DroneListWindow
                loadDroneData();
                refreshDroneListWindowView();
            }
            catch (UndefinedObjectException)
            {
                MessageBox.Show("An error has occured in the system. The drone no longer exists.");
            }
            catch (UnableToAssignException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone available?)");
            }
            catch (UnableToCollectException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone delivering and not mid-transfer?)");
            }
            catch (UnableToDeliverException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone delivering and mid-transfer?)");
            }
            catch (EmptyListException ex)
            {
                MessageBox.Show(ex.Message + "\nTry using a different drone.");
            }
        }

        /// <summary>
        /// Remove the drone.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.RemoveDrone(drone.ID);
                MessageBox.Show("Drone " + drone.ID + " deleted.");
                refreshDroneListWindowView();

                closeButtonClicked = true;
                Close();
            }
            catch (UndefinedObjectException)
            {
                MessageBox.Show("An error has occured in the system. The drone already doesn't exist.");
            }
            catch (UnableToRemoveException ex)
            {
                MessageBox.Show(ex.Message + "\nTry removing drone after the delivery is completed.");
            }
        }

        /// <summary>
        /// Load the data of the drone to be displayed in the window.
        /// </summary>
        private void loadDroneData()
        {
            try
            {
                Drone droneEntity = bl.GetDrone(drone.ID);

                actions_DroneID.Content = droneEntity.ID;
                actions_Model.Text = droneEntity.Model;
                actions_MaxWeight.Content = droneEntity.MaxWeight;
                actions_Battery.Content = Math.Floor(droneEntity.Battery) + "%";
                actions_Status.Content = droneEntity.Status;
                actions_PackageInTransfer.Content = droneEntity.PackageInTransfer;
                actions_Location.Content = droneEntity.Location;
            }
            catch (UndefinedObjectException)
            {
                MessageBox.Show("An error has occured in the system. The drone no longer exists.");
            }
        }

        /// <summary>
        /// Refresh the DroneListView of the parent DroneListWindow to reflect the updates.
        /// </summary>
        private static void refreshDroneListWindowView()
        {
            //find the open DroneListWindow
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(DroneListWindow))
                {
                    DroneListWindow droneListWindow = (DroneListWindow)window;

                    droneListWindow.droneListView.Items.Refresh();

                    //remove droneListWindow's DroneListView filters to refresh, then reset the filters
                    Enums.DroneStatus? statusFilter = (Enums.DroneStatus?)droneListWindow.statusSelector.SelectedItem;
                    Enums.WeightCategories? weightFilter = (Enums.WeightCategories?)droneListWindow.maxWeightSelector.SelectedItem;

                    droneListWindow.statusSelector.SelectedItem = null;
                    droneListWindow.maxWeightSelector.SelectedItem = null;

                    droneListWindow.statusSelector.SelectedItem = statusFilter;
                    droneListWindow.maxWeightSelector.SelectedItem = weightFilter;
                }
            }
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            closeButtonClicked = true;
            Close();
        }

        /// <summary>
        /// Prevent the window from being closed by force via the X button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void droneWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeButtonClicked)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the " + (add.Visibility == Visibility.Visible ? "Cancel" : "Close") + " button on the lower right.");
            }
        }
    }
}
