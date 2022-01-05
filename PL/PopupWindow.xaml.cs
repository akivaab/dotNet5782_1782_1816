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
    /// Interaction logic for PopupWindow.xaml
    /// </summary>
    public partial class PopupWindow : Window
    {
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

        private void changePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (newPassword.Password == newPasswordConfirm.Password)
            {
                try
                {
                    bl.UpdateCustomerPassword(customerID, newPassword.Password);
                    Close();
                }
                catch (BO.IllegalArgumentException ex)
                {
                    MessageBox.Show(ex.Message + "\nPlease choose a password at least 6 characters long.");
                }
                catch (BO.UndefinedObjectException)
                {
                    MessageBox.Show("Error. You do not exist in the system.");
                }
            }
            else
            {
                newPassword.Password = "";
                newPasswordConfirm.Password = "";
                MessageBox.Show("Your confirmation did not match your new password.");
            }
        }

        private void yesButton_Click(object sender, RoutedEventArgs e)
        {
            packages.Remove(package);
            Close();
        }

        private void noButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
