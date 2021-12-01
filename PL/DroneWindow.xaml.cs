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
using System.Windows.Shapes;
using IBL.BO;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        private IBL.IBL bl;
        public DroneWindow(IBL.IBL bl)
        {
            InitializeComponent();
            this.bl = bl;
        }
        public DroneWindow(IBL.IBL bl, DroneToList drone)
        {
            InitializeComponent();
            this.bl = bl;
        }
    }
}
