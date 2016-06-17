using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LANSupervisor
{
    public class MainListObject
    {
        public string IP { get; set; }
        public BitmapImage Image { get; set; }
        public bool DisplayedOnMainList = false;
        public int port { get; set; }
        public List<string> URLlist = new List<string>();
    }

    public class ReceivedObject
    {
        public string IP { get; set; }
        public Bitmap Image { get; set; }
        public int port { get; set; }
    }

    public class Global
    {
        public static MainWindow MainWindow { get; set; }
        public static ProcessWindow ProcessWin { get; set; }
        public static SessionWindow SessionWin { get; set; }
    }

    public class ProcessItem
    {
        public string Name { get; set; }
        public string Size { get; set; }
    }

}
