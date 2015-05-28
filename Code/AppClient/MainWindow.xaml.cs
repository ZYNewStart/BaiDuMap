using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZY.MyControl;

namespace AppClient
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        AdvertPicControl advertPic = new AdvertPicControl();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> imageList = GetUserImages();
            advertPic.AdvertPicList = this.GetUserImages();
            grdContent.Children.Add(advertPic);
            advertPic.AdvertPicPlayStateChanged += playStateHandler;
        }


        /// <summary>
        /// 获取当前用户的图片文件夹中的图片(不包含子文件夹)
        /// </summary>
        /// <returns>返回图片路径列表</returns>
        private List<string> GetUserImages()
        {
            List<string> images = new List<string>();
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string path =@"F:\zhouyong\项目\BDMapDevelop\BaiDuMap_Winform嵌入百度地图网页\BaiDuMap\BaiDuMap\bin\Debug\Upload\ListImage\2015\5\";
            //DirectoryInfo dir = new DirectoryInfo(path);
            //FileInfo[] files = GetPicFiles(path, "*.jpg,*.png,*.bmp,*.gif,", SearchOption.TopDirectoryOnly);// dir.GetFiles("*.jpg", SearchOption.AllDirectories);

            //if (files != null)
            //{
            //    foreach (FileInfo file in files)
            //    {
            //        images.Add(file.FullName);
            //    }
            //}
            for (int i = 58; i >= 0; i--)
            {
                images.Add(path + i.ToString() + ".jpg");
            }
            return images;
        }

        public FileInfo[] GetPicFiles(string picPath, string searchPattern, SearchOption searchOption)
        {
            System.Collections.Generic.List<FileInfo> ltList = new List<FileInfo>();
            DirectoryInfo dir = new DirectoryInfo(picPath);
            string[] sPattern = searchPattern.Replace(';', ',').Split(',');
            for (int i = 0; i < sPattern.Length; i++)
            {
                FileInfo[] files = null;
                try
                {
                    files = dir.GetFiles(sPattern[i], searchOption);
                }
                catch (System.Exception ex)
                {
                    files = new FileInfo[] { };
                }

                ltList.AddRange(files);
            }
            return ltList.ToArray();
        }

        private void playStateHandler(object sender, AdvertPicControl.AdvertPicPlayEventArgs args)
        {
            MessageBox.Show("播放完了，触发事件....");
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            advertPic.Play(Convert.ToInt32(tbTime.Text)); //设置默认切换时间
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            advertPic.Stop();
        }
    }
}
