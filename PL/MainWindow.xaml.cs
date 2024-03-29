﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

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
        private PO.Customer customer;

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
            catch (BO.InstanceInitializationException)
            {
                MessageBox.Show("Error: Initialization of BL has failed.\nTry to restart the system.");
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

        /// <summary>
        /// Logout of the employee interface.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void employeeLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            employeeGrid.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            employeeName.Text = "";
            employeePassword.Password = "";
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
            MessageBox.Show("Note: You will still need to sign in after registering.\n" +
                            "Your default password will match your ID.\n" +
                            "It is strongly advised that you change it after signing in.");
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
            try
            {
                if (isInteger && customerPassword.Password == bl.GetCustomerPassword(id))
                {
                    this.customer = new(bl.GetCustomer(id));
                    DataContext = customer;
                    openCustomerGrid(sender, e);
                }
                else
                {
                    MessageBox.Show("The information supplied is incorrect");
                }
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: This ID does not match any customer in the system.\nPlease try again.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
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

            try
            {
                sentPackagesNotCollectedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.SenderID == customer.ID && p.Collected == null));
                sentPackagesCollectedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.SenderID == customer.ID && p.Collected != null));
                incomingPackagesNotReceivedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.ReceiverID == customer.ID && p.Delivered == null));
                incomingPackagesReceivedCollection = new ObservableCollection<BO.PackageToList>(bl.FindPackages(p => p.ReceiverID == customer.ID && p.Delivered != null));

                sentPackagesNotCollected.ItemsSource = sentPackagesNotCollectedCollection;
                sentPackagesCollected.ItemsSource = sentPackagesCollectedCollection;
                incomingPackagesNotReceived.ItemsSource = incomingPackagesNotReceivedCollection;
                incomingPackagesReceived.ItemsSource = incomingPackagesReceivedCollection;
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Critical Error: Some of your packages are being transacted with nonexistent customers.\nTry restarting the system.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }

        /// <summary>
        /// Update the user profile.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void updateProfileButton_Click(object sender, RoutedEventArgs e)
        {
            int phoneNumber;
            bool isInteger = int.TryParse(customer.Phone, out phoneNumber);

            if (isInteger)
            {
                try
                {
                    bl.UpdateCustomer(customer.ID, customer.Name, customer.Phone);
                    MessageBox.Show("Profile successfully updated.");

                    BO.Customer blCustomer = bl.GetCustomer(customer.ID);
                    customer.Name = blCustomer.Name;
                    customer.Phone = blCustomer.Phone;
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: You are no longer in the system.\nTry logging out and logging back in.");
                }
                catch (BO.IllegalArgumentException)
                {
                    MessageBox.Show("The phone number must be 9 digits long.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
            else
            {
                MessageBox.Show("The phone number must be 9 digits long.");
            }
        }

        /// <summary>
        /// Change the customer's password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            new PopupWindow(bl, customer.ID).Show();
        }

        /// <summary>
        /// Create a new package.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void requestPackageButton_Click(object sender, RoutedEventArgs e)
        {
            PackageWindow packageWindow = new PackageWindow(bl, customer.ID);
            packageWindow.Show();

            try
            {
                //refresh when the PackageWindow closes so the new requested package appears
                packageWindow.Closed += new EventHandler((object sender, EventArgs e) =>
                {
                    sentPackagesNotCollectedCollection.Clear();
                    foreach (BO.PackageToList package in bl.FindPackages(p => p.SenderID == customer.ID && p.Collected == null))
                    {
                        sentPackagesNotCollectedCollection.Add(package);
                    }
                });
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("Error: The customer you are sending the package to is not in the system.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
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
        private void customerLogoutButton_Click(object sender, RoutedEventArgs e)
        {
            customerGrid.Visibility = Visibility.Collapsed;
            login.Visibility = Visibility.Visible;
            customerID.Text = "";
            customerPassword.Password = "";
            seeSend.IsChecked = true;
        }
        #endregion
    }
}
