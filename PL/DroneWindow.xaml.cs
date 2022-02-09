using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window, IRefreshable
    {
        #region Fields
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The drone we enabling the user to update.
        /// </summary>
        private PO.Drone drone;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool allowClose = false;

        /// <summary>
        /// BackgroundWorker for running a simulation of a drone performance. 
        /// </summary>
        private BackgroundWorker bgWorker;

        /// <summary>
        /// Flag if the simulation should close upon completion.
        /// </summary>
        private bool closeFromSimulator = false;
        #endregion

        #region Constructors
        /// <summary>
        /// DroneWindow constructor for adding a drone.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        public DroneWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;

            //make only the features needed for adding a drone visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;

            //remove "free" from the possible MaxWeights to choose from
            List<BO.Enums.WeightCategories> weights = new((BO.Enums.WeightCategories[])Enum.GetValues(typeof(BO.Enums.WeightCategories)));
            weights.Remove(BO.Enums.WeightCategories.free);
            add_MaxWeight.ItemsSource = weights;

            try
            {
                //initialize existing station IDs to choose from
                add_StationID.ItemsSource = from station in bl.GetStationsList()
                                            select station.ID;
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }

        /// <summary>
        /// DroneWindow constructor for performing actions on a drone.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        /// <param name="droneToList">The drone being updated/acted upon.</param>
        public DroneWindow(BlApi.IBL bl, BO.DroneToList droneToList)
        {
            InitializeComponent();
            try
            {
                drone = new(bl.GetDrone(droneToList.ID));
                this.bl = bl;
                DataContext = drone;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This drone is already removed from the system.\nTry closing this window and refreshing the list.");
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
                    bl.AddDrone(id, add_Model.Text, (BO.Enums.WeightCategories)add_MaxWeight.SelectedItem, (int)add_StationID.SelectedItem);
                    MessageBox.Show("Drone successfully added.");

                    allowClose = true;
                    Close();
                }
                catch (BO.NonUniqueIdException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease enter a different ID.");
                }
                catch (BO.UndefinedObjectException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease try a different station.");
                }
                catch (BO.UnableToChargeException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease select a different station.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter other information." +
                    (idIsInteger ? "" : "\n(Is your Drone ID a number?)"));
            }
        }
        #endregion

        #region Action
        /// <summary>
        /// Update the drone model.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (drone.Model.Length > 0)
            {
                try
                {
                    bl.UpdateDroneModel(drone.ID, drone.Model);
                    MessageBox.Show("Model name successfully updated.");
                    refresh();
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: The relevant station does not exist.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
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
                if (drone.Status == BO.Enums.DroneStatus.available)
                {
                    bl.SendDroneToCharge(drone.ID);
                    MessageBox.Show("Drone successfully sent to station to charge.");
                }
                else if (drone.Status == BO.Enums.DroneStatus.maintenance)
                {
                    DateTime beganCharging = bl.GetTimeChargeBegan(drone.ID);
                    bl.ReleaseFromCharge(drone.ID, (DateTime.Now - beganCharging).TotalSeconds);
                    MessageBox.Show("Drone successfully released from station.");
                }
                else
                {
                    MessageBox.Show("The drone is currently delivering and cannot be sent to charge.");
                }

                refresh();
            }
            catch (BO.UnableToChargeException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone available?)");
            }
            catch (BO.UnableToReleaseException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone in maintenance?)");
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: The relevant station does not exist.");
            }
            catch (BO.EmptyListException)
            {
                MessageBox.Show("Error: There are no stations.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
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
                if (drone.Status == BO.Enums.DroneStatus.available)
                {
                    bl.AssignPackage(drone.ID);
                    MessageBox.Show("Package successfully assigned to drone.");
                }
                else if (drone.Status == BO.Enums.DroneStatus.delivery)
                {
                    BO.Package package = bl.GetPackage(drone.PackageInTransfer.ID);
                    
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
                        MessageBox.Show("Error: The drone has completed its delivery but is not considered available.");
                    }
                }
                else
                {
                    MessageBox.Show("The drone is currently unavailable for delivering.\n(Is the drone available?)");
                }

                refresh();
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("An error has occured in the system. The drone no longer exists.");
            }
            catch (BO.UnableToAssignException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone available?)");
            }
            catch (BO.UnableToCollectException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone delivering and not mid-transfer?)");
            }
            catch (BO.UnableToDeliverException ex)
            {
                MessageBox.Show(ex.Message + "\n(Is the drone delivering and mid-transfer?)");
            }
            catch (BO.EmptyListException ex)
            {
                MessageBox.Show(ex.Message + "\nTry using a different drone.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }

        /// <summary>
        /// Remove the drone from the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.RemoveDrone(drone.ID);
                MessageBox.Show("Drone " + drone.ID + " deleted.");

                allowClose = true;
                Close();
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This drone is already removed from the system.\nTry closing this window and refreshing the list.");
            }
            catch (BO.UnableToRemoveException ex)
            {
                MessageBox.Show(ex.Message + "\nThe drone may be removed after this is completed.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }
        #endregion

        #region Open Window
        /// <summary>
        /// Open a more detailed PackageWindow for the package being transferred (if there is one). 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void package_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.PackageInTransfer packageInTransfer = ((FrameworkElement)e.OriginalSource).DataContext as BO.PackageInTransfer;

            try
            {
                if (packageInTransfer != null)
                {
                    bl.GetPackage(packageInTransfer.ID);
                    BO.PackageToList packageToList = bl.FindPackages(p => p.ID == packageInTransfer.ID).Single();
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
        }
        #endregion

        #region Refresh
        /// <summary>
        /// Refresh the data of the drone to be displayed in the window.
        /// </summary>
        public void refresh()
        {
            if (actions.Visibility == Visibility.Visible)
            {
                try
                {
                    BO.Drone drone = bl.GetDrone(this.drone.ID);
                    this.drone.Model = drone.Model;
                    this.drone.Battery = drone.Battery;
                    this.drone.Status = drone.Status;
                    this.drone.PackageInTransfer = drone.PackageInTransfer;
                    this.drone.Location = drone.Location;
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This drone is not in the system.\nTry closing this window and refreshing the list.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
        }
        #endregion

        #region Simulator
        /// <summary>
        /// Run a simulation of this drone performing its duties.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simulatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (bgWorker == null)
            {
                bgWorker = new BackgroundWorker();
                bgWorker.DoWork += bgWorker_DoWork;
                bgWorker.ProgressChanged += bgWorker_ProgressChanged;
                bgWorker.RunWorkerCompleted += bgWorker_RunWorkerCompleted;
                bgWorker.WorkerReportsProgress = true;
                bgWorker.WorkerSupportsCancellation = true;
            }

            if (!bgWorker.IsBusy)
            {
                bgWorker.RunWorkerAsync();
                simulatorButton.Content = "Manual";
                enableControllers(false);
                runningSimulator.Visibility = Visibility.Visible;
            }
            else
            {
                bgWorker.CancelAsync();
                idleMessage.Text = "Stopping simulator...";
                idleMessage.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// The routine assigned to the background worker's DoWork event property, which runs the simulator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bl.ActivateSimulator(drone.ID, bgWorker.ReportProgress, () => bgWorker.CancellationPending);
            }
            catch (BO.UndefinedObjectException ex)
            {
                MessageBox.Show(ex.Message + "Please reset the simulator and try again.");
            }
            catch (BO.UnableToAssignException ex)
            {
                MessageBox.Show(ex.Message + "Please reset the simulator and try again.");
            }
            catch (BO.UnableToCollectException ex)
            {
                MessageBox.Show(ex.Message + "Please reset the simulator and try again.");
            }
            catch (BO.UnableToDeliverException ex)
            {
                MessageBox.Show(ex.Message + "Please reset the simulator and try again.");
            }
            catch (BO.UnableToChargeException ex)
            {
                MessageBox.Show(ex.Message + "Please reset the simulator and try again.");
            }
            catch (BO.UnableToReleaseException ex)
            {
                MessageBox.Show(ex.Message + "Please reset the simulator and try again.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error has occured when saving/loading xml.");
            }
        }

        /// <summary>
        /// The routine assigned to the background worker's ProgressChanged event property, which updates the display windows.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            switch (e.ProgressPercentage)
            {
                case 1:
                    if (!bgWorker.CancellationPending)
                    {
                        idleMessage.Visibility = Visibility.Hidden;
                    }

                    IEnumerable<IRefreshable> refreshableWindows = from Window window in Application.Current.Windows
                                                                   where ((IEnumerable<string>)e.UserState).Contains(window.GetType().Name)
                                                                   select (IRefreshable)window;
                    foreach (IRefreshable window in refreshableWindows)
                    {
                        window.refresh();
                    }
                    break;
                case 2:
                    idleMessage.Text = "Waiting for available charge slot...";
                    idleMessage.Visibility = Visibility.Visible;
                    break;
                case 3:
                    idleMessage.Text = "Waiting for new packages...";
                    idleMessage.Visibility = Visibility.Visible;
                    break;
            }
        }

        /// <summary>
        /// The routine assigned to the background worker's RunWorkerCompleted event property, which restores the window to its pre-simulator state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            simulatorButton.Content = "Automatic";
            idleMessage.Text = "";
            idleMessage.Visibility = Visibility.Hidden;
            runningSimulator.Visibility = Visibility.Hidden;
            MessageBox.Show("Simulation Complete!");

            enableControllers(true);
            if (closeFromSimulator)
            {
                closeFromSimulator = false;
                closeButton.IsEnabled = true;
                allowClose = true;
                Close();
            }
        }

        /// <summary>
        /// Disable/enable certain controllers when the simulation starts/stops running.
        /// </summary>
        /// <param name="enable">Value indocating whether the controllers should be enabled.</param>
        private void enableControllers(bool enable)
        {
            actions_Model.IsReadOnly = !enable;
            removeButton.IsEnabled = enable;
            updateButton.IsEnabled = enable;
            chargeButton.IsEnabled = enable;
            deliverButton.IsEnabled = enable;
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
            if (bgWorker != null && bgWorker.IsBusy)
            {
                closeFromSimulator = true;
                closeButton.IsEnabled = false;

                simulatorButton_Click(sender, e);
            }
            else
            {
                allowClose = true;
                Close();
            }
        }

        /// <summary>
        /// Prevent the window from being closed by force via the X button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void droneWindow_Closing(object sender, CancelEventArgs e)
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
