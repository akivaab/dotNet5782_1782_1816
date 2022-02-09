using System.Collections.ObjectModel;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
        #region Fields
        /// <summary>
        /// Instance of the BL.
        /// </summary>
        private BlApi.IBL bl;

        /// <summary>
        /// ID of the customer.
        /// </summary>
        private int customerID;

        /// <summary>
        /// The package selected to confirm.
        /// </summary>
        private BO.PackageToList package;

        /// <summary>
        /// The collection of packages that the selected package is part of.
        /// </summary>
        private ObservableCollection<BO.PackageToList> packages;
        #endregion

        #region Constructors
        /// <summary>
        /// PopupWindow constructor, for changing the customer's password.
        /// </summary>
        /// <param name="bl">The BL instance.</param>
        /// <param name="customerID">The ID of the customer.</param>
        public PopupWindow(BlApi.IBL bl, int customerID)
        {
            InitializeComponent();
            this.bl = bl;
            this.customerID = customerID;
            passwordGrid.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// PopupWindow constructor, for confirming a package was collected/received.
        /// </summary>
        /// <param name="bl">The BL instance.</param>
        /// <param name="package">The package being confirmed.</param>
        /// <param name="packages">The collection the selected package is a part of.</param>
        public PopupWindow(BlApi.IBL bl, BO.PackageToList package, ObservableCollection<BO.PackageToList> packages)
        {
            InitializeComponent();
            this.bl = bl;
            this.package = package;
            this.packages = packages;
            confirmPackageGrid.Visibility = Visibility.Visible;
        }
        #endregion

        #region Password
        /// <summary>
        /// Change the customer's password.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (newPassword.Password == newPasswordConfirm.Password)
            {
                try
                {
                    bl.UpdateCustomerPassword(customerID, newPassword.Password);
                    Close();
                }
                catch (BO.IllegalArgumentException)
                {
                    MessageBox.Show("Please choose a password at least 6 characters long for security.");
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error: You do not exist in the system.\nTry logging out and logging back in.");
                }
                catch (BO.XMLFileLoadCreateException)
                {
                    MessageBox.Show("An error occured while saving/loading data from an XML file.");
                }
            }
            else
            {
                newPassword.Password = "";
                newPasswordConfirm.Password = "";
                MessageBox.Show("Your confirmation did not match your new password.");
            }
        }
        #endregion

        #region Confirm Package
        /// <summary>
        /// Confirm that a package was collected/received.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            packages.Remove(package);
            Close();
        }

        /// <summary>
        /// Cancel confirmation that a package was collected/received.
        /// Also used to cancel a password change.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
