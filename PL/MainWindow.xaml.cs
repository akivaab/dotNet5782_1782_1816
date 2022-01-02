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
            bl = BlApi.BlFactory.GetBl();
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
                login.Visibility = Visibility.Collapsed;
                customerGrid.Visibility = Visibility.Visible;
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
            registration.Closed += new EventHandler(registration_Closed);
        }

        private void registration_Closed(object sender, EventArgs e)
        {
            login.Visibility = Visibility.Collapsed;
            customerGrid.Visibility = Visibility.Visible;
        }
    }
}
