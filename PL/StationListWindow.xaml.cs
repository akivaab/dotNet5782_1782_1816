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
        private bool closeButtonClicked;
        public StationListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            closeButtonClicked = false;

            DataContext = new ObservableCollection<BO.StationToList>(bl.GetStationsList());
        }

        private void stationListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //make sure that a station was double-clicked (not just anywhere on the window)
            BO.StationToList station = ((FrameworkElement)e.OriginalSource).DataContext as BO.StationToList;
            if (station != null)
            {
                new StationWindow(bl, station).Show();
            }
        }

        private void addStationButton_Click(object sender, RoutedEventArgs e)
        {
            new StationWindow(bl).Show();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            closeButtonClicked = true;
            Close();
        }

        private void groupByAvailabilityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(stationListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("NumAvailableChargeSlots == 0");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void groupByChargeSlotQuantityCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(stationListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("NumAvailableChargeSlots");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void groupByCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(stationListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }

        private void stationListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeButtonClicked)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Close button on the lower right.");
            }
        }
    }
}
