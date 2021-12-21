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
        private BlApi.IBL bl;
        private DroneToList drone;

        //flag if Close or Cancel button is clicked 
        private bool closeButtonClicked;

        /// <summary>
        /// DroneWindow constructor for adding a drone.
        /// </summary>
        /// <param name="bl">BL object</param>
        public DroneWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            closeButtonClicked = false;

            //make only the features needed for adding a drone visible in the window. 
            Add.Visibility = Visibility.Visible;
            Actions.Visibility = Visibility.Collapsed;

            //remove "free" from the possible MaxWeights to choose from
            List<Enums.WeightCategories> weights = new((Enums.WeightCategories[])Enum.GetValues(typeof(Enums.WeightCategories)));
            weights.Remove(Enums.WeightCategories.free);
            Add_MaxWeight.ItemsSource = weights;

            //initialize existing station IDs to choose from
            foreach (StationToList station in bl.DisplayAllStations())
            {
                Add_StationID.Items.Add(station.ID);
            }
        }

        /// <summary>
        /// DroneWindow constructor for performing actions on a drone.
        /// </summary>
        /// <param name="bl">BL object</param>
        /// <param name="drone">drone being updated/acted upon</param>
        public DroneWindow(BlApi.IBL bl, DroneToList drone)
        {
            InitializeComponent();
            this.bl = bl;
            this.drone = drone;
            closeButtonClicked = false;

            //make only the features needed for perfroming actions on a drone visible in the window. 
            Add.Visibility = Visibility.Collapsed;
            Actions.Visibility = Visibility.Visible;

            //load the drone information displayed in the window
            LoadDroneData();
        }

        /// <summary>
        /// Add a drone to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int id;
            bool idIsInteger = int.TryParse(Add_DroneID.Text, out id);

            //ascertain that proper data has been entered
            if (idIsInteger && id > 0 && Add_Model.Text.Length > 0 && Add_MaxWeight.SelectedItem != null && Add_StationID.SelectedItem != null)
            {
                try
                {
                    bl.AddDrone(id, Add_Model.Text, (Enums.WeightCategories)Add_MaxWeight.SelectedItem, (int)Add_StationID.SelectedItem);
                    MessageBox.Show("Drone successfully added.");
                    
                    RefreshDroneListWindowView();

                    //even though no button was clicked, allow this window to close
                    closeButtonClicked = true;
                    Close();
                }
                catch (NonUniqueIdException)
                {
                    MessageBox.Show("This Drone ID already exists.");
                }
                catch (UndefinedObjectException)
                {
                    MessageBox.Show("No station with this ID exists.");
                }
                catch (UnableToChargeException)
                {
                    MessageBox.Show("This station has no charge slots available.");
                }
            }
            else
            {
                MessageBox.Show("Please enter valid information.");
            }
        }

        /// <summary>
        /// Update the drone's model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Actions_Model.Text.Length > 0)
            {
                bl.UpdateDroneModel(drone.ID, Actions_Model.Text);
                MessageBox.Show("Model name successfully updated.");
                
                //reload this window and refresh the parent DroneListWindow
                LoadDroneData();
                RefreshDroneListWindowView();
            }
            else
            {
                MessageBox.Show("Enter valid model name.");
            }
        }

        /// <summary>
        /// Send the drone to, or release it from, a charging station.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChargeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((Enums.DroneStatus)Actions_Status.Content == Enums.DroneStatus.available)
                {
                    bl.SendDroneToCharge(drone.ID);
                    MessageBox.Show("Drone successfully sent to station to charge.");
                }
                else if ((Enums.DroneStatus)Actions_Status.Content == Enums.DroneStatus.maintenance)
                {
                    DateTime beganCharging = bl.GetTimeChargeBegan(drone.ID);
                    bl.ReleaseFromCharge(drone.ID, (DateTime.Now - beganCharging).TotalHours);
                    MessageBox.Show("Drone successfully released from station.");
                }
                else
                {
                    MessageBox.Show("Cannot charge while delivering.");
                }

                //reload this window and refresh the parent DroneListWindow
                LoadDroneData();
                RefreshDroneListWindowView();
            }
            catch (UnableToChargeException)
            {
                MessageBox.Show("Cannot charge this drone.");
            }
            catch (UnableToReleaseException)
            {
                MessageBox.Show("Cannot release this drone.");
            }
            catch (UndefinedObjectException)
            {
                MessageBox.Show("An error occured when searching for a station.");
            }
            catch (EmptyListException)
            {
                MessageBox.Show("An error occured when searching for a station.");
            }
        }

        /// <summary>
        /// Depending on the drone's status, either assign it a package, instruct it to collect, or instruct it to deliver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeliverButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((Enums.DroneStatus)Actions_Status.Content == Enums.DroneStatus.available)
                {
                    bl.AssignPackage(drone.ID);
                    MessageBox.Show("Package assigned to drone.");
                }
                else if ((Enums.DroneStatus)Actions_Status.Content == Enums.DroneStatus.delivery)
                {
                    Package package = bl.DisplayPackage((int)drone.PackageID);
                    
                    if (package.CollectingTime == null)
                    {
                        bl.CollectPackage(drone.ID);
                        MessageBox.Show("Package collected.");
                    }
                    else if (package.DeliveringTime == null)
                    {
                        bl.DeliverPackage(drone.ID);
                        MessageBox.Show("Package delivered.");
                    }
                    else
                    {
                        MessageBox.Show("An error has occured. Contact support for help.");
                    }
                }
                else
                {
                    MessageBox.Show("Drone is unavailable for delivering.");
                }

                //reload this window and refresh the parent DroneListWindow
                LoadDroneData();
                RefreshDroneListWindowView();
            }
            catch (UndefinedObjectException)
            {
                MessageBox.Show("An error has occured.");
            }
            catch (UnableToAssignException)
            {
                MessageBox.Show("Drone cannot be assigned a package.");
            }
            catch (UnableToCollectException)
            {
                MessageBox.Show("Drone cannot collect package.");
            }
            catch (UnableToDeliverException)
            {
                MessageBox.Show("Drone cannot deliver package.");
            }
            catch (EmptyListException)
            {
                MessageBox.Show("Drone is incapable of delivering any existing package (or no packages exist).");
            }
        }

        /// <summary>
        /// Load the data of the drone to be displayed in the window.
        /// </summary>
         private void LoadDroneData()
        {
            Drone droneEntity = bl.DisplayDrone(drone.ID);

            Actions_DroneID.Content = droneEntity.ID;
            Actions_Model.Text = droneEntity.Model;
            Actions_MaxWeight.Content = droneEntity.MaxWeight;
            Actions_Battery.Content = Math.Floor(droneEntity.Battery) + "%";
            Actions_Status.Content = droneEntity.Status;
            Actions_PackageInTransfer.Content = droneEntity.PackageInTransfer;
            Actions_Location.Content = droneEntity.Location;
        }

        /// <summary>
        /// Refresh the DroneListView of the parent DroneListWindow to reflect the updates.
        /// </summary>
        private static void RefreshDroneListWindowView()
        {
            //find the open DroneListWindow
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(DroneListWindow))
                {
                    DroneListWindow droneListWindow = (DroneListWindow)window;

                    droneListWindow.DroneListView.Items.Refresh();

                    //remove droneListWindow's DroneListView filters to refresh, then reset the filters
                    Enums.DroneStatus? statusFilter = (Enums.DroneStatus?)droneListWindow.StatusSelector.SelectedItem;
                    Enums.WeightCategories? weightFilter = (Enums.WeightCategories?)droneListWindow.MaxWeightSelector.SelectedItem;

                    droneListWindow.StatusSelector.SelectedItem = null;
                    droneListWindow.MaxWeightSelector.SelectedItem = null;

                    droneListWindow.StatusSelector.SelectedItem = statusFilter;
                    droneListWindow.MaxWeightSelector.SelectedItem = weightFilter;
                }
            }
        }

        /// <summary>
        /// Close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            closeButtonClicked = true;
            Close();
        }

        /// <summary>
        /// Prevent the window from being closed by force via the X button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeButtonClicked)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the " + (Add.Visibility == Visibility.Visible ? "Cancel" : "Close") + " button on the lower right.");
            }
        }
    }
}
