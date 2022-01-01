using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for StationListWindow.xaml
    /// </summary>
    public partial class StationListWindow : Window
    {
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// Flag if the close button was clicked.
        /// </summary>
        private bool allowClose;

        /// <summary>
        /// The stations being displayed.
        /// </summary>
        private ObservableCollection<BO.StationToList> stationToListCollection;

        /// <summary>
        /// StationListWindow constructor.
        /// </summary>
        /// <param name="bl"></param>
        public StationListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            allowClose = false;
            stationToListCollection = new ObservableCollection<BO.StationToList>(bl.GetStationsList());

            DataContext = stationToListCollection;
        }

        /// <summary>
        /// Open a StationWindow to add a station to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addStationButton_Click(object sender, RoutedEventArgs e)
        {
            new StationWindow(bl).Show();
        }

        /// <summary>
        /// Open a StationWindow to perform actions with a station double-clicked in stationListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stationListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //make sure that a station was double-clicked (not just anywhere on the window)
            BO.StationToList station = ((FrameworkElement)e.OriginalSource).DataContext as BO.StationToList;
            if (station != null)
            {
                new StationWindow(bl, station).Show();
            }
        }

        /// <summary>
        /// Group the stations in stationListView by number of available charging slots.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupByChargeSlotQuantityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(stationListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("NumAvailableChargeSlots");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// Revert the stationListView to its default state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupByChargeSlotsQuantityCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(stationListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }

        /// <summary>
        /// Refresh the stationListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            stationToListCollection.Clear();
            foreach (BO.StationToList stationToList in bl.GetStationsList())
            {
                stationToListCollection.Add(stationToList);
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
        private void stationListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Close button on the lower right.");
            }
        }
    }
}
