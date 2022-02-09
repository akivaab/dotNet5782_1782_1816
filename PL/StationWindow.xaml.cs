using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window, IRefreshable
    {
        #region Fields
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The station we are enabling the user to update as PO entity.
        /// </summary>
        private PO.Station station;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool allowClose = false;
        #endregion

        #region Constructors
        /// <summary>
        /// StationWindow constructor for adding a station.
        /// </summary>
        /// <param name="bl">A BL instance.</param>
        public StationWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;

            //make only the features needed for adding a station visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// StationWindow constructor for performing actions on a station.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        /// <param name="stationToList">The station being acted upon.</param>
        public StationWindow(BlApi.IBL bl, BO.StationToList stationToList)
        {
            InitializeComponent();
            try
            {
                this.bl = bl;
                station = new(this.bl.GetStation(stationToList.ID));
                DataContext = station;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This station is already removed from the system.\nTry closing this window and refreshing the list.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }

            //make only the features needed for perfroming actions on a station visible in the window. 
            add.Visibility = Visibility.Collapsed;
            actions.Visibility = Visibility.Visible;

            actions_TotalChargeSlots.Text = (station.AvailableChargeSlots + station.DronesCharging.Count).ToString();
        }
        #endregion

        #region Add
        /// <summary>
        /// Add a station to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            int id;
            int name;
            int numAvailableChargeSlots;
            double latitude;
            double longitude;
            bool isInteger1 = int.TryParse(add_StationID.Text, out id);
            bool isInteger2 = int.TryParse(add_Name.Text, out name);
            bool isInteger3 = int.TryParse(add_AvailableChargeSlots.Text, out numAvailableChargeSlots);
            bool isDouble1 = double.TryParse(add_Latitude.Text, out latitude);
            bool isDouble2 = double.TryParse(add_Longitude.Text, out longitude);

            if (isInteger1 && isInteger2 && isInteger3 && isDouble1 && isDouble2 && id > 0)
            {
                try
                {
                    bl.AddStation(id, name, new(latitude, longitude), numAvailableChargeSlots);
                    MessageBox.Show("Station successfully added.");

                    allowClose = true;
                    Close();
                }
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\n" +
                        "Suggestions:\n" +
                        "The latitude must be a floating point value between -1 and 1.\n" +
                        "The longitude must be a floating point value between 0 and 2.\n" +
                        "The number of charge slots must be positive.");
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
                        "The ID and name must both be numbers.\n" +
                        "The latitude must be a floating point value between -1 and 1.\n" +
                        "The longitude must be a floating point value between 0 and 2.\n" +
                        "The number of charge slots must be positive.");
            }
        }
        #endregion

        #region Action
        /// <summary>
        /// Update the name of the station and the total number of charging slots it has.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            int name;
            int numTotalChargeSlots;
            bool isInteger1 = int.TryParse(actions_Name.Text, out name);
            bool isInteger2 = int.TryParse(actions_TotalChargeSlots.Text, out numTotalChargeSlots);
            
            if (isInteger1 && isInteger2)
            {
                try
                {
                    bl.UpdateStation(station.ID, station.Name, numTotalChargeSlots);
                    MessageBox.Show("Station successfully updated.");

                    refresh();
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This station is not in the system.\nTry closing this window and refreshing the list.");
                }
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\nTry entering a larger number.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter different information.\n" +
                     "Suggestions:\n" +
                        "The name must be a number.\n" +
                        "The number of charge slots must be positive.");
            }
        }

        /// <summary>
        /// Remove the station from the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bl.RemoveStation(station.ID);
                MessageBox.Show("Station " + station.ID + " deleted.");

                allowClose = true;
                Close();
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This station is already removed from the system.\nTry closing this window and refreshing the list.");
            }
            catch (BO.UnableToRemoveException ex)
            {
                MessageBox.Show(ex.Message + "\nIt cannot be removed.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
            catch (BO.LinqQueryException)
            {
                MessageBox.Show("Critical Error: A query has failed.\nTry restarting the system.");
            }
        }
        #endregion

        #region Open Window
        /// <summary>
        /// Open a DroneWindow detailing a drone charging at the station.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dronesCharging_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.DroneCharging droneCharging = ((FrameworkElement)e.OriginalSource).DataContext as BO.DroneCharging;
            try
            {
                if (droneCharging != null)
                {
                    bl.GetDrone(droneCharging.ID);
                    BO.DroneToList droneToList = bl.FindDrones(d => d.ID == droneCharging.ID).Single();
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
            catch (Exception)
            {
                MessageBox.Show("Critical Error: Cannot resolve a drone with this ID.");
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
                    BO.Station station = bl.GetStation(this.station.ID);
                    this.station.Name = station.Name;
                    this.station.AvailableChargeSlots = station.AvailableChargeSlots;
                    this.station.DronesCharging = new ObservableCollection<BO.DroneCharging>(station.DronesCharging);

                    actions_TotalChargeSlots.Text = (station.AvailableChargeSlots + station.DronesCharging.Count()).ToString();
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This station is not in the system.\nTry closing this window and refreshing the list.");
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
        private void stationWindow_Closing(object sender, CancelEventArgs e)
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
