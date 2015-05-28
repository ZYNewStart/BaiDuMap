using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AppClient
{
    /// <summary>
    /// MapDisplay.xaml 的交互逻辑
    /// </summary>
    public partial class MapDisplay
    {
        [DllImport("./easyusb_card_std.dll")]
        static extern unsafe int OpenUsb();
        [DllImport("./easyusb_card_std.dll")]
        static extern unsafe int CloseUsb();
        [DllImport("./easyusb_card_std.dll")]
        static extern byte DI_Soft();

        private DispatcherTimer _dataDispatcher;
        private delegate void LoadDataDelegate();
        private List<Point> mapPoints = new List<Point>();
        private byte _pointIndex = 0;
        private byte _initStatus = 0;
        private byte _lightStatus = 0;
        public MapDisplay()
        {
            InitializeComponent();
        }

        private void StartSystem()
        {
            if (_dataDispatcher == null)
            {
                _dataDispatcher = new DispatcherTimer();
                _dataDispatcher.Tick += dataDispatcher_Tick;
            }
            _dataDispatcher.IsEnabled = true;
            _dataDispatcher.Start();
        }

        private void dataDispatcher_Tick(object sender, EventArgs e)
        {
            byte ioIn = 0;
            ioIn = DI_Soft();
            if (mapPoints.Count == 0)
            {
                GetPointsData();
                return;
            }
            if (_pointIndex >= mapPoints.Count)
            {
                _pointIndex = 0;
            }
            byte currentStatus = (byte)(ioIn & 0x01);
            if (currentStatus != _lightStatus)
            {
                if (currentStatus != _initStatus)
                {
                    this.Dispatcher.BeginInvoke((Action)delegate()
                    {
                        WebBrowser.Document.InvokeScript("SetPanoramaPosition", new object[] { mapPoints[_pointIndex].X, mapPoints[_pointIndex++].Y });
                    });
                }
                _lightStatus = currentStatus;
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
                if (result == -1) return;
                StartSystem();
                _initStatus = (byte)(DI_Soft() & 0x01);
                WebBrowser.Document.InvokeScript("GetRoutePoints", new object[] { 114.341089, 22.608342, 114.348706, 22.602237 });
            }
            catch (Exception ex)
            {
                MessageBox.Show("系统打开失败！" + ex.Message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_dataDispatcher != null)
            {
                _dataDispatcher.Stop();
                _dataDispatcher.Tick -= dataDispatcher_Tick;
                _dataDispatcher = null;
            }
            try
            {
                CloseUsb();
            }
            catch(Exception)
            {

            }
        }

        private void LoadData()
        {
            try
            {
                //这里传入x、y的值，调用JavaScript脚本
                this.Dispatcher.Invoke((Action)(() =>
                    WebBrowser.Document.InvokeScript("GetRoutePoints",
                        new object[] {114.341089, 22.608342, 114.348706, 22.602237})));
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
                dl.BeginInvoke(dl.EndInvoke, null);
            }
        }

        private void Window_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                WindowState = WindowState.Minimized;
            }
        }
    }
}
