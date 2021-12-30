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
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;
        
        /// <summary>
        /// Flag if the window was closed properly.
        /// </summary>
        private bool allowClose;

        /// <summary>
        /// The drones being displayed.
        /// </summary>
        private ObservableCollection<BO.DroneToList> droneToListCollection;

        /// <summary>
        /// DroneListWindow constructor, initializes ItemSources.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        public DroneListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            allowClose = false;
            droneToListCollection = new ObservableCollection<BO.DroneToList>(bl.GetDronesList());

            DataContext = droneToListCollection;
            
            statusSelector.ItemsSource = Enum.GetValues(typeof(BO.Enums.DroneStatus));

            //ensure MaxWeightSelector.ItemsSource does not include "free"
            List<BO.Enums.WeightCategories> weights = new((BO.Enums.WeightCategories[])Enum.GetValues(typeof(BO.Enums.WeightCategories)));
            weights.Remove(BO.Enums.WeightCategories.free);
            maxWeightSelector.ItemsSource = weights;
        }

        /// <summary>
        /// Filter the droneViewList based on the selected options of both selectors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (statusSelector.SelectedItem == null && maxWeightSelector.SelectedItem == null)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
                view.Filter = (object o) => { return true; };
            }
            else if (statusSelector.SelectedItem == null)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
                view.Filter = (object drone) => { return (drone as BO.DroneToList).MaxWeight == (BO.Enums.WeightCategories)maxWeightSelector.SelectedItem; };
            }
            else if (maxWeightSelector.SelectedItem == null)
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
                view.Filter = (object drone) => { return (drone as BO.DroneToList).Status == (BO.Enums.DroneStatus)statusSelector.SelectedItem; };
            }
            else
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
                view.Filter = (object drone) => { return (drone as BO.DroneToList).MaxWeight == (BO.Enums.WeightCategories)maxWeightSelector.SelectedItem && (drone as BO.DroneToList).Status == (BO.Enums.DroneStatus)statusSelector.SelectedItem; };
            }
        }

        /// <summary>
        /// Clear the filter of statusSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearStatusSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            statusSelector.SelectedItem = null;
        }

        /// <summary>
        /// Clear the filter of maxWeightSelector.
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
        /// Open a DroneWindow to perform actions with a drone double-clicked in droneListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void droneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //make sure that a drone was double-clicked (not just anywhere on the window)
            BO.DroneToList drone = ((FrameworkElement)e.OriginalSource).DataContext as BO.DroneToList;
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
            allowClose = true;
            Close();
        }

        /// <summary>
        /// Prevent the window from being closed by force via the X button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void droneListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Close button on the lower right.");
            }
        }

        /// <summary>
        /// Group the drones in droneListView by status.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupByStatusCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Status");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// Revert the droneListView to its default state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupByStatusCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(droneListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }

        /// <summary>
        /// Refresh the droneListView to reflect any updates.
        /// </summary>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            bool? grouped = groupByStatusCheckBox.IsChecked;
            BO.Enums.DroneStatus? droneStatus = (BO.Enums.DroneStatus?)statusSelector.SelectedItem;
            BO.Enums.WeightCategories? weightCategories = (BO.Enums.WeightCategories?)maxWeightSelector.SelectedItem;

            droneToListCollection.Clear();
            foreach (BO.DroneToList droneToList in bl.GetDronesList())
            {
                droneToListCollection.Add(droneToList);
            }

            statusSelector.SelectedItem = droneStatus;
            maxWeightSelector.SelectedItem = weightCategories;
            groupByStatusCheckBox.IsChecked = grouped;
        }
    }
}
