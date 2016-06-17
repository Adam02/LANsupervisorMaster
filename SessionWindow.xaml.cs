using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LANSupervisor
{
    /// <summary>
    /// Interaction logic for SessionWindow.xaml
    /// </summary>
    public partial class SessionWindow : Window
    {
        public MainListObject Client;

        DispatcherTimer ReloadImageTimer = new DispatcherTimer();
        private BackgroundWorker FirstBackgroundWorker = null;
        List<ProcessItem> ProcessesList { get; set; }
      
        public SessionWindow(MainListObject ClientObject)
        {
            InitializeComponent();
            Client = ClientObject;
            Imagee.Source = Client.Image;

            this.Topmost = true;
        }

        public SessionWindow()
        {
            InitializeComponent();
        }
        public static BitmapImage ToBitmapImage(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public void SetImage(BitmapImage im)
        {
            Imagee.Source = im;
            this.UpdateLayout();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Global.MainWindow.StopFastTransmission(Client.IP);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Global.MainWindow.GetProcesses(Client.IP);
        }

        public void SetListBoxSource(List<ProcessItem> list)
        {
            ProcessesList = list;
            ListBox.ItemsSource = ProcessesList;
            this.UpdateLayout();
        }

        public void AddURL(string URL, string IP)
        {
            if (Client.IP == IP)
            {
                Client.URLlist.Add(URL);
            }
        }

        private void ProcessesButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProStackPanel.Visibility == Visibility.Collapsed)
            {
                ProStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                ProStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void SortByName_Click(object sender, RoutedEventArgs e)
        {
            ListBox.ItemsSource = ProcessesList.OrderBy(a => a.Name);
        }

        private void SortByMemory_Click(object sender, RoutedEventArgs e)
        {
            ListBox.ItemsSource = ProcessesList.OrderBy(a => a.Size);
        }

        private void RefreshProcessesButton_Click(object sender, RoutedEventArgs e)
        {
            Global.MainWindow.GetProcesses(Client.IP);
        }

        private void SitesButton_Click(object sender, RoutedEventArgs e)
        {
            BrowserHistoryWindow BHW = new BrowserHistoryWindow(Client.URLlist);
            BHW.Show();
        }

    }
}
