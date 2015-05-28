using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Threading;


namespace ZY.MyControl
{
    /// <summary>
    /// AdvertPicControl.xaml 的交互逻辑 广告图片控件 播放完毕后自我销毁
    /// </summary>
    public partial class AdvertPicControl : UserControl
    {
        #region 加载List数据
        /// <summary>
        /// 当前图片地址播放列表
        /// </summary>
        private static List<string> currentList;

        public static DependencyProperty advertPicList = DependencyProperty.Register("advertPicList", typeof(List<string>), typeof(AdvertPicControl)
            , new PropertyMetadata(new PropertyChangedCallback(loadAdvertPic)));

        public List<string> AdvertPicList
        {
            get { return (List<string>)GetValue(advertPicList); }
            set { SetValue(advertPicList, value); }
        }

        /// <summary>
        /// 图片播放器地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void loadAdvertPic(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AdvertPicControl advertPicControl = (AdvertPicControl)sender;
            if (e.Property == advertPicList)
            {
                advertPicControl.AdvertPicList = (List<string>)e.NewValue;
                currentList = advertPicControl.AdvertPicList;
            }
        }
        #endregion

        #region 加载图片停留时间
        /// <summary>
        /// 当前图片地址播放列表
        /// </summary>
        private static List<int> currentTimeList;

        public static DependencyProperty advertPicStayTime = DependencyProperty.Register("advertPicStayTime", typeof(List<int>), typeof(AdvertPicControl)
            , new PropertyMetadata(new PropertyChangedCallback(loadAdvertStayTime)));

        public List<int> AdvertPicStayTime
        {
            get { return (List<int>)GetValue(advertPicStayTime); }
            set { SetValue(advertPicStayTime, value); }
        }

        /// <summary>
        /// 图片播放器图片停留时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void loadAdvertStayTime(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            AdvertPicControl advertPicControl = (AdvertPicControl)sender;
            if (e.Property == advertPicStayTime)
            {
                advertPicControl.AdvertPicStayTime = (List<int>)e.NewValue;
                currentTimeList = advertPicControl.AdvertPicStayTime;
            }
        }
        #endregion

        #region 注册自定义事件和参数
        public static readonly RoutedEvent AdvertPicPlayStateChangedEvent;

        public class AdvertPicPlayEventArgs : RoutedEventArgs
        {
            public int playState
            {
                get;
                set;
            }

            public int playLength
            {
                get;
                set;
            }

            public int playIndex
            {
                get;
                set;
            }
        }

        static AdvertPicControl()
        {
            AdvertPicPlayStateChangedEvent = EventManager.RegisterRoutedEvent("AdvertPicPlayStateChanged",
                RoutingStrategy.Bubble, typeof(AdvertPicPlayStateChangedHandler), typeof(AdvertPicControl));
        }
        public delegate void AdvertPicPlayStateChangedHandler(object sender, AdvertPicPlayEventArgs e);
        public event AdvertPicPlayStateChangedHandler AdvertPicPlayStateChanged
        {
            add { AddHandler(AdvertPicControl.AdvertPicPlayStateChangedEvent, value); }
            remove { RemoveHandler(AdvertPicControl.AdvertPicPlayStateChangedEvent, value); }
        }
        #endregion


        public AdvertPicControl()
        {
            InitializeComponent();
        }

        DispatcherTimer switchPicTimer = new DispatcherTimer();
        int i = 0;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //默认 1秒切换一张图片
            switchPicTimer.IsEnabled = false;
            switchPicTimer.Tick += SwitchPicEvent;
        }

        /// <summary>
        /// 开始播放
        /// </summary>
        /// <param name="interval">图片切换时间</param>
        public void Play(int interval)
        {
            switchPicTimer.IsEnabled = true;
            switchPicTimer.Interval = new TimeSpan(0,0,0,0,interval);
            switchPicTimer.Start();
            i = 0;
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void Stop()
        {
            switchPicTimer.IsEnabled = false;
            switchPicTimer.Stop();
        }

        /// <summary>
        /// 切换图片事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchPicEvent(object sender, EventArgs e)
        {
            if (null != currentList)
            {
                Console.WriteLine("开始切换~~~");
                if (i < currentList.Count - 1)
                {
                    DoHandlerStop(Image.OpacityProperty, 1, 0, 1, imgAdvertPic, SwitchPic);
                }
                else
                {
                    AdvertPicPlayEventArgs args = new AdvertPicPlayEventArgs();
                    args.RoutedEvent = AdvertPicPlayStateChangedEvent;
                    args.playState = 1;
                    RaiseEvent(args);
                    switchPicTimer.Stop();
                    switchPicTimer.IsEnabled = false;
                }
                if (null != currentTimeList)
                {
                    Thread.Sleep(currentTimeList[i]); //图片停留时间
                }
            }
        }

        /// <summary>
        /// 动画播放完毕切换图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwitchPic(object sender, EventArgs e)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(currentList[i], UriKind.Absolute));
            imgAdvertPic.Stretch = Stretch.Fill;
            imgAdvertPic.Source = bitmap;
            if (i < currentList.Count)
            {
                i++;
            }
        }

        /// <summary>
        /// 动画
        /// </summary>
        /// <param name="dp"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="element"></param>
        /// <param name="complateHander"></param>
        public void DoHandlerStop(DependencyProperty dp, double from, double to, double duration, UIElement element, EventHandler complateHander)
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();//创建双精度动画对象
            doubleAnimation.From = from;
            doubleAnimation.To = to;//设置动画的结束值
            doubleAnimation.Duration = TimeSpan.FromSeconds(duration);//设置动画时间线长度
            doubleAnimation.FillBehavior = FillBehavior.Stop;//设置动画完成后执行的操作 
            doubleAnimation.Completed += complateHander;
            element.BeginAnimation(dp, doubleAnimation);//设置动画应用的属性并启动动画
        }
    }
}
