using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Sensors;
using Windows.Media.Capture;
using Windows.Media.Devices;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.Display;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 物理实验助手.Function_Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LabRecord : Page
    {
        public MediaCapture AppMediaCapture { get; private set; }

        MediaCapture _capture;

        public IReadOnlyList<StorageFile> storageFile { get; set; }

        public StorageFile[] x;

        //当前打开的文件索引，此索引用于storageFile，x，fileSource这些数据结构
        public int fileIndex;

        // 漫游文件夹中的文件集合
        public ObservableCollection<FileSource> fileSource { get; set; }

        // 全局变量，漫游文件夹
        StorageFolder roamingFolder = ApplicationData.Current.RoamingFolder;

        public LabRecord()
        {
            this.InitializeComponent();
            // 页面的缓存
            this.NavigationCacheMode = NavigationCacheMode.Required;
            // ListView必需的代码
            fileSource = new ObservableCollection<FileSource>();
            this.DataContext = this;
        }

        #region 导航操作

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 文本恢复
            try
            {
                Labrecord.Text = await FileIO.ReadTextAsync(await roamingFolder.GetFileAsync("labrecord.txt"));
            }
            catch
            {
                await roamingFolder.CreateFileAsync("labrecord.txt", CreationCollisionOption.ReplaceExisting);
                Labrecord.Text = "此界面打开时首先显示的是默认文件，这就是默认文件。";
            }
            refreshList();
            // 相机调用
            try
            {
                await this.InitializeCapture();
                // 调用相机
                _capture = null;
                _capture = AppMediaCapture;
                this.myCamera.Source = _capture;
                // 不会锁屏
                _displayRequest.RequestActive();
                // 启动预览
                await _capture.StartPreviewAsync();
                // 连续对焦
                var focusControl = _capture.VideoDeviceController.FocusControl;
                if (focusControl.Supported)
                {
                    await focusControl.UnlockAsync();
                    var settings = new FocusSettings { Mode = FocusMode.Continuous, AutoFocusRange = AutoFocusRange.FullRange };
                    focusControl.Configure(settings);
                    await focusControl.FocusAsync();
                }
            }
            catch
            {
                msg.Text += "相机打开错误";
            }
            // 数据归档
            if (e.Parameter is string)
            {
                Labrecord.Text += e.Parameter.ToString();
                save(null, null);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            clear();
        }

        private async Task InitializeCapture()
        {
            // 实例化MediaCapture
            this.AppMediaCapture = new MediaCapture();
            MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
            // 查找摄像头
            DeviceInformationCollection dvcls = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (dvcls.FirstOrDefault(d => d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back) != null)
            {
                DeviceInformation backCamera = dvcls.FirstOrDefault(d => d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Back);
                // 引用后置摄像头的ID
                settings.VideoDeviceId = backCamera.Id;
                // 捕捉视频和音频
                settings.StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo;
                // 初始化MediaCapture对象
                await AppMediaCapture.InitializeAsync(settings);
            }
            else if (dvcls.FirstOrDefault(d => d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front) != null)
            {
                DeviceInformation frontCamera = dvcls.FirstOrDefault(d => d.EnclosureLocation.Panel == Windows.Devices.Enumeration.Panel.Front);
                // 引用前置摄像头的ID
                settings.VideoDeviceId = frontCamera.Id;
                // 捕捉视频和音频
                settings.StreamingCaptureMode = StreamingCaptureMode.AudioAndVideo;
                // 初始化MediaCapture对象
                await AppMediaCapture.InitializeAsync(settings);
            }
            else
            {
                msg.Text = "找不到相机";
            }
        }

        /// <summary>
        /// 清理相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clear()
        {
            if (AppMediaCapture != null)
            {
                AppMediaCapture.Dispose();
                AppMediaCapture = null;
            }
        }

        private void open_list(object sender, RoutedEventArgs e)
        {
            File_list.IsPaneOpen = !File_list.IsPaneOpen;
        }

        #endregion

        #region 文件操作

        private async void refreshList()
        {
            storageFile = await roamingFolder.GetFilesAsync();
            x = storageFile.ToArray<StorageFile>();
            fileSource.Clear();
            for (int i = 0; i < x.Length; i++)
            {
                fileSource.Add(new FileSource { FileName = x[i].DisplayName });
            }
        }

        private async void file_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as FileSource;
            fileName.Text = item.FileName;
            fileIndex = fileSource.IndexOf(item);
            try
            {
                Labrecord.Text = await FileIO.ReadTextAsync(await roamingFolder.GetFileAsync(item.FileName + ".txt"));
            }
            catch
            {
                Labrecord.Text = "";
            }
        }

        private async void save(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = x[fileIndex].DisplayName;
                // 因为索引是同步于x，fileSource和的，所以用哪个来获取文件名都可以,一般选择代码少的
                StorageFile file = await roamingFolder.GetFileAsync(name + ".txt");
                await FileIO.WriteTextAsync(file, Labrecord.Text);
                await new MessageDialog("保存成功").ShowAsync();
            }
            catch (Exception ex)
            {
                Labrecord.Text = Labrecord.Text.Insert(0, ex.Message + "\t\n");
            }
        }

        private async void othersave(object sender, RoutedEventArgs e)
        {
            FileSavePicker picker = new FileSavePicker();
            // 指定文件类型
            picker.FileTypeChoices.Add("实验记录.doc", new String[] { ".doc" });
            picker.FileTypeChoices.Add("实验记录.txt", new String[] { ".txt" });
            // 默认定位到桌面
            picker.SuggestedStartLocation = PickerLocationId.Desktop;
            // 默认文件名
            picker.SuggestedFileName = "物理实验记录" + DateTime.Now.ToString("yyyyMMddHHmmss");
            // 显示选择器
            StorageFile file = await picker.PickSaveFileAsync();
            // 保存文本
            if (file != null)
            {
                await FileIO.WriteTextAsync(file, Labrecord.Text, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
        }

        private void clear(object sender, RoutedEventArgs e)
        {
            Labrecord.Text = "";
        }

        private async void newfile(object sender, RoutedEventArgs e)
        {
            string name = DateTime.Now.ToString("yyyyMMddHHmmss");
            try
            {
                await roamingFolder.CreateFileAsync(name + ".txt", CreationCollisionOption.ReplaceExisting);
                fileName.Text = "";
                Labrecord.Text = "";
                refreshList();
            }
            catch (Exception ex)
            {
                Labrecord.Text = Labrecord.Text.Insert(0, ex.Message + "\t\n");
            }
        }

        private async void deletefile(object sender, RoutedEventArgs e)
        {
            if (x[fileIndex].DisplayName.Equals("labrecord"))
            {
                await new MessageDialog("默认文件不能删除").ShowAsync();
                return;
            }
            try
            {
                // 删除的是实例文件，不是List中的元素，所以要用x，因为这个数组可以直接操作文件。
                await x[fileIndex].DeleteAsync();
                fileName.Text = "";
                Labrecord.Text = "";
                refreshList();
            }
            catch (Exception ex)
            {
                Labrecord.Text = Labrecord.Text.Insert(0, ex.Message + "\t\n");
            }
        }

        private async void rename(object sender, RoutedEventArgs e)
        {
            if (x[fileIndex].DisplayName.Equals("labrecord"))
            {
                await new MessageDialog("默认文件不能重命名").ShowAsync();
                return;
            }
            if (newName.Text == "" || newName.Text.Equals(x[fileIndex].DisplayName))
            {
                return;
            }
            try
            {
                await x[fileIndex].RenameAsync(newName.Text + ".txt");
                refreshList();
            }
            catch (Exception ex)
            {
                Labrecord.Text = Labrecord.Text.Insert(0, ex.Message + "\t\n");
            }
        }

        #endregion

        #region 相机操作

        // 不会锁屏
        private readonly DisplayRequest _displayRequest = new DisplayRequest();

        private void photo_Checked(object sender, RoutedEventArgs e)
        {
            this.StartVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.SuspendVideo.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.CapPhoto.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void video_Checked(object sender, RoutedEventArgs e)
        {
            this.CapPhoto.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.StartVideo.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.SuspendVideo.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private async void CapPhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 获取照片存放目录
                StorageFolder cameraRoll = KnownFolders.PicturesLibrary;
                // 新图像文件名
                string newFilename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                // 创建新文件
                StorageFile newFile = await cameraRoll.CreateFileAsync(newFilename, CreationCollisionOption.ReplaceExisting);
                // 捕捉照片
                await _capture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), newFile);
            }
            catch
            {
                msg.Text += "!";
            }
        }

        private async void StartVideo_Unchecked(object sender, RoutedEventArgs e)
        {
            StartVideo.Content = "开始录制";
            await _capture.StopRecordAsync();
        }

        private async void StartVideo_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                StartVideo.Content = "停止录制";
                // 获取视频目录
                StorageFolder vdfolder = KnownFolders.VideosLibrary;
                // 新文件名
                string newFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp4";
                // 创建新文件
                StorageFile newFile = await vdfolder.CreateFileAsync(newFileName, CreationCollisionOption.ReplaceExisting);
                // 开始录制
                await _capture.StartRecordToStorageFileAsync(MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Auto), newFile);
            }
            catch
            {
                msg.Text += "!";
            }
        }

        private async void SuspendVideo_Checked(object sender, RoutedEventArgs e)
        {
            SuspendVideo.Content = "恢复录制";
            // 暂停录制但不释放资源
            await _capture.PauseRecordAsync(MediaCapturePauseBehavior.RetainHardwareResources);
        }

        private async void SuspendVideo_Unchecked(object sender, RoutedEventArgs e)
        {
            SuspendVideo.Content = "暂停录制";
            await _capture.ResumeRecordAsync();
        }

        private async void resume_preview(object sender, RoutedEventArgs e)
        {
            // 用户离开再回来时，却发现预览卡死了，这让人很不爽
            try
            {
                await this.InitializeCapture();
                // 调用相机
                _capture = null;
                _capture = AppMediaCapture;
                this.myCamera.Source = _capture;
                // 不会锁屏
                _displayRequest.RequestActive();
                // 启动预览
                await _capture.StartPreviewAsync();
                // 连续对焦
                var focusControl = _capture.VideoDeviceController.FocusControl;
                if (focusControl.Supported)
                {
                    await focusControl.UnlockAsync();
                    var settings = new FocusSettings { Mode = FocusMode.Continuous, AutoFocusRange = AutoFocusRange.FullRange };
                    focusControl.Configure(settings);
                    await focusControl.FocusAsync();
                }
            }
            catch
            {
                msg.Text += "相机打开错误";
            }
        }

        #endregion

        private void black_box(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            WebView webview = new WebView();
            webview.Source = new Uri("blackbox:");
        }
    }
}

public class FileSource
{
    public String FileName { get; set; }

}
