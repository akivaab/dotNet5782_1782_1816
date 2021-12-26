﻿using System;
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
        /// Collection of usernames and passwords.
        /// </summary>
        IEnumerable<string> usernames;
        IEnumerable<string> passwords;

        /// <summary>
        /// MainWindow constructor that gets a BL instance.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            bl = BlApi.BlFactory.GetBl();
            usernames = new List<string>();
            passwords = new List<string>();
        }

        /// <summary>
        /// Show the DroneListWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowDroneListButton_Click(object sender, RoutedEventArgs e)
        {
            new DroneListWindow(bl).Show();
        }

        /// <summary>
        /// Verify an employee signing in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void employeeSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (employeeName.Text == "WDGaster" && employeePassword.Password == "WingDings")
            {
                login.Visibility = Visibility.Collapsed;
                employeeView.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Verify a customer signing in.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerSignIn_Click(object sender, RoutedEventArgs e)
        {
            if (usernames.Contains(customerName.Text) && passwords.Contains(employeePassword.Password))
            {
                login.Visibility = Visibility.Collapsed;
                customerView.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Register a new customer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void register_Click(object sender, RoutedEventArgs e)
        {
            //TODO: open CustomerRegisterWindow to add new customer

            login.Visibility = Visibility.Collapsed;
            customerView.Visibility = Visibility.Visible;
        }
    }
}
