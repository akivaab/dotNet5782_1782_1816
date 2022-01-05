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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// The customer who signed in (initialized at sign in).
        /// </summary>
        private BO.Customer customer;

        /// <summary>
        /// Collection of packages the customer is sending that have not yet been collected.
        /// </summary>
        ObservableCollection<BO.PackageToList> sentPackagesNotCollectedCollection;

        /// <summary>
        /// Collection of packages the customer is sending that have been collected.
        /// </summary>
        ObservableCollection<BO.PackageToList> sentPackagesCollectedCollection;

        /// <summary>
        /// Collection of packages the customer is receiving that have not yet been delivered.
        /// </summary>
        ObservableCollection<BO.PackageToList> incomingPackagesNotReceivedCollection;

        /// <summary>
        /// Collection of packages the customer is receiving that have been delivered.
        /// </summary>
        ObservableCollection<BO.PackageToList> incomingPackagesReceivedCollection;
        #endregion

        #region Constructors
        /// <summary>
        /// MainWindow constructor that gets a BL instance.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                bl = BlApi.BlFactory.GetBl();
            }
            catch (BO.EmptyListException ex)
            {
                MessageBox.Show(ex.Message + "\nCongratulations, you achieved the near impossible situation where no randomly generated customer received a package yet!");
            }
        }
        #endregion

        #region Employee Interface
        /// <summary>
        /// Verify an employee signing in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void employeeSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (employeeName.Text == "admin" && employeePassword.Password == "123")
            {
                login.Visibility = Visibility.Collapsed;
                employeeGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Show the DroneListWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showDroneListButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneListWindow(bl).Show();
        }

        /// <summary>
        /// Show the StationListWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showStationListButton_Click(object sender, RoutedEventArgs e)
        {
            new StationListWindow(bl).Show();
        }

        /// <summary>
        /// Show the CustomerListWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showCustomerListButton_Click(object sender, RoutedEventArgs e)
        {
            new CustomerListWindow(bl).Show();
        }

        /// <summary>
        /// Show the PackageListWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showPackageListButton_Click(object sender, RoutedEventArgs e)
        {
            new PackageListWindow(bl).Show();
        }
        #endregion

        #region Customer Interface
        /// <summary>
        /// Register a new customer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void register_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl).Show();
            MessageBox.Show("Note: You will still need to sign in after registering.");
        }

        /// <summary>
        /// Verify a customer signing in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerSignIn_Click(object sender, RoutedEventArgs e)
        {
            int id;
            bool isInteger = int.TryParse(customerID.Text, out id);
            if (isInteger && customerPassword.Password == bl.GetCustomerPassword(id))
            {
                this.customer = bl.GetCustomer(id);
                DataContext = customer;
                openCustomerGrid(sender, e);
            }
            else
            {
                MessageBox.Show("The password is incorrect");
            }
        }

        /// <summary>
        /// Set up the window for customer interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openCustomerGrid(object sender, EventArgs e)
        {
            login.Visibility = Visibility.Collapsed;
            customerGrid.Visibility = Visibility.Visible;

            sentPackagesNotCollectedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.SenderID == customer.ID && p.Collected == null));
            sentPackagesCollectedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.SenderID == customer.ID && p.Collected != null));
            incomingPackagesNotReceivedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.ReceiverID == customer.ID && p.Delivered == null));
            incomingPackagesReceivedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.ReceiverID == customer.ID && p.Delivered != null));

            sentPackagesNotCollected.ItemsSource = sentPackagesNotCollectedCollection;
            sentPackagesCollected.ItemsSource = sentPackagesCollectedCollection;
            incomingPackagesNotReceived.ItemsSource = incomingPackagesNotReceivedCollection;
            incomingPackagesReceived.ItemsSource = incomingPackagesReceivedCollection;
        }

        /// <summary>
        /// Change the customer's password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePassword_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(bl, customer.ID).Show();
        }

        /// <summary>
        /// Create a new package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void requestPackage_Click(object sender, RoutedEventArgs e)
        {
            new PackageWindow(bl).Show();
        }

        /// <summary>
        /// Confirm a package was collected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sentPackagesCollected_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.PackageToList package = ((FrameworkElement)e.OriginalSource).DataContext as BO.PackageToList;
            if (package != null)
            {
                new PopupWindow(bl, package, sentPackagesCollectedCollection).Show();
            }
        }

        /// <summary>
        /// Confirm a package was received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void incomingPackagesReceived_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BO.PackageToList package = ((FrameworkElement)e.OriginalSource).DataContext as BO.PackageToList;
            if (package != null)
            {
                new PopupWindow(bl, package, incomingPackagesReceivedCollection).ShowDialog();
            }
        }

        /// <summary>
        /// Customer logout.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            customerGrid.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            customerID.Text = "";
            customerPassword.Password = "";
            seeSend.IsChecked = false;
            seeReceive.IsChecked = false;
        }
        #endregion
    }
}
