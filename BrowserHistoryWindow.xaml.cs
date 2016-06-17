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

namespace LANSupervisor
{
    /// <summary>
    /// Interaction logic for BrowserHistoryWindow.xaml
    /// </summary>
    public partial class BrowserHistoryWindow : Window
    {
        public BrowserHistoryWindow(List<string> URLhistory)
        {
            InitializeComponent();
            Listbox.ItemsSource = URLhistory;
        }
    }
}
