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
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        BlApi.IBL bl;

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

        /// <summary>
        /// Show the DroneListWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showDroneListButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneListWindow(bl).Show();
        }

        private void showStationListButton_Click(object sender, RoutedEventArgs e)
        {
            new StationListWindow(bl).Show();
        }

        private void showCustomerListButton_Click(object sender, RoutedEventArgs e)
        {
            new CustomerListWindow(bl).Show();
        }

        private void showPackageListButton_Click(object sender, RoutedEventArgs e)
        {
            new PackageListWindow(bl).Show();
        }

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
                //login.Visibility = Visibility.Collapsed;
                //customerGrid.Visibility = Visibility.Visible;
                openCustomerGrid(sender, e);
            }
        }

        /// <summary>
        /// Register a new customer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void register_Click(object sender, RoutedEventArgs e)
        {
            CustomerWindow registration = new CustomerWindow(bl);
            registration.Show();
            registration.Closed += new EventHandler(openCustomerGrid);
        }

        private void openCustomerGrid(object sender, EventArgs e)
        {
            login.Visibility = Visibility.Collapsed;
            customerGrid.Visibility = Visibility.Visible;
            
            sentPackagesNotCollected.ItemsSource = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.SenderID.ToString() == customerID.Text && p.Collected == null));
            sentPackagesCollected.ItemsSource = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.SenderID.ToString() == customerID.Text && p.Collected != null));
            incomingPackagesNotReceived.ItemsSource = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.ReceiverID.ToString() == customerID.Text && p.Delivered == null));
            incomingPackagesReceived.ItemsSource = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.ReceiverID.ToString() == customerID.Text && p.Delivered != null));
        }

        private void requestPackage_Click(object sender, RoutedEventArgs e)
        {
            new PackageWindow(bl).Show();
        }

        private void seeSend_Click(object sender, RoutedEventArgs e)
        {
            incomingPackagesNotReceived.Visibility = Visibility.Collapsed;
            incomingPackagesReceived.Visibility = Visibility.Collapsed;
            sentPackagesNotCollected.Visibility = Visibility.Visible;
            sentPackagesCollected.Visibility = Visibility.Visible;
        }

        private void seeReceive_Click(object sender, RoutedEventArgs e)
        {
            sentPackagesNotCollected.Visibility = Visibility.Collapsed;
            sentPackagesCollected.Visibility = Visibility.Collapsed;
            incomingPackagesNotReceived.Visibility = Visibility.Visible;
            incomingPackagesReceived.Visibility = Visibility.Visible;
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            customerGrid.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            customerID.Text = "";
            customerPassword.Password = "";
        }

        private void updatePassword_Click(object sender, RoutedEventArgs e)
        {
            updatePasswordPopup.IsOpen = true;
        }
    }
}
