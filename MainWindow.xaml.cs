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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LANSupervisor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public static Socket Server2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        public static SessionWindow sesWindow;
        private BackgroundWorker FirstBackgroundWorker = null;

        private BackgroundWorker FastCommBackgroundWorker = null;

        public static string ConnectionMessage = "CONNECTIONPORT_";
        public static int AvaiblePort;

        public BitmapImage toSet { get; set; }
        DispatcherTimer ReloadImagesTimer = new DispatcherTimer();

        public Thread MainThread;
        public Thread Receive;

        string FastIP = "192.168.0.26";

        public List<MainListObject> mainlist = new List<MainListObject>();


        

        public MainWindow()
        {
            InitializeComponent();

            double WorkHeight = System.Windows.SystemParameters.WorkArea.Height;
            double WorkWidth = System.Windows.SystemParameters.WorkArea.Width;
            this.Left = 0;
            this.Top = 0;
            Application.Current.MainWindow.Resources["AppHeight"] = Convert.ToDouble(WorkHeight);
            Application.Current.MainWindow.Resources["AppWidth"] = Convert.ToDouble(WorkWidth);
            Application.Current.MainWindow.Resources["MainListHeight"] = Convert.ToDouble(WorkHeight * 0.75);
            Application.Current.MainWindow.Resources["MainListWidth"] = Convert.ToDouble(WorkWidth * 0.75);
            Application.Current.MainWindow.Resources["RightPanelHeight"] = Convert.ToDouble(WorkHeight * 0.75);
            Application.Current.MainWindow.Resources["RightPanelWidth"] = Convert.ToDouble(WorkWidth * 0.25 - 40);
            Application.Current.MainWindow.Resources["RightUnitHeight"] = Convert.ToDouble(WorkHeight * 0.75 - 40);
            Application.Current.MainWindow.Resources["RightPanelItemWidth"] = Convert.ToDouble(WorkWidth * 0.2 - 10);
            Application.Current.MainWindow.Resources["RightPanelTextWidth"] = Convert.ToDouble(WorkWidth * 0.2 - 50);

            this.Height = WorkHeight;
            this.Width = WorkWidth;

            Global.MainWindow = this;

            IPAddress ServerIP = IPAddress.Parse(GetMyIp());
            IPEndPoint ServerIEP = new IPEndPoint(ServerIP, 415);
            Server.Bind(ServerIEP);


            IPAddress ServerIP2 = IPAddress.Parse(GetMyIp());
            IPEndPoint ServerIEP2 = new IPEndPoint(ServerIP2, 12414);
            Server2.Bind(ServerIEP2);
      
      
            AvaiblePort = 500;

            #region stare
            //MainThread = new Thread(() => CheckMessageType());
            //MainThread.Start();

            //ListObject neww = new ListObject();
            //neww.IP = "192.168.1.14";
            //neww.ImageSource = "C:\\Users\\sebb\\Desktop\\LANSupervisor\\LANSupervisor\\screen.png";
            //list.Add(neww);
            //ListObject neww2 = new ListObject();
            //neww2.IP = "192.168.1.5";
            //neww2.ImageSource = "C:\\Users\\sebb\\Desktop\\LANSupervisor\\LANSupervisor\\screen22.png";
            //list.Add(neww2);
            //ListObject neww3 = new ListObject();
            //neww3.IP = "192.168.1.3";
            //neww3.ImageSource = "C:\\Users\\sebb\\Desktop\\LANSupervisor\\LANSupervisor\\screen3.png";
            //list.Add(neww3);
            //ListObject neww4 = new ListObject();
            //neww4.IP = "192.168.1.11";
            //neww4.ImageSource = "C:\\Users\\sebb\\Desktop\\LANSupervisor\\LANSupervisor\\screen4.png";
            //list.Add(neww4);
          
            //AvailableComputers nowy = new AvailableComputers();
            //nowy.IP = "192.168.1.7";
            //list2.Add(nowy);
            //AvailableComputers nowy2 = new AvailableComputers();
            //nowy2.IP = "192.168.1.17";
            //list2.Add(nowy2);
            //AvailableComputers nowy3 = new AvailableComputers();
            //nowy3.IP = "192.168.1.20";
            //list2.Add(nowy3);

            // MainListBox.ItemsSource = list;
            #endregion
         
            if (null == FirstBackgroundWorker)
            {
                FirstBackgroundWorker = new BackgroundWorker();
                FirstBackgroundWorker.DoWork += new DoWorkEventHandler(FirstBackgroundWorker_DoWork);
                FirstBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(FirstBackgroundWorker_ProgressChanged);
                FirstBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FirstBackgroundWorker_RunWorkerCompleted);
                FirstBackgroundWorker.WorkerReportsProgress = true;
                FirstBackgroundWorker.WorkerSupportsCancellation = true;
            }

            FastCommBackgroundWorker = new BackgroundWorker();
            FastCommBackgroundWorker.DoWork += new DoWorkEventHandler(FastCommBackgroundWorker_DoWork);
            FastCommBackgroundWorker.ProgressChanged += new ProgressChangedEventHandler(FastCommBackgroundWorker_ProgressChanged);
            FastCommBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FastCommBackgroundWorker_RunWorkerCompleted);
            FastCommBackgroundWorker.WorkerReportsProgress = true;
            FastCommBackgroundWorker.WorkerSupportsCancellation = true;

            FastCommBackgroundWorker.RunWorkerAsync();
            FirstBackgroundWorker.RunWorkerAsync();

            ReloadImagesTimer.Interval = TimeSpan.FromSeconds(3);
            ReloadImagesTimer.Tick += ReloadImagesTimerTick;
            ReloadImagesTimer.Start();

            //List<MainListObject> listt = new List<MainListObject>();
            //MainListObject neww = new MainListObject();
            //neww.IP = "110101010";
            //listt.Add(neww);
            //AvailableComputers.ItemsSource = listt;

        }

        private void FastCommBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            FastCommBackgroundWorker.RunWorkerAsync();
        }

        private void FastCommBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (sesWindow != null)
            {
                BitmapImage image = (BitmapImage)e.UserState;
                sesWindow.SetImage(image);
            }
        }

        private void FastCommBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (mainlist.Exists(a => a.IP == FastIP))
            {
               BitmapImage im = mainlist.Find(a => a.IP == FastIP).Image;
               FastCommBackgroundWorker.ReportProgress(0, im);
            }
        }

        void FirstBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            CheckMessageType();       
        }


        void FirstBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ReceivedObject obj = (ReceivedObject)e.UserState;
            Bitmap a = obj.Image;
            string IP = obj.IP;
            if (mainlist.Exists(aa => aa.IP == IP))
            {
               ((MainListObject)mainlist.Find(b => b.IP == IP)).Image = ToBitmapImage(a);
            }
            else
            {
                MainListObject neww = new MainListObject();
                neww.IP = IP;
                neww.Image = ToBitmapImage(a);
                neww.port = obj.port +1;
                mainlist.Add(neww);
               
            }
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

        void FirstBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // a to odpala go na nowo
            FirstBackgroundWorker.RunWorkerAsync();
        }


        private void ReloadImagesTimerTick(object sender, EventArgs e)
        {        
            List<MainListObject> tempList = new List<MainListObject>();
            List<MainListObject> tempList2 = new List<MainListObject>();
            foreach (MainListObject ob in mainlist)
            {
                if (ob.DisplayedOnMainList == true)
                {
                    tempList.Add(ob);
                }
                else
                {
                    tempList2.Add(ob);
                }
            }
            MainListBox.ItemsSource = null;
            MainListBox.ItemsSource = tempList;

            AvailableComputers.ItemsSource = null;
            AvailableComputers.ItemsSource = tempList2;
        }

  

     
        





        public void CheckMessageType()
        {
            while (true)
            {
                byte[] Info = new byte[20000];
                IPEndPoint RandomClient = new IPEndPoint(IPAddress.Any, 415);
                EndPoint RandomClientEP = (EndPoint)(RandomClient);
                Server.ReceiveFrom(Info, ref RandomClientEP);
                string ReceivedInfo = GetString(Info);
                if (ReceivedInfo.Contains("STARTCONNECTION"))
                {
                    //ODSYLA PORT I OTWIERA TCPLISTENER NA TEN PORT
                    string FirstResponseSTR = "CONNECTIONPORT_" + AvaiblePort.ToString() + "_";
                    byte[] FirstResponse = GetBytes(FirstResponseSTR);
                    Server.SendTo(FirstResponse,RandomClientEP); 
                    
                    string HostIP = RandomClientEP.ToString();
                    string[] IPArray = HostIP.Split(':');
                    
                    Thread ListeningThread = new Thread(()=> OpenListener(AvaiblePort, IPArray[0]));
                    ListeningThread.Start();
                    AvaiblePort++;

                }
                else if (ReceivedInfo.Contains("STOPCONNECTION"))
                {
                    //ZAMYKA WATEK Z TYM TCPLISTENEREM

                }
                else if (ReceivedInfo.Contains("WhoIsMaster"))
                {
                    string FirstResponseSTR = "CONNECTIONPORT_" + AvaiblePort.ToString() + "_" + GetMyIp() + "_";
                    byte[] FirstResponse = GetBytes(FirstResponseSTR);
                    Server.SendTo(FirstResponse, RandomClientEP);

                    string HostIP = RandomClientEP.ToString();
                    string[] IPArray = HostIP.Split(':');

                    Thread ListeningThread = new Thread(() => OpenListener(AvaiblePort, IPArray[0]));
                    ListeningThread.Start();
                    AvaiblePort++;
                }
                else if (ReceivedInfo.Contains("PROCESESS"))
                {
                    List<ProcessItem> ProcessesList = new List<ProcessItem>();
                    string[] processes = ReceivedInfo.Split(';');
                    for (int i = 1; i < processes.Length - 3; i += 2)
                    {
                        ProcessItem neww = new ProcessItem();
                        neww.Name = processes[i];
                        string sizestring = processes[i + 1];
                        neww.Size = sizestring;
                        ProcessesList.Add(neww);
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                       () => Global.SessionWin.SetListBoxSource(ProcessesList)));
                }
                else if (ReceivedInfo.Contains("BROWSER"))
                {
                    string url = "";
                    string ip = "";
                    string[] message = ReceivedInfo.Split(';');
                    ip =  message[1];
                    url = message[2];

                    System.Windows.Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action(
                       () => Global.MainWindow.AddUrl(url, ip)));
                }
            }
        }

        public void AddUrl(string URL, string IP)
        {
            if (mainlist.Exists(a => a.IP == IP))
            {
                mainlist.Find(a => a.IP == IP).URLlist.Add(URL);
                if (Global.SessionWin != null)
                {
                    Global.SessionWin.AddURL(URL, IP);
                }
            }

        }

        public void OpenListener(int Port, string IP)
        {
            Port--;
            TcpListener Listener = new TcpListener(IPAddress.Any, Port);
            TcpClient Client = new TcpClient();
            NetworkStream mainStream;
            Listener.Start();
            Client = Listener.AcceptTcpClient();
            BinaryFormatter binFormatter = new BinaryFormatter();
            while (Client.Connected)
            {
                mainStream = Client.GetStream();
                try
                {
                    Bitmap a = (Bitmap)binFormatter.Deserialize(mainStream);
                    if (a != null)
                    {
                        ReceivedObject neww = new ReceivedObject();
                        neww.Image = a;
                        neww.IP = IP;
                        neww.port = Port;
                        FirstBackgroundWorker.ReportProgress(0, neww);
                    }
                }
                catch
                {
                    mainlist.Remove(mainlist.Find(a=> a.IP == IP));
                    if (Global.SessionWin != null)
                    {
                        if (Global.SessionWin.Client.IP == IP)
                        {
                            MessageBox.Show("Przerwano połaczenie z: " + IP);
                        }
                    }
                    Thread.CurrentThread.Abort();
                    
                }
            }
                       
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }



        public string GetMyIp()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        private void CloseAppButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void StackPanel_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Top = 0;
            this.Left = 0;
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                object item = btn.DataContext;
                if (item != null)
                {
                    ((MainListObject)item).DisplayedOnMainList = true;
                    RefreshLists();
                }
            }
        }

        private void CloseItemOnList_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                object item = btn.DataContext;
                if (item != null)
                {
                    ((MainListObject)item).DisplayedOnMainList = false;
                    RefreshLists();
                }
            }
        }

        private void RefreshLists()
        {
            List<MainListObject> tempList = new List<MainListObject>();
            List<MainListObject> tempList2 = new List<MainListObject>();
            foreach (MainListObject ob in mainlist)
            {
                if (ob.DisplayedOnMainList == true)
                {
                    tempList.Add(ob);
                }
                else
                {
                    tempList2.Add(ob);
                }
            }
            MainListBox.ItemsSource = null;
            MainListBox.ItemsSource = tempList;

            AvailableComputers.ItemsSource = null;
            AvailableComputers.ItemsSource = tempList2;
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainListObject obj = (MainListObject)(sender as System.Windows.Controls.Image).DataContext;
            FastIP = obj.IP;
            SendFastTransmissionRequest(obj.IP);
            SessionWindow NewOne = new SessionWindow((MainListObject)(sender as System.Windows.Controls.Image).DataContext);
            sesWindow = NewOne;
            Global.SessionWin = NewOne;
            NewOne.Show();
        }

        private void SendFastTransmissionRequest(string IP)
        {
            try
            {
                IPAddress IPC = IPAddress.Parse(IP);
                IPEndPoint FastClient = new IPEndPoint(IPC, 12414);
                EndPoint FastClientEP = (EndPoint)(FastClient);
                string MessageSTR = "STARTFASTCONNECTION";
                byte[] MessageToSent = GetBytes(MessageSTR);
                Server2.SendTo(MessageToSent, FastClientEP);

                //Server2.ReceiveFrom( tablicabajtpw FastClientEP)
            }
            catch
            {
            }

        }

        public void StopFastTransmission(string IP)
        {
            try
            {
                    IPAddress IPC = IPAddress.Parse(IP);
                    IPEndPoint FastClient = new IPEndPoint(IPC, 12414);
                    EndPoint FastClientEP = (EndPoint)(FastClient);
                    string MessageSTR = "STOPFASTCONNECTION";
                    byte[] MessageToSent = GetBytes(MessageSTR);
                    Server2.SendTo(MessageToSent, FastClientEP);
            }
            catch (Exception ex)
            {
            }

        }

        public BitmapImage GetBitMap(string IP)
        {
            if (mainlist.Exists(a => a.IP == IP))
                return mainlist.Find(a => a.IP == IP).Image;
            else
                return new BitmapImage();
        }

        private void ShowPrcesess_Click(object sender, RoutedEventArgs e)
        {
            MainListObject obj = (MainListObject)(sender as Button).DataContext;
            try
            {
                IPAddress IPC = IPAddress.Parse(obj.IP);
                IPEndPoint FastClient = new IPEndPoint(IPC, 12414);
                EndPoint FastClientEP = (EndPoint)(FastClient);
                string MessageSTR = "SHOWPROCESESS";
                byte[] MessageToSent = GetBytes(MessageSTR);
                Server2.SendTo(MessageToSent, FastClientEP);
            }
            catch (Exception ex)
            {
            }
        }

        public void GetProcesses(string IP)
        {
            try
            {
                IPAddress IPC = IPAddress.Parse(IP);
                IPEndPoint FastClient = new IPEndPoint(IPC, 12414);
                EndPoint FastClientEP = (EndPoint)(FastClient);
                string MessageSTR = "SHOWPROCESESS";
                byte[] MessageToSent = GetBytes(MessageSTR);
                Server2.SendTo(MessageToSent, FastClientEP);
            }
            catch (Exception ex)
            {
            }
        }

    }

}
