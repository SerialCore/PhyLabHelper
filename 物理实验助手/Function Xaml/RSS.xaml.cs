using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Syndication;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace 物理实验助手.Function_Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RSS : Page
    {
        public ObservableCollection<RssSource> rssSource { get; set; }

        public RSS()
        {
            this.InitializeComponent();
            // 页面的缓存
            this.NavigationCacheMode = NavigationCacheMode.Required;
            // ListView必需的代码
            rssSource = new ObservableCollection<RssSource>();
            this.DataContext = this;
            Button_Click(null, null);

            rssSource.Add(new RssSource { SourceName = "科学网论文-数理科学", SourceUrl = "http://www.sciencenet.cn/xml/paper.aspx?di=7" });
            rssSource.Add(new RssSource { SourceName = "科学网论文-信息科学", SourceUrl = "http://www.sciencenet.cn/xml/paper.aspx?di=6" });
            rssSource.Add(new RssSource { SourceName = "科学网论文-工程材料", SourceUrl = "http://www.sciencenet.cn/xml/paper.aspx?di=5" });
            rssSource.Add(new RssSource { SourceName = "科学网博客-科研笔记", SourceUrl = "http://www.sciencenet.cn/xml/blog.aspx?di=1" });
            rssSource.Add(new RssSource { SourceName = "科学网新闻-信息科学", SourceUrl = "http://www.sciencenet.cn/xml/field.aspx?di=9" });
            rssSource.Add(new RssSource { SourceName = "科学网新闻-工程技术", SourceUrl = "http://www.sciencenet.cn/xml/field.aspx?di=8" });
            rssSource.Add(new RssSource { SourceName = "科学网新闻-基础科学", SourceUrl = "http://www.sciencenet.cn/xml/field.aspx?di=7" });
            rssSource.Add(new RssSource { SourceName = "科学网新闻-前沿交叉", SourceUrl = "http://www.sciencenet.cn/xml/field.aspx?di=4" });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (rssURL.Text != "")
            {
                RssService.GetRssItems(
                     rssURL.Text,
                   async (items) =>
                   {
                       await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           listbox.ItemsSource = items;
                       });
                   },
                   async (exception) =>
                   {
                       await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                       {
                           rssURL.Text = exception;
                       });

                   },
                    null
                    );
            }
            else
            {
                rssURL.Text = "请输入RSS地址";
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listbox.SelectedItem == null)
                return;
            var template = (RssItem)listbox.SelectedItem;
            Frame.Navigate(typeof(Function_Xaml.DetailPage), template);
            listbox.SelectedItem = null;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.RSS_Source.IsPaneOpen = !this.RSS_Source.IsPaneOpen;
        }

        private void rss_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RssSource;
            rssURL.Text = item.SourceUrl;
            this.RSS_Source.IsPaneOpen = !this.RSS_Source.IsPaneOpen;
            Button_Click(null, null);
        }
    }

    // 收集RSS源
    public class RssSource
    {
        public RssSource()
        {

        }

        public string SourceName { get; set; }

        public string SourceUrl { get; set; }
    }

    // 显示RSS的信息，可扩展
    public class RssItem
    {
        public RssItem(string title, string summary, string publishedDate, string url)
        {
            Title = title;
            Summary = summary;
            PublishedDate = publishedDate;
            Url = url;
            PlainSummary = WebUtility.HtmlDecode(Regex.Replace(summary, "<[^>]+?>", ""));
        }

        public string Title { get; set; }

        public string Summary { get; set; }

        public string PublishedDate { get; set; }

        public string Url { get; set; }

        public string PlainSummary { get; set; }

    }

    // 获取RSS信息
    public static class RssService
    {

        public static void GetRssItems(string rssFeed, Action<IEnumerable<RssItem>> onGetRssItemsCompleted = null, Action<string> onError = null, Action onFinally = null)
        {
            var request = HttpWebRequest.Create(rssFeed);
            request.Method = "GET";

            request.BeginGetResponse((result) =>
            {
                try
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)result.AsyncState;
                    WebResponse webResponse = httpWebRequest.EndGetResponse(result);
                    using (Stream stream = webResponse.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string content = reader.ReadToEnd();
                            List<RssItem> rssItems = new List<RssItem>();
                            SyndicationFeed feeds = new SyndicationFeed();
                            feeds.Load(content);
                            foreach (SyndicationItem f in feeds.Items)
                            {
                                RssItem rssItem = new RssItem(f.Title.Text, f.Summary.Text, f.PublishedDate.ToString(), f.Links[0].Uri.AbsoluteUri);
                                rssItems.Add(rssItem);
                            }
                            if (onGetRssItemsCompleted != null)
                            {
                                onGetRssItemsCompleted(rssItems);
                            }
                        }

                    }
                }
                catch (WebException webEx)
                {
                    string exceptionInfo = "";
                    switch (webEx.Status)
                    {
                        case WebExceptionStatus.ConnectFailure:
                            exceptionInfo = "ConnectFailure:远程服务器连接失败。";
                            break;
                        case WebExceptionStatus.MessageLengthLimitExceeded:
                            exceptionInfo = "MessageLengthLimitExceeded:网路请求的消息长度受到限制。";
                            break;
                        case WebExceptionStatus.Pending:
                            exceptionInfo = "Pending:内部异步请求挂起。";
                            break;
                        case WebExceptionStatus.RequestCanceled:
                            exceptionInfo = "RequestCanceled:该请求将被取消。";
                            break;
                        case WebExceptionStatus.SendFailure:
                            exceptionInfo = "SendFailure:发送失败，未能将完整请求发送到远程服务器。";
                            break;
                        case WebExceptionStatus.UnknownError:
                            exceptionInfo = "UnknownError:未知错误。";
                            break;
                        case WebExceptionStatus.Success:
                            exceptionInfo = "Success:请求成功。";
                            break;
                        default:
                            exceptionInfo = "未知网络异常。";
                            break;
                    }
                    if (onError != null)
                    {
                        onError(exceptionInfo);
                    }
                }
                catch (Exception e)
                {
                    if (onError != null)
                    {
                        onError("异常：" + e.Message);
                    }
                }
                finally
                {
                    if (onFinally != null)
                    {
                        onFinally();
                    }
                }
            }, request);

        }

    }

}
