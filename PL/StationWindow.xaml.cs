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
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
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
        /// <param name="bl">A BL object.</param>
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
                station = new(bl.GetStation(stationToList.ID));
                this.bl = bl;
                DataContext = station;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This station is already removed from the system.\nTry closing this window and refreshing the list.");
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
                    MessageBox.Show(ex.Message + "\nThe latitude must be between -1 and 1, the longitude between 0 and 2,\nand there cannot be a negative number of charge slots.");
                }
                catch (BO.NonUniqueIdException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease enter a different ID.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter other information." +
                    (isInteger1 || isInteger2 || isInteger3 ? "" : "\n(Are the ID, name, and number of charging slots all numbers?)") +
                    (isDouble1 || isDouble2 ? "" : "\n(Are the latitude/longitude of the location floating point values?)"));
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

                    reloadStationData();
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: This station is not in the system.\nTry closing this window and refreshing the list.");
                }
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\nTry entering a larger number.");
                }
            }
            else
            {
                MessageBox.Show("Some of the information supplied is invalid. Please enter other information." +
                    "\n(Are the name, and number of charging slots all numbers?)");
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
        }
        #endregion

        #region Reload
        /// <summary>
        /// Reload the data of the station to be displayed in the window.
        /// </summary>
        private void reloadStationData()
        {
            try
            {
                BO.Station station = bl.GetStation(this.station.ID);
                this.station.Name = station.Name;
                this.station.AvailableChargeSlots = station.AvailableChargeSlots;

                actions_TotalChargeSlots.Text = (station.AvailableChargeSlots + station.DronesCharging.Count()).ToString();
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This station is not in the system.\nTry closing this window and refreshing the list.");
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
        private void stationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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
