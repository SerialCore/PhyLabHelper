using System;
using System.Collections.ObjectModel;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 物理实验助手.Function_Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ScienceCalculator : Page
    {
        Function_Class.Calculate mycalculator = new Function_Class.Calculate();
        StringBuilder my_formula = new StringBuilder("");
        // 用于删除字符时的间接量
        string temp;
        // 用于记录输入
        String[] last = new string[50];
        int count = 0;

        public ObservableCollection<Item> Items { get; set; }

        public ScienceCalculator()
        {
            this.InitializeComponent();
            // 页面的缓存
            this.NavigationCacheMode = NavigationCacheMode.Required;
            // ListView必需的代码
            Items = new ObservableCollection<Item>();
            this.DataContext = this;
        }

        #region 导航按钮

        private void Bamburg(object sender, RoutedEventArgs e)
        {
            this.mySplitView.IsPaneOpen = !this.mySplitView.IsPaneOpen;
        }

        private void open_record(object sender, RoutedEventArgs e)
        {
            this.cal_record.IsPaneOpen = !this.cal_record.IsPaneOpen;
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

        private void place_on_file(object sender, RoutedEventArgs e)
        {
            string data = "";
            for (int i = 0; i < Items.Count; i++)
            {
                data += Items[i].Record + "\t\n";
            }
            this.Frame.Navigate(typeof(Function_Xaml.LabRecord), "\t\n" + "科学计算器数据：" + "\t\n" + data);
        }

        #endregion

        #region 计算器操作

        // List被点击
        private void record_list_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 之所以要用Item是因为要读取的数据是Item的成员
            var item = e.ClickedItem as Item;
            // 输入计算式
            my_formula.Append(item.Result);
            result.Text = my_formula.ToString();
            last[count++] = item.Result;
            // 随手关门
            this.cal_record.IsPaneOpen = !this.cal_record.IsPaneOpen;
        }

        private void _scale(object sender, RoutedEventArgs e)
        {
            mycalculator.div = 180m / (decimal)Math.PI;
            mycalculator.multis = 180m / (decimal)Math.PI;
            state.Text = "科学计算器    角度制";
        }

        private void _radian(object sender, RoutedEventArgs e)
        {
            mycalculator.div = 1;
            mycalculator.multis = 1;
            state.Text = "科学计算器    弧度制";
        }

        private void _deleterecord(object sender, RoutedEventArgs e)
        {
            Items.Clear();
            this.cal_record.IsPaneOpen = !this.cal_record.IsPaneOpen;
        }

        private void before(object sender, RoutedEventArgs e)
        {
            my_formula.Append("(");
            result.Text = my_formula.ToString();
            last[count++] = "(";
        }

        private void after(object sender, RoutedEventArgs e)
        {
            my_formula.Append(")");
            result.Text = my_formula.ToString();
            last[count++] = ")";
        }

        private void sqrt(object sender, RoutedEventArgs e)
        {
            if (count == 0)
            {
                my_formula.Append(mycalculator._result.ToString("G") + "^(1/");
                result.Text = my_formula.ToString();
                last[count++] = mycalculator._result.ToString("G") + "^(1/";
            }
            else
            {
                my_formula.Append("^(1/");
                result.Text = my_formula.ToString();
                last[count++] = "^(1/";
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            my_formula.Clear();
            result.Text = "";
            _result.Text = "";
        }

        private void division(object sender, RoutedEventArgs e)
        {
            if (count == 0)
            {
                my_formula.Append(mycalculator._result.ToString("G") + "/");
                result.Text = my_formula.ToString();
                last[count++] = mycalculator._result.ToString("G") + "/";
            }
            else
            {
                my_formula.Append("/");
                result.Text = my_formula.ToString();
                last[count++] = "/";
            }
        }

        private void multi(object sender, RoutedEventArgs e)
        {
            if (count == 0)
            {
                my_formula.Append(mycalculator._result.ToString("G") + "*");
                result.Text = my_formula.ToString();
                last[count++] = mycalculator._result.ToString("G") + "*";
            }
            else
            {
                my_formula.Append("*");
                result.Text = my_formula.ToString();
                last[count++] = "*";
            }
        }

        private void _delete(object sender, RoutedEventArgs e)
        {
            if (result.Text != "")
            {
                count--;
                temp = my_formula.ToString();
                temp = temp.Substring(0, temp.Length - last[count].Length);      //去掉最后一个字符
                my_formula.Clear();
                my_formula.Append(temp);
                result.Text = my_formula.ToString();
            }
        }

        private void arcsin(object sender, RoutedEventArgs e)
        {
            my_formula.Append("arcsin(");
            result.Text = my_formula.ToString();
            last[count++] = "arcsin(";
        }

        private void arccos(object sender, RoutedEventArgs e)
        {
            my_formula.Append("arccos(");
            result.Text = my_formula.ToString();
            last[count++] = "arccos(";
        }

        private void arctan(object sender, RoutedEventArgs e)
        {
            my_formula.Append("arctan(");
            result.Text = my_formula.ToString();
            last[count++] = "arctan(";
        }

        private void _7(object sender, RoutedEventArgs e)
        {
            my_formula.Append("7");
            result.Text = my_formula.ToString();
            last[count++] = "7";
        }

        private void _8(object sender, RoutedEventArgs e)
        {
            my_formula.Append("8");
            result.Text = my_formula.ToString();
            last[count++] = "8";
        }

        private void _9(object sender, RoutedEventArgs e)
        {
            my_formula.Append("9");
            result.Text = my_formula.ToString();
            last[count++] = "9";
        }

        private void minus(object sender, RoutedEventArgs e)
        {
            if (count == 0)
            {
                my_formula.Append(mycalculator._result.ToString("G") + "-");
                result.Text = my_formula.ToString();
                last[count++] = mycalculator._result.ToString("G") + "-";
            }
            else
            {
                my_formula.Append("-");
                result.Text = my_formula.ToString();
                last[count++] = "-";
            }
        }

        private void sin(object sender, RoutedEventArgs e)
        {
            my_formula.Append("sin(");
            result.Text = my_formula.ToString();
            last[count++] = "sin(";
        }

        private void sinh(object sender, RoutedEventArgs e)
        {
            my_formula.Append("sinh(");
            result.Text = my_formula.ToString();
            last[count++] = "sinh(";
        }

        private void cosh(object sender, RoutedEventArgs e)
        {
            my_formula.Append("cosh(");
            result.Text = my_formula.ToString();
            last[count++] = "csoh(";
        }

        private void tanh(object sender, RoutedEventArgs e)
        {
            my_formula.Append("tanh(");
            result.Text = my_formula.ToString();
            last[count++] = "tanh(";
        }

        private void fac(object sender, RoutedEventArgs e)
        {
            if (count == 0)
            {
                my_formula.Append(mycalculator._result.ToString("G") + "!");
                result.Text = my_formula.ToString();
                last[count++] = mycalculator._result.ToString("G") + "!";
            }
            else
            {
                my_formula.Append("!");
                result.Text = my_formula.ToString();
                last[count++] = "!";
            }
        }

        private void exp(object sender, RoutedEventArgs e)
        {
            my_formula.Append("exp(");
            result.Text = my_formula.ToString();
            last[count++] = "exp(";
        }

        private void cos(object sender, RoutedEventArgs e)
        {
            my_formula.Append("cos(");
            result.Text = my_formula.ToString();
            last[count++] = "cos(";
        }

        private void tan(object sender, RoutedEventArgs e)
        {
            my_formula.Append("tan(");
            result.Text = my_formula.ToString();
            last[count++] = "tan(";
        }

        private void _4(object sender, RoutedEventArgs e)
        {
            my_formula.Append("4");
            result.Text = my_formula.ToString();
            last[count++] = "4";
        }

        private void _5(object sender, RoutedEventArgs e)
        {
            my_formula.Append("5");
            result.Text = my_formula.ToString();
            last[count++] = "5";
        }

        private void _6(object sender, RoutedEventArgs e)
        {
            my_formula.Append("6");
            result.Text = my_formula.ToString();
            last[count++] = "6";
        }

        private void add(object sender, RoutedEventArgs e)
        {
            if (count == 0)
            {
                my_formula.Append(mycalculator._result.ToString("G") + "+");
                result.Text = my_formula.ToString();
                last[count++] = mycalculator._result.ToString("G") + "+";
            }
            else
            {
                my_formula.Append("+");
                result.Text = my_formula.ToString();
                last[count++] = "+";
            }
        }

        private void ln(object sender, RoutedEventArgs e)
        {
            my_formula.Append("ln(");
            result.Text = my_formula.ToString();
            last[count++] = "ln(";
        }

        private void log(object sender, RoutedEventArgs e)
        {
            my_formula.Append("lg(");
            result.Text = my_formula.ToString();
            last[count++] = "lg(";
        }

        private void exponent(object sender, RoutedEventArgs e)
        {
            if (count == 0)
            {
                my_formula.Append(mycalculator._result.ToString("G") + "^(");
                result.Text = my_formula.ToString();
                last[count++] = mycalculator._result.ToString("G") + "^(";
            }
            else
            {
                my_formula.Append("^(");
                result.Text = my_formula.ToString();
                last[count++] = "^(";
            }
        }

        private void _1(object sender, RoutedEventArgs e)
        {
            my_formula.Append("1");
            result.Text = my_formula.ToString();
            last[count++] = "1";
        }

        private void _2(object sender, RoutedEventArgs e)
        {
            my_formula.Append("2");
            result.Text = my_formula.ToString();
            last[count++] = "2";
        }

        private void _3(object sender, RoutedEventArgs e)
        {
            my_formula.Append("3");
            result.Text = my_formula.ToString();
            last[count++] = "3";
        }

        private void make(object sender, RoutedEventArgs e)
        {
            if (result.Text != "")
            {
                my_formula.Append("\0");
                mycalculator.formula = my_formula.ToString();
                mycalculator.start();

                // 各种运行的可能性
                switch (mycalculator.out_error)
                {
                    // 为了方便用户的使用，必需给出容错的功能，但是考虑到算法，下面的容错提示不会显示，但也不至于使应用崩溃。
                    // 保留容错提示是为了保持代码的可移植性。
                    case 1: result.Text = "除数不为零"; break;
                    case 3: result.Text = "不能识别输入符号"; break;
                    case 4: result.Text = "你让我算啥？"; break;
                    default:
                        _result.Text = result.Text + " = " + mycalculator._result.ToString("G");
                        Items.Add(new Item { Record = result.Text + " = " + mycalculator._result.ToString("G"), Result = mycalculator._result.ToString("G") });
                        break;
                }
                result.Text = "";
                my_formula.Clear();
                mycalculator.formula = "";
                count = 0;
            }
        }

        private void Ans(object sender, RoutedEventArgs e)
        {
            my_formula.Append(mycalculator._result.ToString("G"));
            result.Text = my_formula.ToString();
            last[count++] = mycalculator._result.ToString("G");
        }

        private void pi(object sender, RoutedEventArgs e)
        {
            my_formula.Append("pi");
            result.Text = my_formula.ToString();
            last[count++] = "pi";
        }

        private void e(object sender, RoutedEventArgs e)
        {
            my_formula.Append("e");
            result.Text = my_formula.ToString();
            last[count++] = "e";
        }

        private void _0(object sender, RoutedEventArgs e)
        {
            my_formula.Append("0");
            result.Text = my_formula.ToString();
            last[count++] = "0";
        }

        private void point(object sender, RoutedEventArgs e)
        {
            my_formula.Append(".");
            result.Text = my_formula.ToString();
            last[count++] = ".";
        }

        #endregion

    }

    // ListView绑定的数据
    public class Item
    {
        public string Record { get; set; }    //前台
        public string Result { get; set; }    //后台
    }
}
