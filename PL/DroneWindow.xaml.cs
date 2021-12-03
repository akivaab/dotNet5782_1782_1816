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
using IBL.BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        private IBL.IBL bl;
        private DroneToList drone;

        public DroneWindow(IBL.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            Add.Visibility = Visibility.Visible;
            Actions.Visibility = Visibility.Collapsed;

            List<Enums.WeightCategories> weights = new((Enums.WeightCategories[])Enum.GetValues(typeof(Enums.WeightCategories)));
            weights.Remove(Enums.WeightCategories.free);
            Add_MaxWeight.ItemsSource = weights;

            foreach (StationToList station in bl.DisplayAllStations())
            {
                Add_StationID.Items.Add(station.ID);
            }
        }
        public DroneWindow(IBL.IBL bl, DroneToList drone)
        {
            InitializeComponent();
            this.bl = bl;
            this.drone = drone;
            Add.Visibility = Visibility.Collapsed;
            Actions.Visibility = Visibility.Visible;
            LoadDroneData(drone.ID);
        }

        private void LoadDroneData(int droneID)
        {
            Drone drone = bl.DisplayDrone(droneID);

            this.Actions_DroneID.Content = drone.ID;
            this.Actions_Model.Text = drone.Model;
            this.Actions_MaxWeight.Content = drone.MaxWeight;
            this.Actions_Battery.Content = Math.Floor(drone.Battery) + "%";
            this.Actions_Status.Content = drone.Status;
            this.Actions_PackageInTransfer.Content = drone.PackageInTransfer;
            this.Actions_Location.Content = drone.Location;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            int id;
            bool idIsInteger = int.TryParse(Add_DroneID.Text, out id);

            if (idIsInteger && id > 0 && Add_Model.Text.Length > 0 && Add_MaxWeight.SelectedItem != null && Add_StationID.SelectedItem != null)
            {
                try
                {
                    bl.AddDrone(id, Add_Model.Text, (Enums.WeightCategories)Add_MaxWeight.SelectedItem, (int)Add_StationID.SelectedItem);
                    MessageBox.Show("Drone successfully added.");

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(DroneListWindow))
                        {
                            DroneListWindow droneListWindow = (DroneListWindow)window;
                            //Enums.WeightCategories weightFilter = (Enums.WeightCategories)droneListWindow.MaxWeightSelector.SelectedItem;
                            droneListWindow.DroneListView.Items.Refresh();
                        }
                    }

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
                catch(UnableToChargeException)
                {
                    MessageBox.Show("This station has no charge slots available.");
                }
            }
            else
            {
                MessageBox.Show("Please enter valid information.");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Actions_Model.Text.Length > 0)
            {
                bl.UpdateDroneModel(drone.ID, Actions_Model.Text);
                MessageBox.Show("Model name updated successfully.");
                LoadDroneData(drone.ID);
            }
            else
            {
                MessageBox.Show("Enter valid model name.");
            }
        }

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
                    //what about charging time?
                    bl.ReleaseFromCharge(drone.ID, 1.5);
                    MessageBox.Show("Drone successfully released from station.");
                }
                else
                {
                    MessageBox.Show("Cannot charge while delivering.");
                }

                LoadDroneData(drone.ID);
            }
            catch(UnableToChargeException)
            {
                MessageBox.Show("Cannot charge this drone.");
            }
            catch(UnableToReleaseException)
            {
                MessageBox.Show("Cannot release this drone.");
            }
            catch(UndefinedObjectException)
            {
                MessageBox.Show("An error occured when searching for a station.");
            }
        }

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
                    MessageBox.Show("Drone is unavailable.");
                }

                LoadDroneData(drone.ID);
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
        }
    }
}
