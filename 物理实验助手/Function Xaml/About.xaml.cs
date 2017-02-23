using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 物理实验助手.Function_Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class About : Page
    {
        public About()
        {
            this.InitializeComponent();
            // 页面的缓存
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        #region 导航按钮

        private void Bamburg(object sender, RoutedEventArgs e)
        {
            this.mySplitView.IsPaneOpen = !this.mySplitView.IsPaneOpen;
        }

        private void ToHome(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void ToTimer(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Function_Xaml.Timer));
        }

        private void ToLabRecord(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Function_Xaml.LabRecord));
        }

        private void ToCalculator(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Function_Xaml.ScienceCalculator));
        }

        private void ToDataProcess(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Function_Xaml.DataProcess));
        }

        private void ToConstantSearch(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Function_Xaml.ConstantSearch));
        }

        private void ToAbout(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Function_Xaml.About));
        }

        #endregion

    }
}
