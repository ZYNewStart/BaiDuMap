using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace BaiDuMap
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //这个文件于可执行文件放在同一目录
                webBrowser1.Url = new Uri(Path.Combine(Application.StartupPath, "GoogleMap.htm"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //这里传入x、y的值，调用JavaScript脚本
            if (webBrowser1.Document != null)
                webBrowser1.Document.InvokeScript("setLocation", new object[] { 121.504, 39.212 });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string lng = "";
            lng = webBrowser1.Document.GetElementById("points").InnerText;
            String[] points = lng.Split(';');
            for (int i = 0; i < points.Length - 1; i++)
            {
                string path = "http://api.map.baidu.com/panorama?width=512&height=256&location=" + points[i] + "&fov=180&ak=OyDszy2RWczbWEyxG5xHVKU5";
                DownloadListImg(i.ToString(), path);
            }
            Console.WriteLine(lng);
        }

        protected string DownloadListImg(string contentId, string serverImgPath)
        {
            long fileLength = 0;
            string fileFullPath = GetDirectoryFullPath();
            string localImgPath = string.Format(@"{0}{1}{2}", fileFullPath, contentId, ".jpg");//serverImgPath.Substring(serverImgPath.LastIndexOf(".")));
            try
            {
                WebRequest webReq = WebRequest.Create(serverImgPath);
                WebResponse webRes = webReq.GetResponse();
                fileLength = webRes.ContentLength;

                Stream srm = webRes.GetResponseStream();
                StreamReader srmReader = new StreamReader(srm);
                byte[] bufferbyte = new byte[fileLength];
                int allByte = (int)bufferbyte.Length;
                int startByte = 0;
                while (fileLength > 0)
                {
                    int downByte = srm.Read(bufferbyte, startByte, allByte);
                    if (downByte == 0) { break; };
                    startByte += downByte;
                    allByte -= downByte;
                    //float part = (float)startByte / 1024;
                    //float total = (float)bufferbyte.Length / 1024;
                    //int percent = Convert.ToInt32((part / total) * 100);
                }

                FileStream fs = new FileStream(localImgPath, FileMode.OpenOrCreate, FileAccess.Write);
                fs.Write(bufferbyte, 0, bufferbyte.Length);
                srm.Close();
                srmReader.Close();
                fs.Close();
                return localImgPath;
            }
            catch
            {
                return null;
            }
        }
        private string GetDirectoryFullPath()
        {
            string currentDomain = AppDomain.CurrentDomain.BaseDirectory.ToString();
            string filePath = string.Format(@"Upload\ListImage\{0}\{1}\", DateTime.Now.Year, DateTime.Now.Month);
            string fileFullPath = currentDomain + filePath;
            if (!Directory.Exists(fileFullPath))
            {
                Directory.CreateDirectory(fileFullPath);
            }
            return fileFullPath;
        }
    }
}
