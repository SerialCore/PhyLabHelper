using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using 物理实验助手.Function_Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace 物理实验助手
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Scenario> Scenarios { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            Scenarios = new ObservableCollection<Scenario>
            {
                new Scenario() { Title = "Feature", Icon = (Symbol)0xE781, ClassType = typeof(Feature) },
                new Scenario() { Title = "RSS", Icon = (Symbol)0xE128, ClassType = typeof(RSS) },
                new Scenario() { Title = "秒表", Icon = (Symbol)0xE121, ClassType = typeof(Timer) },
                new Scenario() { Title = "实验记录", Icon = (Symbol)0xE104, ClassType = typeof(LabRecord) },
                new Scenario() { Title = "科学计算器", Icon = (Symbol)0xE1D0, ClassType = typeof(ScienceCalculator) },
                new Scenario() { Title = "数据处理", Icon = (Symbol)0xE74C, ClassType = typeof(DataProcess) },
                new Scenario() { Title = "参数查询", Icon = (Symbol)0xE1A3, ClassType = typeof(ConstantSearch) },
                new Scenario() { Title = "关于", Icon = (Symbol)0xE946, ClassType = typeof(About) }
            };
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StorageFile file = e.Parameter as StorageFile;
            if (file == null)
            {
                mainFrame.Navigate(typeof(Feature));
            }
            else
            {
                ;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.function.IsPaneOpen = !this.function.IsPaneOpen;
        }

        private void function_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 之所以要用Item是因为要读取的数据是Item的成员
            var item = e.ClickedItem as Scenario;
            mainFrame.SourcePageType = item.ClassType;

            this.function.IsPaneOpen = !this.function.IsPaneOpen;
        }
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Symbol Icon { get; set; }

        public Type ClassType { get; set; }
    }
}
