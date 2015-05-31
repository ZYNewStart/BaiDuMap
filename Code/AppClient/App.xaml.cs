using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AppClient
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary> 
        /// 使用指定路径中的指定文件作为配置文件
        /// </summary> 
        /// <param name="configFileName">配置文件名</param> 
        /// <param name="configFileDirectory">配置文件路径</param> 
        static void SetConfigFile(string configFileName, string configFileDirectory)
        {
            string configFilePath = System.IO.Path.Combine(configFileDirectory, configFileName);
            System.AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configFilePath);
        }

        /// <summary> 
        /// 使用当前路径中的指定文件作为配置文件         
        /// </summary> 
        /// <param name="configFileName">配置文件名</param>         
        static void SetConfigFile(string configFileName)
        {
            string configFilePath = System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory, configFileName);
            System.AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", configFilePath);
        }

        public App()
        {
            SetConfigFile("AppClient.exe.Config");
        }
    }
}
