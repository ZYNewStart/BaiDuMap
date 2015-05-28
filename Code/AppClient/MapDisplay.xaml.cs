using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace AppClient
{
    /// <summary>
    /// MapDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class MapDisplay : Window
    {
        [DllImport("./easyusb_card_std.dll")]
        static extern unsafe int OpenUsb();
        [DllImport("./easyusb_card_std.dll")]
        static extern unsafe int CloseUsb();
        [DllImport("./easyusb_card_std.dll")]
        static extern byte DI_Soft();

        private DispatcherTimer dataDispatcher;
        private delegate void LoadDataDelegate();
        private List<Point> mapPoints = new List<Point>();
        private int pointIndex = 0;
        private int initStatus = 0;
        private int lightStatus = 0;
        public MapDisplay()
        {
            InitializeComponent();
        }

        private void StartSystem()
        {
            if (dataDispatcher == null)
            {
                dataDispatcher = new DispatcherTimer();
                dataDispatcher.Tick += dataDispatcher_Tick;
            }
            dataDispatcher.IsEnabled = true;
            dataDispatcher.Start();
        }

        private void dataDispatcher_Tick(object sender, EventArgs e)
        {
            byte io_in = 0;
            io_in = DI_Soft();
            if (mapPoints.Count == 0)
            {
                GetPointsData();
                return;
            }
            if (pointIndex >= mapPoints.Count)
            {
                pointIndex = 0;
            }
            var currentStatus = io_in & 0x01;
            if (currentStatus != lightStatus)
            {
                if (currentStatus != initStatus)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        WebBrowser.Document.InvokeScript("SetPanoramaPosition", new object[] { mapPoints[pointIndex].X, mapPoints[pointIndex++].Y });
                    });
                }
                lightStatus = currentStatus;
            }
            //I1CheckBox.IsChecked = ((io_in & 0x01) != 0);
            //I2CheckBox.IsChecked = ((io_in & 0x02) != 0);
            //I3CheckBox.IsChecked = ((io_in & 0x04) != 0);
            //I4CheckBox.IsChecked = ((io_in & 0x08) != 0);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int result  = OpenUsb();
                StartSystem();
                initStatus = (DI_Soft() & 0x01);
                WebBrowser.Document.InvokeScript("GetRoutePoints", new object[] { 114.341089, 22.608342, 114.348706, 22.602237 });
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统打开失败！" + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (dataDispatcher != null)
            {
                dataDispatcher.Stop();
                dataDispatcher.Tick -= dataDispatcher_Tick;
                dataDispatcher = null;
            }
            CloseUsb();
        }

        private void LoadData()
        {
            try
            {
                //这里传入x、y的值，调用JavaScript脚本
                this.Dispatcher.Invoke((Action)delegate()
                {
                    WebBrowser.Document.InvokeScript("GetRoutePoints", new object[] { 114.341089, 22.608342, 114.348706, 22.602237 });               
                });
            }
            catch (Exception)
            {
                MessageBox.Show("获取地图数据失败！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetPointsData()
        {
            try
            {
                string lng = "";
                this.Dispatcher.Invoke((Action)delegate()
                {
                    lng = WebBrowser.Document.GetElementById("points").InnerText;
                });
                String[] points = lng.Split(';');
                mapPoints.Clear();
                for (int i = 0; i < points.Length - 1; i++)
                {
                    String[] xy = points[i].Split(',');
                    mapPoints.Add(new Point(Convert.ToDouble(xy[0]), Convert.ToDouble(xy[1])));
                }
            }
            catch (Exception)
            {

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //这个文件于可执行文件放在同一目录
                WebBrowser.Url = new Uri(System.AppDomain.CurrentDomain.BaseDirectory + "MapDisplay.htm");
                WebBrowser.DocumentCompleted += WebBrowser_DocumentCompleted;
                Start_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("地图加载失败！" + ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void WebBrowser_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            if (WebBrowser.ReadyState == System.Windows.Forms.WebBrowserReadyState.Complete)
            {
                LoadDataDelegate dl = LoadData;
                dl.BeginInvoke(ar =>
                {
                    dl.EndInvoke(ar);
                }, null);
            }
        }
    }
}
