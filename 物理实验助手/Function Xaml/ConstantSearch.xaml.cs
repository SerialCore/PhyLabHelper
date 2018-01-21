using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Store;
using Windows.Services.Store;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 物理实验助手.Function_Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConstantSearch : Page
    {
        public ConstantSearch()
        {
            this.InitializeComponent();
            // 页面的缓存
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #region 网页链接导航

        private void bing(object sender, RoutedEventArgs e)
        {
            myWebView.Source = new Uri("http://blamder.github.io/Phy-MyLab/");
            Urlbox.Text = "http://blamder.github.io/Phy-MyLab/";
        }

        private void kexuewang(object sender, RoutedEventArgs e)
        {
            myWebView.Source = new Uri("http://www.sciencenet.cn/");
            Urlbox.Text = "http://www.sciencenet.cn/";
        }

        private void nature(object sender, RoutedEventArgs e)
        {
            myWebView.Source = new Uri("http://www.nature.com/");
            Urlbox.Text = "http://www.nature.com/";
        }

        private void ZKY(object sender, RoutedEventArgs e)
        {
            myWebView.Source = new Uri("http://www.csdb.cn/");
            Urlbox.Text = "http://www.csdb.cn/";
        }

        #endregion

        #region 浏览器操作

        private void back(object sender, RoutedEventArgs e)
        {
            try
            {
                myWebView.GoBack();
                Urlbox.Text = myWebView.Source.ToString();
            }
            catch
            {
                ;
            }
        }

        private void forward(object sender, RoutedEventArgs e)
        {
            try
            {
                myWebView.GoForward();
                Urlbox.Text = myWebView.Source.ToString();
            }
            catch
            {
                ;
            }
        }

        private void select(object sender, RoutedEventArgs e)
        {
            if (Urlbox.Text != "")
                myWebView.Source = new Uri(Urlbox.Text);
        }

        private void myWebView_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            Urlbox.Text = myWebView.Source.ToString();
            waitPage.IsActive = true;
        }

        private void myWebView_LoadCompleted(object sender, NavigationEventArgs e)
        {
            waitPage.IsActive = false;
        }

        #endregion
    }
}
