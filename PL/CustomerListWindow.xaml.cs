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
    /// Lógica interna para CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// Flag if the close button was clicked.
        /// </summary>
        private bool allowClose;

        /// <summary>
        /// The customer being displayed.
        /// </summary>
        private ObservableCollection<BO.CustomerToList> customerToListCollection;

        /// <summary>
        /// CustomerListWindow constructor.
        /// </summary>
        /// <param name="bl"></param>
        public CustomerListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
            allowClose = false;
            customerToListCollection = new ObservableCollection<BO.CustomerToList>(bl.GetCustomersList());

            DataContext = customerToListCollection;
        }

        /// <summary>
        /// Open a CustomerWindow to add a customer to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            new CustomerWindow(bl).Show();
        }

        /// <summary>
        /// Open a CustomerWindow to perform actions with a customer double-clicked in customerListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //make sure that a customer was double-clicked (not just anywhere on the window)
            BO.CustomerToList customer = ((FrameworkElement)e.OriginalSource).DataContext as BO.CustomerToList;
            if (customer != null)
            {
                new CustomerWindow(bl, customer).Show();
            }
        }

        /// <summary>
        /// Refresh the customernListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            customerToListCollection.Clear();
            foreach (BO.CustomerToList customerToList in bl.GetCustomersList())
            {
                customerToListCollection.Add(customerToList);
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
        private void customerListWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!allowClose)
            {
                e.Cancel = true;
                MessageBox.Show("Please use the Close button on the lower right.");
            }
        }

    }
}
