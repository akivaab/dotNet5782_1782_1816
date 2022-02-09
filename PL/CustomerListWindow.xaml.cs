using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Internal logic of CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window, IRefreshable
    {
        #region Fields
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// Flag if the close button was clicked.
        /// </summary>
        private bool allowClose = false;

        /// <summary>
        /// The customer being displayed.
        /// </summary>
        private ObservableCollection<BO.CustomerToList> customerToListCollection;
        #endregion

        #region Constructors
        /// <summary>
        /// CustomerListWindow constructor.
        /// </summary>
        /// <param name="bl">Instance of the BL layer.</param>
        public CustomerListWindow(BlApi.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;

            try
            {
                customerToListCollection = new ObservableCollection<BO.CustomerToList>(bl.GetCustomersList());
                DataContext = customerToListCollection;
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }
        #endregion

        #region Add
        /// <summary>
        /// Open a CustomerWindow to add a customer to the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addCustomerButton_Click(object sender, RoutedEventArgs e) => new CustomerWindow(bl).Show();
        #endregion

        #region Action
        /// <summary>
        /// Open a CustomerWindow to perform actions with a customer double-clicked in customerListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void customerListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //make sure that a customer was double-clicked (not just anywhere on the window)
            BO.CustomerToList customer = ((FrameworkElement)e.OriginalSource).DataContext as BO.CustomerToList;
            
            try
            {
                if (customer != null)
                {
                    bl.GetCustomer(customer.ID);
                    new CustomerWindow(bl, customer).Show();
                }
            }
            catch (BO.UndefinedObjectException)
            {
                MessageBox.Show("This customer has been deleted. Please refresh the list.");
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
        }
        #endregion

        #region Refresh
        /// <summary>
        /// Refresh the customerListView.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        public void refresh()
        {
            try
            {
                customerToListCollection.Clear();
                foreach (BO.CustomerToList customerToList in bl.GetCustomersList())
                {
                    customerToListCollection.Add(customerToList);
                }
            }
            catch (BO.XMLFileLoadCreateException)
            {
                MessageBox.Show("An error occured while saving/loading data from an XML file.");
            }
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
        private void customerListWindow_Closing(object sender, CancelEventArgs e)
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
