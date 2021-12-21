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
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        private BlApi.IBL bl;
        
        //flag if Close button is clicked 
        private bool closeButtonClicked;

        /// <summary>
        /// DroneListWindow constructor, initializes ItemSources.
        /// </summary>
        /// <param name="bl">BL object</param>
        public DroneListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            closeButtonClicked = false;

            DroneListView.ItemsSource = bl.DisplayAllDrones();
            StatusSelector.ItemsSource = Enum.GetValues(typeof(Enums.DroneStatus));

            //ensure MaxWeightSelector.ItemsSource does not include "free"
            List<Enums.WeightCategories> weights = new((Enums.WeightCategories[])Enum.GetValues(typeof(Enums.WeightCategories)));
            weights.Remove(Enums.WeightCategories.free);
            MaxWeightSelector.ItemsSource = weights;
        }

        /// <summary>
        /// Change the selected option of StatusSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector_SelectionChanged(sender, e);
        }

        /// <summary>
        /// Change the selected option of MaxWeightSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaxWeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Selector_SelectionChanged(sender, e);
        }

        /// <summary>
        /// Filter the DroneViewList based on the selected options of both selectors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusSelector.SelectedItem == null && MaxWeightSelector.SelectedItem == null)
            {
                DroneListView.ItemsSource = bl.DisplayAllDrones();
            }
            else if (StatusSelector.SelectedItem == null)
            {
                DroneListView.ItemsSource = bl.FindDrones(d => d.MaxWeight == (Enums.WeightCategories)MaxWeightSelector.SelectedItem);
            }
            else if (MaxWeightSelector.SelectedItem == null)
            {
                DroneListView.ItemsSource = bl.FindDrones(d => d.Status == (Enums.DroneStatus)StatusSelector.SelectedItem);
            }
            else
            {
                DroneListView.ItemsSource = bl.FindDrones(d => d.Status == (Enums.DroneStatus)StatusSelector.SelectedItem && d.MaxWeight == (Enums.WeightCategories)MaxWeightSelector.SelectedItem);
            }
        }

        /// <summary>
        /// Clear the filter of StatusSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearStatusSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            StatusSelector.SelectedItem = null;
        }

        /// <summary>
        /// Clear the filter of MaxWeightSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearMaxWeightSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            MaxWeightSelector.SelectedItem = null;
        }
        
        /// <summary>
        /// Open a DroneWindow to add a drone to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddDroneButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
        }

        /// <summary>
        /// Open a DroneWindow to perform actions with a drone double-clicked in DroneViewList.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
        private void DroneListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!closeButtonClicked)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Close button on the lower right.");
            }
        }
    }
}
