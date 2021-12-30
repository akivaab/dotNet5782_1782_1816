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
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The station we enabling the user to update.
        /// </summary>
        private BO.StationToList station;

        /// <summary>
        /// Flag if the close button is clicked.
        /// </summary>
        private bool allowClose;

        public StationWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            allowClose = false;

            //make only the features needed for adding a station visible in the window. 
            add.Visibility = Visibility.Visible;
            actions.Visibility = Visibility.Collapsed;
        }

        public StationWindow(BlApi.IBL bl, BO.StationToList station)
        {
            InitializeComponent();
            this.bl = bl;
            this.station = station;
            allowClose = false;

            DataContext = station;

            //make only the features needed for perfroming actions on a station visible in the window. 
            add.Visibility = Visibility.Collapsed;
            actions.Visibility = Visibility.Visible;

            loadStationData();
        }

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

            if (isInteger1 && isInteger2 && isInteger3 && isDouble1 && isDouble2 && id > 0 && numAvailableChargeSlots > 0)
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
                    (isInteger1 || isInteger2 || isInteger3 ? "" : "\n(Are the ID, name, and number of charging slots all numbers?)") +
                    (isDouble1 || isDouble2 ? "" : "\n(Are the latitude/longitude of the location floating point values?)"));
            }
        }

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
                    bl.UpdateStation(station.ID, name, numTotalChargeSlots);
                    MessageBox.Show("Station successfully updated.");
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
        }

        private void loadStationData()
        {
            try
            {
                BO.Station stationEntity = bl.GetStation(station.ID);

                actions_StationID.Content = stationEntity.ID;
                actions_Name.Text = stationEntity.Name.ToString();
                actions_Location.Content = stationEntity.Location;
                actions_AvailableChargeSlots.Content = stationEntity.AvailableChargeSlots;

                string presentDrones = "";
                foreach (BO.DroneCharging droneCharging in stationEntity.DronesCharging)
                {
                    presentDrones += droneCharging + "\n";
                }
                actions_DronesCharging.Content = presentDrones;

                actions_TotalChargeSlots.Text = (station.NumAvailableChargeSlots + station.NumOccupiedChargeSlots).ToString();
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This station is not in the system.\nTry closing this window and refreshing the list.");
            }
        }

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
    }
}
