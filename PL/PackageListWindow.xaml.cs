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
    /// Interaction logic for PackageListWindow.xaml
    /// </summary>
    public partial class PackageListWindow : Window
    {
        #region Fields

        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// Flag if the window was closed properly.
        /// </summary>
        private bool allowClose;

        /// <summary>
        /// The packages being displayed.
        /// </summary>
        private ObservableCollection<BO.PackageToList> packageToListCollection;

        #endregion

        #region Constructors

        /// <summary>
        /// PackageListWindow constructor, initializes ItemSources.
        /// </summary>
        /// <param name="bl">A BL object.</param>
        public PackageListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            allowClose = false;
            packageToListCollection = new ObservableCollection<BO.PackageToList>(bl.GetPackagesList());

            DataContext = packageToListCollection;

            statusSelector.ItemsSource = Enum.GetValues(typeof(BO.Enums.PackageStatus));

            //ensure MaxWeightSelector.ItemsSource does not include "free"
            List<BO.Enums.WeightCategories> weights = new((BO.Enums.WeightCategories[])Enum.GetValues(typeof(BO.Enums.WeightCategories)));
            weights.Remove(BO.Enums.WeightCategories.free);
            weightSelector.ItemsSource = weights;

            prioritySelector.ItemsSource = Enum.GetValues(typeof(BO.Enums.Priorities));
        }

        #endregion

        #region Filter ListView

        /// <summary>
        /// Filter the packageListView based on the selected options of all selectors.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(packageListView.ItemsSource);
            Predicate<object> defaultPred = (object package) => { return true; };
            Predicate<object> statusPred = (object package) => { return (package as BO.PackageToList).Status == (BO.Enums.PackageStatus)statusSelector.SelectedItem; };
            Predicate<object> weightPred = (object package) => { return (package as BO.PackageToList).Weight == (BO.Enums.WeightCategories)weightSelector.SelectedItem; };
            Predicate<object> priorityPred = (object package) => { return (package as BO.PackageToList).Priority == (BO.Enums.Priorities)prioritySelector.SelectedItem; };

            if (statusSelector.SelectedItem == null && weightSelector.SelectedItem == null && prioritySelector.SelectedItem == null)
            {
                view.Filter = defaultPred;
            }
            else if (weightSelector.SelectedItem == null && prioritySelector.SelectedItem == null)
            {
                view.Filter = statusPred;
            }
            else if (statusSelector.SelectedItem == null && prioritySelector.SelectedItem == null)
            {
                view.Filter = weightPred;
            }
            else if (statusSelector.SelectedItem == null && weightSelector.SelectedItem == null)
            {
                view.Filter = priorityPred;
            }
            else if (prioritySelector.SelectedItem == null)
            {
                view.Filter = (object package) => statusPred(package) && weightPred(package);
            }
            else if (weightSelector.SelectedItem == null)
            {
                view.Filter = (object package) => statusPred(package) && priorityPred(package);
            }
            else if (statusSelector.SelectedItem == null)
            {
                view.Filter = (object package) => weightPred(package) && priorityPred(package);
            }
            else
            {
                view.Filter = (object package) => statusPred(package) && weightPred(package) && priorityPred(package);
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
        /// Clear the filter of weightSelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearWeightSelectorButton_Click(object sender, RoutedEventArgs e)
        {
            weightSelector.SelectedItem = null;
        }

        /// <summary>
        /// Clear the filter of prioritySelector.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearPrioritySelectorButton_Click(object sender, RoutedEventArgs e)
        {
            prioritySelector.SelectedItem = null;
        }

        /// <summary>
        /// Group the packages in packageListView by sender.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupBySenderRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(packageListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("SenderName");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// Group the packages in packageListView by receiver.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void groupByReceiverRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(packageListView.ItemsSource);
            view.GroupDescriptions.Clear();
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ReceiverName");
            view.GroupDescriptions.Add(groupDescription);
        }

        /// <summary>
        /// Revert the packageListView to its default state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearRadioButtons_Click(object sender, RoutedEventArgs e)
        {
            groupBySenderRadioButton.IsChecked = false;
            groupByReceiverRadioButton.IsChecked = false;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(packageListView.ItemsSource);
            view.GroupDescriptions.Clear();
        }

        #endregion

        #region Add

        /// <summary>
        /// Open a PackageWindow to add a package to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addPackageButton_Click(object sender, RoutedEventArgs e)
        {
            new PackageWindow(bl).Show();
        }

        #endregion

        #region Action

        /// <summary>
        /// Open a PackageWindow to perform actions with a package double-clicked in packageListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void packageListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //make sure that a package was double-clicked (not just anywhere on the window)
            BO.PackageToList package = ((FrameworkElement)e.OriginalSource).DataContext as BO.PackageToList;
            if (package != null)
            {
                new PackageWindow(bl, package).Show();
            }
        }

        #endregion

        #region Refresh

        /// <summary>
        /// Refresh the packageListView to reflect any updates.
        /// </summary>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            bool? groupedBySender = groupBySenderRadioButton.IsChecked;
            bool? groupedByReceiver = groupByReceiverRadioButton.IsChecked;
            BO.Enums.PackageStatus? packageStatus = (BO.Enums.PackageStatus?)statusSelector.SelectedItem;
            BO.Enums.WeightCategories? weightCategories = (BO.Enums.WeightCategories?)weightSelector.SelectedItem;
            BO.Enums.Priorities? priorities = (BO.Enums.Priorities?)prioritySelector.SelectedItem;

            packageToListCollection.Clear();
            foreach (BO.PackageToList packageToList in bl.GetPackagesList())
            {
                packageToListCollection.Add(packageToList);
            }

            statusSelector.SelectedItem = packageStatus;
            weightSelector.SelectedItem = weightCategories;
            prioritySelector.SelectedItem = priorities;
            groupBySenderRadioButton.IsChecked = groupedBySender;
            groupByReceiverRadioButton.IsChecked = groupedByReceiver;
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
        private void packageListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Close button on the lower right.");
            }
        }

        #endregion
    }
}
