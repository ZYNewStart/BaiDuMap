using System;
using System.Configuration;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;

namespace AppClient
{
    /// <summary>
    /// SystemPara.xaml 的交互逻辑
    /// </summary>
    public partial class SystemPara
    {
        /// <summary>
        /// 初始化时的参数设置字符串
        /// </summary>
        private string InitParamString;

        public SystemPara()
        {
            InitializeComponent();
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SPLatTextBox.Text.Trim()) || string.IsNullOrEmpty(SPLngTextBox.Text.Trim())
                    || string.IsNullOrEmpty(EPLatTextBox.Text.Trim()) || string.IsNullOrEmpty(EPLngTextBox.Text.Trim()))
                {
                    MessageBox.Show("输入不能为空！");
                    return;
                }
                var regex = new Regex(@"^(-?\d+)(\.\d+)?$");//匹配正整数 
                if (regex.IsMatch(SPLatTextBox.Text.Trim()) && regex.IsMatch(SPLngTextBox.Text.Trim())
                    && regex.IsMatch(EPLatTextBox.Text.Trim()) && regex.IsMatch(EPLngTextBox.Text.Trim()))
                {

                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["SPLng"].Value = SPLngTextBox.Text.Trim();
                    cfa.AppSettings.Settings["SPLat"].Value = SPLatTextBox.Text.Trim();
                    cfa.AppSettings.Settings["EPLng"].Value = EPLngTextBox.Text.Trim();
                    cfa.AppSettings.Settings["EPLat"].Value = EPLatTextBox.Text.Trim();
                    cfa.AppSettings.Settings["IOIndex"].Value = IoComboBox.SelectedIndex.ToString();
                    cfa.Save(ConfigurationSaveMode.Modified);
                    //if (MessageBox.Show("修改成功！重启软件生效。是否立即重启软件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    if (MessageBox.Show("修改成功！是否立即关闭配置软件？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        this.Close();
                        //string path = Process.GetCurrentProcess().MainModule.FileName;
                        //var p = new Process {StartInfo = {FileName = path}};
                        //p.Start();
                        //Process.GetCurrentProcess().Kill();
                    }
                }
                else
                {
                    MessageBox.Show("请输入浮点数字！");
                }
            }
            catch
            {
                MessageBox.Show("修改失败！");
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SystemPara_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SPLngTextBox.Text = ConfigurationManager.AppSettings["SPLng"];
                SPLatTextBox.Text = ConfigurationManager.AppSettings["SPLat"];
                EPLngTextBox.Text = ConfigurationManager.AppSettings["EPLng"];
                EPLatTextBox.Text = ConfigurationManager.AppSettings["EPLat"];
                int ioIndex;
                bool convertResult = Int32.TryParse(ConfigurationManager.AppSettings["IOIndex"].ToString(),out ioIndex);
                if (convertResult)
                {
                    IoComboBox.SelectedIndex = ioIndex;                
                }
                InitParamString = GetParamString();
            }
            catch (Exception)
            {
                MessageBox.Show("配置文件读取出错！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 获取界面参数字符串值
        /// </summary>
        /// <returns></returns>
        private String GetParamString()
        {
            return SPLngTextBox.Text + SPLatTextBox.Text + EPLngTextBox.Text + EPLatTextBox.Text
                + IoComboBox.SelectedIndex;
        }
    }
}
