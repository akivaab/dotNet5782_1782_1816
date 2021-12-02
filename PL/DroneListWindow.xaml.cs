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
    /// Interaction logic for DroneListWindow.xaml
    /// </summary>
    public partial class DroneListWindow : Window
    {
        private IBL.IBL bl;
        public DroneListWindow(IBL.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;

            DroneListView.ItemsSource = bl.DisplayAllDrones();
            StatusSelector.ItemsSource = Enum.GetValues(typeof(Enums.DroneStatus));

            List<Enums.WeightCategories> weights = new((Enums.WeightCategories[])Enum.GetValues(typeof(Enums.WeightCategories)));
            weights.Remove(Enums.WeightCategories.free);
            MaxWeightSelector.ItemsSource = weights;
        }

        private void StatusSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (StatusSelector.SelectedItem)
            {
                case Enums.DroneStatus.available:
                    DroneListView.ItemsSource = bl.FindDrones(d => d.Status == Enums.DroneStatus.available);
                    break;
                case Enums.DroneStatus.delivery:
                    DroneListView.ItemsSource = bl.FindDrones(d => d.Status == Enums.DroneStatus.delivery);
                    break;
                case Enums.DroneStatus.maintenance:
                    DroneListView.ItemsSource = bl.FindDrones(d => d.Status == Enums.DroneStatus.maintenance);
                    break;
                default:
                    break;
            }
        }

        private void MaxWeightSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (MaxWeightSelector.SelectedItem)
            {
                case Enums.WeightCategories.light:
                    DroneListView.ItemsSource = bl.FindDrones(d => d.MaxWeight == Enums.WeightCategories.light);
                    break;
                case Enums.WeightCategories.medium:
                    DroneListView.ItemsSource = bl.FindDrones(d => d.MaxWeight == Enums.WeightCategories.medium);
                    break;
                case Enums.WeightCategories.heavy:
                    DroneListView.ItemsSource = bl.FindDrones(d => d.MaxWeight == Enums.WeightCategories.heavy);
                    break;
                default:
                    break;
            }
        }

        private void AddDroneButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneWindow(bl).Show();
        }

        private void DroneListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DroneToList drone = ((FrameworkElement)e.OriginalSource).DataContext as DroneToList;
            if (drone != null)
            {
                new DroneWindow(bl, drone).Show();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
