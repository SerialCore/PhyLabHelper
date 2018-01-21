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

    }
}
