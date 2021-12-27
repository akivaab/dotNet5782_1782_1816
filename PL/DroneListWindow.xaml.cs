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
using BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;
        
        /// <summary>
        /// Flag if the close button was clicked.
        /// </summary>
        private bool closeButtonClicked;

        private ObservableCollection<DroneToList> droneToListCollection;

        /// <summary>
        /// DroneListWindow constructor, initializes ItemSources.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        public DroneListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            closeButtonClicked = false;

            droneToListCollection = new ObservableCollection<DroneToList>(bl.GetDronesList());
            DataContext = droneToListCollection;
            
            statusSelector.ItemsSource = Enum.GetValues(typeof(Enums.DroneStatus));

            //ensure MaxWeightSelector.ItemsSource does not include "free"
            List<Enums.WeightCategories> weights = new((Enums.WeightCategories[])Enum.GetValues(typeof(Enums.WeightCategories)));
            weights.Remove(Enums.WeightCategories.free);
            maxWeightSelector.ItemsSource = weights;
        }

        /// <summary>
        /// Filter the DroneViewList based on the selected options of both selectors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statusSelector.SelectedItem == null && maxWeightSelector.SelectedItem == null)
            {
                //droneToListCollection.Clear();
                //AddRange(droneToListCollection, bl.GetDronesList());
                droneListView.ItemsSource = bl.GetDronesList();
            }
            else if (statusSelector.SelectedItem == null)
            {
                //droneToListCollection.Clear();
                //AddRange(droneToListCollection, bl.FindDrones(d => d.MaxWeight == (Enums.WeightCategories)maxWeightSelector.SelectedItem));
                droneListView.ItemsSource = bl.FindDrones(d => d.MaxWeight == (Enums.WeightCategories)maxWeightSelector.SelectedItem);
            }
            else if (maxWeightSelector.SelectedItem == null)
            {
                //droneToListCollection.Clear();
                //AddRange(droneToListCollection, bl.FindDrones(d => d.Status == (Enums.DroneStatus)statusSelector.SelectedItem));
                droneListView.ItemsSource = bl.FindDrones(d => d.Status == (Enums.DroneStatus)statusSelector.SelectedItem);
            }
            else
            {
                //droneToListCollection.Clear();
                //AddRange(droneToListCollection, bl.FindDrones(d => d.Status == (Enums.DroneStatus)statusSelector.SelectedItem && d.MaxWeight == (Enums.WeightCategories)maxWeightSelector.SelectedItem));
                droneListView.ItemsSource = bl.FindDrones(d => d.Status == (Enums.DroneStatus)statusSelector.SelectedItem && d.MaxWeight == (Enums.WeightCategories)maxWeightSelector.SelectedItem);
            }
        }

        /// <summary>
        /// Clear the filter of StatusSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearStatusSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            statusSelector.SelectedItem = null;
        }

        /// <summary>
        /// Clear the filter of MaxWeightSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearMaxWeightSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            maxWeightSelector.SelectedItem = null;
        }
        
        /// <summary>
        /// Open a DroneWindow to add a drone to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addDroneButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
        }

        /// <summary>
        /// Open a DroneWindow to perform actions with a drone double-clicked in DroneViewList.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void droneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //make sure that a drone was double-clicked (not just anywhere on the window)
            DroneToList drone = ((FrameworkElement)e.OriginalSource).DataContext as DroneToList;
            if (drone != null)
            {
                new DroneWindow(bl, drone).Show();
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
        private void droneListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeButtonClicked)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Close button on the lower right.");
            }
        }

        private void groupByStatusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
            view.GroupDescriptions.Add(groupDescription);
        }

        private void groupByStatusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }

        //private void AddRange<T>(ObservableCollection<T> coll, IEnumerable<T> items)
        //{
        //    foreach (var item in items)
        //    {
        //        coll.Add(item);
        //    }
        //}
    }
}
