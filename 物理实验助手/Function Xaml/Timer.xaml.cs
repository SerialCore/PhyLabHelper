using System;
using Windows.System.Display;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 物理实验助手.Function_Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Timer : Page
    {
        public Timer()
        {
            this.InitializeComponent();
            // 页面的缓存
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        // 不会锁屏
        private readonly DisplayRequest _displayRequest = new DisplayRequest();

        ThreadPoolTimer PeriodicTimer;
        long h, m, s, hs;
        int n = 0;     //采集数据的个数
        int control = 0;   //用户使用秒表的行为

        #region 导航按钮

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (control == 1)
                PeriodicTimer.Cancel();
        }

        private void place_on_file(object sender, RoutedEventArgs e)
        {
            if (control == 1)
                PeriodicTimer.Cancel();
            this.Frame.Navigate(typeof(Function_Xaml.LabRecord), "\t\n" + "秒表数据：" + "\t\n" + timerecord.Text);
        }

        private void calculate(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Function_Xaml.DataProcess), timerecord.Text + "timer_data");
        }

        #endregion

        #region 秒表操作

        private void refresh(object sender, RoutedEventArgs e)
        {
            timestring.Text = "00:00:00.00";
            timerecord.Text = "";
            n = 0;
        }

        private void start(object sender, RoutedEventArgs e)
        {
            control = 1;
            DateTime StartTime = DateTime.Now;
            TimeSpan period = TimeSpan.FromMilliseconds(10);
            PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                long ticks = DateTime.Now.Ticks - StartTime.Ticks;
                ticks = ticks / 100000;
                hs = ticks % 100;
                ticks = ticks / 100;
                s = ticks % 60;
                ticks = ticks / 60;
                m = ticks % 60;
                ticks = ticks / 60;
                h = ticks;

                await Dispatcher.RunAsync(CoreDispatcherPriority.High,
                    () =>
                    {
                        timestring.Text = string.Format("{0:00}:{1:00}:{2:00}.{3:00}", h, m, s, hs);
                    });
            }, period);

            // 不会锁屏
            _displayRequest.RequestActive();
        }

        private void end(object sender, RoutedEventArgs e)
        {
            PeriodicTimer.Cancel();
            if (n < 20)
            {
                timerecord.Text += (h * 3600 + m * 60 + s + (decimal)hs / 100).ToString("F2") + "s" + "\t\n";
            }
            n++;

            // 会锁屏(测试未成功)
            _displayRequest.RequestRelease();
        }

        #endregion
    }
}
