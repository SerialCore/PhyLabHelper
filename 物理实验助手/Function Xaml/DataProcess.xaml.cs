using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace 物理实验助手.Function_Xaml
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DataProcess : Page
    {
        public DataProcess()
        {
            this.InitializeComponent();
            // 页面的缓存
            this.NavigationCacheMode = NavigationCacheMode.Required;

            mydata[0] = num1; mydata[1] = num2; mydata[2] = num3; mydata[3] = num4; mydata[4] = num5; mydata[5] = num6; mydata[6] = num7; mydata[7] = num8; mydata[8] = num9; mydata[9] = num10;
            mydata[10] = num11; mydata[11] = num12; mydata[12] = num13; mydata[13] = num14; mydata[14] = num15; mydata[15] = num16; mydata[16] = num17; mydata[17] = num18; mydata[18] = num19; mydata[19] = num20;
            mydata_x[0] = x1; mydata_x[1] = x2; mydata_x[2] = x3; mydata_x[3] = x4; mydata_x[4] = x5; mydata_x[5] = x6; mydata_x[6] = x7; mydata_x[7] = x8; mydata_x[8] = x9; mydata_x[9] = x10;
            mydata_x[10] = x11; mydata_x[11] = x12; mydata_x[12] = x13; mydata_x[13] = x14; mydata_x[14] = x15; mydata_x[15] = x16; mydata_x[16] = x17; mydata_x[17] = x18; mydata_x[18] = x19; mydata_x[19] = x20;
            mydata_y[0] = y1; mydata_y[1] = y2; mydata_y[2] = y3; mydata_y[3] = y4; mydata_y[4] = y5; mydata_y[5] = y6; mydata_y[6] = y7; mydata_y[7] = y8; mydata_y[8] = y9; mydata_y[9] = y10;
            mydata_y[10] = y11; mydata_y[11] = y12; mydata_y[12] = y13; mydata_y[13] = y14; mydata_y[14] = y15; mydata_y[15] = y16; mydata_y[16] = y17; mydata_y[17] = y18; mydata_y[18] = y19; mydata_y[19] = y20;
            matrix_1[0, 0] = A1; matrix_1[0, 1] = A2; matrix_1[0, 2] = A3; matrix_1[0, 3] = A4; matrix_1[1, 0] = A5; matrix_1[1, 1] = A6; matrix_1[1, 2] = A7; matrix_1[1, 3] = A8;
            matrix_1[2, 0] = A9; matrix_1[2, 1] = A10; matrix_1[2, 2] = A11; matrix_1[2, 3] = A12; matrix_1[3, 0] = A13; matrix_1[3, 1] = A14; matrix_1[3, 2] = A15; matrix_1[3, 3] = A16;
            matrix_2[0, 0] = B1; matrix_2[0, 1] = B2; matrix_2[0, 2] = B3; matrix_2[0, 3] = B4; matrix_2[1, 0] = B5; matrix_2[1, 1] = B6; matrix_2[1, 2] = B7; matrix_2[1, 3] = B8;
            matrix_2[2, 0] = B9; matrix_2[2, 1] = B10; matrix_2[2, 2] = B11; matrix_2[2, 3] = B12; matrix_2[3, 0] = B13; matrix_2[3, 1] = B14; matrix_2[3, 2] = B15; matrix_2[3, 3] = B16;
        }

        // 对象数组
        private TextBox[] mydata = new TextBox[20];
        private TextBox[] mydata_x = new TextBox[20];
        private TextBox[] mydata_y = new TextBox[20];
        private TextBox[,] matrix_1 = new TextBox[4, 4];
        private TextBox[,] matrix_2 = new TextBox[4, 4];

        // 不确定度计算
        private decimal t;           // t因子
        private decimal[] Num = new decimal[10];     //多次测量值的std::decimal类型                                      
        private decimal _average, _variance, _p, _A_uncertainty, _B_uncertainty, _All_uncertainty, _min_scale, _errors;   //计算后的数据
        private int n;              //输入数据的个数

        // 线性回归
        private decimal[] X = new decimal[20], Y = new decimal[20];  //需要处理的两组数据
        private decimal average_x, average_y, varians_xx, varians_xy, varians_yy, correlation;   //计算线性回归方程的间接数据
        private decimal k, b;          //线性回归得到的两个截距
        private int m, nx, ny;        //序偶和两组数据的个数

        // 矩阵计算
        private double[,] A, B;       //未确定规模的数组(这要是C++，绝对睁眼瞎)
        private int Ai, Aj, Bi, Bj;   //矩阵的实际规模
        private Function_Class.Calculate mycalculator = new Function_Class.Calculate();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 处理导航数据，判断是何种导航
            if (e.Parameter is string)
            {
                // 这是一个用于标记的尾巴，标明数据来源。尾巴前的内容都是传送的数据。
                // 其实传送的方法很多，可以用文件传送，可以直接传送字符串。可以用Split解析，可以用Json解析。
                // 我更希望用Split，因为简单。用Json只是为了练手。
                if (e.Parameter.ToString().EndsWith("timer_data") == true)
                {
                    // 砍掉尾巴
                    string timelist = e.Parameter.ToString().Replace("timer_data", "");
                    // 以字符‘s’作为分组的依据
                    timelist = timelist.Replace("\t\n", "");
                    // 去除最后一个‘s’，防止数组最后多出一个空字符串
                    timelist = timelist.TrimEnd('s');
                    string[] list = new string[10];
                    list = timelist.Split('s');
                    for (int i = 0; i < list.Length; i++)
                    {
                        mydata[i].Text = list[i];
                    }
                    min_scale.Text = "0.01";
                    calculate(null, null);
                }
            }
            else
            {
                ;
            }
        }

        #region 导航按钮

        private void place_on_file(object sender, RoutedEventArgs e)
        {
            string record = "";
            record += "\t\n" + "不确定度数据：" + "\t\n";
            for (int i = 0; i < 10; i++)
            {
                if (Num[i] != 0)
                    record += Num[i].ToString() + "\t\n";
            }
            record += _average.ToString("F6") + "  ±  " + _All_uncertainty.ToString("F6") + "\t\n" + out_analyse.Text + "\t\n";
            record += "\t\n" + "线性回归数据：" + "\t\n";
            for (int i = 0; i < 10; i++)
            {
                if (X[i] != 0 && Y[i] != 0)
                    record += X[i].ToString() + "    " + Y[i].ToString() + "\t\n";
            }
            record += linear.Text + "\t\n";
            record += "\t\n" + "矩阵计算数据：" + "\t\n";
            if (collectA() == 0)
            {
                var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
                var _a = a.DenseOfArray(A);
                record += "A：" + _a.ToString();
            }
            if (collectB() == 0)
            {
                var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
                var _b = b.DenseOfArray(B);
                record += "B：" + _b.ToString();
            }
            record += out_matrix.Text;

            this.Frame.Navigate(typeof(Function_Xaml.LabRecord), record);
        }

        #endregion

        #region 不确定度

        // 计算事件
        private void calculate(object sender, RoutedEventArgs e)
        {
            // 初始化数据
            n = 0;
            t = 0;
            for (int i = 0; i < 20; i++)
                Num[i] = 0.0m;
            _average = _variance = _p = _A_uncertainty = _B_uncertainty = _All_uncertainty = _min_scale = _errors = 0;

            // 判断如果输入框不为空，就将输入直接转化为decimal,这样可以简化空间复杂度，减少间接数据量
            for (int i = 0; i < 20; i++)
            {
                if (mydata[i].Text != "")
                {
                    try
                    {
                        Num[n] = Convert.ToDecimal(mydata[i].Text);
                        n++;
                    }
                    catch
                    {
                        out_analyse.Text = "出现非法输入，请检查数据";
                        return;
                    }
                }
            }
            _p = Convert.ToDecimal(p.Text);
            _errors = Convert.ToDecimal(errors.Text);
            if (min_scale.Text != "" && Convert.ToDecimal(min_scale.Text) != 0)
            {
                try
                {
                    _min_scale = Convert.ToDecimal(min_scale.Text);
                }
                catch
                {
                    out_analyse.Text = "出现非法输入，请检查数据";
                    return;
                }
            }
            else   //如果B类不确定度为零，那就没有算的必要了
            {
                out_analyse.Text = "请检查仪器最小刻度";
                return;
            }

            // 单次测量的不确定度
            if (n == 1)
            {
                if (_min_scale != 0)
                {
                    _B_uncertainty = _min_scale / _errors;
                    B_uncertainty.Text = _B_uncertainty.ToString("F6");
                    All_uncertainty.Text = _B_uncertainty.ToString("F6");
                    out_analyse.Text = "这是单次测量的不确定度，即B类不确定度本身";
                }
                else
                {
                    out_analyse.Text = "单次测量的不确定度是B类不确定度本身，请检查仪器最小刻度";
                }
                return;
            }

            // 只有两组数据
            if (n == 2)
            {
                out_analyse.Text = "只有两个数据，啥都干不了";
                return;
            }

            // 计算数据
            for (int i = 0; i < n; i++)
            {
                _average += Num[i] / n;
            }
            for (int i = 0; i < n; i++)
            {
                _variance += (Num[i] - _average) * (Num[i] - _average) / n;   //求方差
            }
            if (_p == 0.9m)    //求t因子
            {
                if (n == 3) t = 2.35m;
                if (n == 4) t = 2.13m;
                if (n == 5) t = 2.02m;
                if (n == 6) t = 1.94m;
                if (n == 7) t = 1.89m;
                if (n == 8) t = 1.86m;
                if (n == 9) t = 1.83m;
                if (n == 10) t = 1.81m;
                if (n == 11) t = 1.80m;
                if (n == 12) t = 1.78m;
                if (n == 13) t = 1.77m;
                if (n == 14) t = 1.76m;
                if (n == 15) t = 1.75m;
                if (n == 16) t = 1.75m;
                if (n == 17) t = 1.74m;
                if (n == 18) t = 1.73m;
                if (n == 19) t = 1.73m;
                if (n == 20) t = 1.72m;
            }
            if (_p == 0.95m)
            {
                if (n == 3) t = 3.18m;
                if (n == 4) t = 2.78m;
                if (n == 5) t = 2.57m;
                if (n == 6) t = 2.45m;
                if (n == 7) t = 2.36m;
                if (n == 8) t = 2.31m;
                if (n == 9) t = 2.26m;
                if (n == 10) t = 2.23m;
                if (n == 11) t = 2.20m;
                if (n == 12) t = 2.18m;
                if (n == 13) t = 2.16m;
                if (n == 14) t = 2.14m;
                if (n == 15) t = 2.13m;
                if (n == 16) t = 2.12m;
                if (n == 17) t = 2.11m;
                if (n == 18) t = 2.10m;
                if (n == 19) t = 2.09m;
                if (n == 20) t = 2.09m;
            }
            if (_p == 0.99m)
            {
                if (n == 3) t = 5.84m;
                if (n == 4) t = 4.60m;
                if (n == 5) t = 4.03m;
                if (n == 6) t = 3.71m;
                if (n == 7) t = 3.50m;
                if (n == 8) t = 3.36m;
                if (n == 9) t = 3.25m;
                if (n == 10) t = 3.17m;
                if (n == 11) t = 3.11m;
                if (n == 12) t = 3.05m;
                if (n == 13) t = 3.01m;
                if (n == 14) t = 2.98m;
                if (n == 15) t = 2.95m;
                if (n == 16) t = 2.92m;
                if (n == 17) t = 2.90m;
                if (n == 18) t = 2.88m;
                if (n == 19) t = 2.86m;
                if (n == 20) t = 2.85m;
            }
            if (_p == 0.683m)
            {
                if (n == 3) t = 1.20m;
                if (n == 4) t = 1.14m;
                if (n == 5) t = 1.11m;
                if (n == 6) t = 1.09m;
                if (n == 7) t = 1.08m;
                if (n == 8) t = 1.07m;
                if (n == 9) t = 1.06m;
                if (n == 10) t = 1.05m;
                if (n == 11) t = 1.05m;
                if (n == 12) t = 1.04m;
                if (n == 13) t = 1.04m;
                if (n == 14) t = 1.04m;
                if (n == 15) t = 1.03m;
                if (n == 16) t = 1.03m;
                if (n == 17) t = 1.03m;
                if (n == 18) t = 1.03m;
                if (n == 19) t = 1.03m;
                if (n == 20) t = 1.03m;
            }
            _A_uncertainty = (decimal)Math.Abs((double)t * Math.Sqrt((double)_variance / (n - 1)));  //取绝对值是为了消除-0的情况
            _B_uncertainty = _min_scale / _errors;
            _All_uncertainty = (decimal)Math.Sqrt((double)_A_uncertainty * (double)_A_uncertainty + (double)_B_uncertainty * (double)_B_uncertainty);

            // 将数据输出
            average.Text = _average.ToString("F6");
            variance.Text = _variance.ToString("F6");
            A_uncertainty.Text = _A_uncertainty.ToString("F6");
            B_uncertainty.Text = _B_uncertainty.ToString("F6");
            All_uncertainty.Text = _All_uncertainty.ToString("F6");
            out_analyse.Text = "共计" + n.ToString() + "个数据";
        }

        private void clear_un(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 20; i++)
                mydata[i].Text = "";
            average.Text = ""; variance.Text = ""; A_uncertainty.Text = ""; B_uncertainty.Text = ""; All_uncertainty.Text = ""; min_scale.Text = "";
            out_analyse.Text = "";
        }

        #endregion

        #region 线性回归

        private async void calculateline(object sender, RoutedEventArgs e)
        {
            // 初始化数据
            m = nx = ny = 0;
            k = b = 0;
            for (int i = 0; i < 20; i++)
                X[i] = Y[i] = 0.0m;
            average_x = average_y = varians_xx = varians_xy = varians_yy = correlation = 0;

            // 判断如果输入框不为空，就将输入直接转化为decimal,这样可以简化空间复杂度，减少间接数据量
            for (int i = 0; i < 20; i++)
            {
                if (mydata_x[i].Text != "")
                {
                    try
                    {
                        X[nx] = Convert.ToDecimal(mydata_x[i].Text);
                        nx++;
                    }
                    catch
                    {
                        linear.Text = "出现非法输入，请检查数据";
                        return;
                    }
                }
                if (mydata_y[i].Text != "")
                {
                    try
                    {
                        Y[ny] = Convert.ToDecimal(mydata_y[i].Text);
                        ny++; m++;
                    }
                    catch
                    {
                        linear.Text = "出现非法输入，请检查数据";
                        return;
                    }
                }
            }

            if (m == 0 || m != nx || m != ny)
            {
                linear.Text = "请输入完整的数据";
                return;
            }

            // 计算数据
            for (int i = 0; i < m; i++)
            {
                average_x += X[i] / m;    //求x平均值
            }
            for (int i = 0; i < m; i++)
            {
                average_y += Y[i] / m;    //求y平均值
            }
            for (int i = 0; i < m; i++)
            {
                varians_xx += (X[i] - average_x) * (X[i] - average_x) / m;   //求xx差-积-和
            }
            for (int i = 0; i < m; i++)
            {
                varians_xy += (X[i] - average_x) * (Y[i] - average_y) / m;   //求xy差-积-和
            }
            for (int i = 0; i < m; i++)
            {
                varians_yy += (Y[i] - average_y) * (Y[i] - average_y) / m;   //求xy差-积-和
            }
            k = varians_xy / varians_xx;
            b = average_y - k * average_x;
            correlation = varians_xy / ((varians_xx + varians_yy) / 2);
            if (b > 0)
                linear.Text = "线性回归：y=" + k.ToString("F3") + "x" + "+" + b.ToString("F3") + "\t\n" + "相关系数：" + correlation.ToString("F3");
            if (b < 0)
                linear.Text = "线性回归：y=" + k.ToString("F3") + "x" + "-" + Math.Abs(b).ToString("F3") + "\t\n" + "相关系数：" + correlation.ToString("F3");
            if (b == 0)
                linear.Text = "线性回归：y=" + k.ToString("F3") + "x" + "\t\n" + "相关系数：" + correlation.ToString("F3");

            JsonObject o = new JsonObject();
            JsonArray jsonArray_x = new JsonArray();
            JsonArray jsonArray_y = new JsonArray();

            for (int i = 0; i < m; i++)
            {
                jsonArray_x.Add(JsonValue.CreateNumberValue((double)X[i]));
                jsonArray_y.Add(JsonValue.CreateNumberValue((double)Y[i]));
            }

            o["x"] = jsonArray_x;
            o["y"] = jsonArray_y;
            string[] json = new string[1];
            json[0] = o.Stringify();
            await chart_board.InvokeScriptAsync("clear", null);
            await chart_board.InvokeScriptAsync("data", json);
            await chart_board.InvokeScriptAsync("chart", null);
        }

        private async void clear_lineat(object sender, RoutedEventArgs e)
        {
            linear.Text = "";
            for (int i = 0; i < 20; i++)
                mydata_x[i].Text = "";
            for (int i = 0; i < 20; i++)
                mydata_y[i].Text = "";
            await chart_board.InvokeScriptAsync("clear", null);
        }

        #endregion

        #region 矩阵计算

        /*    加 add
              减 AsubB  BsunA
              乘 AmulB BmulA
              转置 AT BT
              求逆 _A _B
              行列式的值 valueA valueB
              秩   rankA rankB
              分解 divA   divB  */
        private int collectA()
        {
            Ai = Aj = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (matrix_1[i, j].Text != "")
                    {
                        if (i > Ai) Ai = i;
                        if (j > Aj) Aj = j;
                    }
                }
            }
            if (Ai == 0 && Aj == 0) return 1;
            Ai++; Aj++;
            A = new double[Ai, Aj];
            for (int i = 0; i < Ai; i++)
            {
                for (int j = 0; j < Aj; j++)
                {
                    if (matrix_1[i, j].Text != "")
                    {
                        try
                        {
                            A[i, j] = Convert.ToDouble(matrix_1[i, j].Text);
                        }
                        catch
                        {
                            mycalculator.formula = matrix_1[i, j].Text + "\0";
                            mycalculator.start();
                            switch (mycalculator.out_error)
                            {
                                case 1: out_matrix.Text = "出现非法输入，除数不为零"; return 1;
                                case 3: out_matrix.Text = "出现非法输入，不能识别输入符号"; return 1;
                                case 4: break;
                                default: A[i, j] = Convert.ToDouble(mycalculator._result); break;
                            }
                            mycalculator.formula = "";
                        }
                    }
                }
            }
            return 0;
        }

        private int collectB()
        {
            Bi = Bj = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (matrix_2[i, j].Text != "")
                    {
                        if (i > Bi) Bi = i;
                        if (j > Bj) Bj = j;
                    }
                }
            }
            if (Bi == 0 && Bj == 0) return 1;
            Bi++; Bj++;
            B = new double[Bi, Bj];
            for (int i = 0; i < Bi; i++)
            {
                for (int j = 0; j < Bj; j++)
                {
                    if (matrix_2[i, j].Text != "")
                    {
                        try
                        {
                            B[i, j] = Convert.ToDouble(matrix_2[i, j].Text);
                        }
                        catch
                        {
                            mycalculator.formula = matrix_2[i, j].Text + "\0";
                            mycalculator.start();
                            switch (mycalculator.out_error)
                            {
                                case 1: out_matrix.Text = "出现非法输入，除数不为零"; return 1;
                                case 3: out_matrix.Text = "出现非法输入，不能识别输入符号"; return 1;
                                default: B[i, j] = Convert.ToDouble(mycalculator._result); break;
                            }
                            mycalculator.formula = "";
                        }
                    }
                }
            }
            return 0;
        }

        private void add(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1 || collectB() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            try
            {
                _a = _a.Add(_b);
                out_matrix.Text = "A + B = " + _a.ToString();
            }
            catch
            {
                out_matrix.Text = "A + B = " + "A和B必须都满足n*n";
            }
        }

        private void AsubB(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1 || collectB() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            try
            {
                _a = _a.Subtract(_b);
                out_matrix.Text = "A - B = " + _a.ToString();
            }
            catch
            {
                out_matrix.Text = "A - B = " + "A和B必须都满足n*n";
            }
        }

        private void BsubA(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1 || collectB() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            try
            {
                _a = _b.Subtract(_a);
                out_matrix.Text = "B - A = " + _a.ToString();
            }
            catch
            {
                out_matrix.Text = "B - A = " + "A和B必须都满足n*n";
            }
        }

        private void AmulB(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1 || collectB() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            try
            {
                _a = _a.Multiply(_b);
                out_matrix.Text = "AB = " + _a.ToString();
            }
            catch
            {
                out_matrix.Text = "AB = " + "A和B必须分别满足n*m和m*n";
            }
        }

        private void BmulA(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1 || collectB() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            try
            {
                _a = _b.Multiply(_a);
                out_matrix.Text = "BA = " + _a.ToString();
            }
            catch
            {
                out_matrix.Text = "BA = " + "A和B必须分别满足n*m和m*n";
            }
        }

        private void AT(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            _a = _a.Transpose();
            out_matrix.Text = "A的转置：" + _a.ToString();
        }

        private void BT(object sender, RoutedEventArgs e)
        {
            if (collectB() == 1)
                return;
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            _b = _b.Transpose();
            out_matrix.Text = "B的转置：" + _b.ToString();
        }

        private void _A(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            try
            {
                if (_a.Determinant() != 0)
                {
                    try
                    {
                        _a = _a.Inverse();
                        out_matrix.Text = "A的逆：" + _a.ToString();
                    }
                    catch
                    {
                        out_matrix.Text = "A的逆：" + "A不可逆";
                    }
                }
                else
                {
                    out_matrix.Text = "A的逆：" + "行列式值为0时，不可逆";
                }
            }
            catch
            {
                out_matrix.Text = "A的逆：" + "A必须满足n*n";
            }
        }

        private void _B(object sender, RoutedEventArgs e)
        {
            if (collectB() == 1)
                return;
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            try
            {
                if (_b.Determinant() != 0)
                {
                    try
                    {
                        _b = _b.Inverse();
                        out_matrix.Text = "B的逆：" + _b.ToString();
                    }
                    catch
                    {
                        out_matrix.Text = "B的逆：" + "B不可逆";
                    }
                }
                else
                {
                    out_matrix.Text = "B的逆：" + "行列式值为0时，不可逆";
                }
            }
            catch
            {
                out_matrix.Text = "B的逆：" + "B必须满足n*n";
            }
        }

        private void valueA(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            try
            {
                double value = _a.Determinant();
                out_matrix.Text = "A的行列式的值：" + value.ToString();
            }
            catch
            {
                out_matrix.Text = "A的行列式的值：" + "行列式必须满足n*n";
            }
        }

        private void valueB(object sender, RoutedEventArgs e)
        {
            if (collectB() == 1)
                return;
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            try
            {
                double value = _b.Determinant();
                out_matrix.Text = "B的行列式的值：" + value.ToString();
            }
            catch
            {
                out_matrix.Text = "B的行列式的值：" + "行列式必须满足n*n";
            }
        }

        private void rankA(object sender, RoutedEventArgs e)
        {
            if (collectA() == 1)
                return;
            var a = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _a = a.DenseOfArray(A);
            int rank = _a.Rank();
            out_matrix.Text = "A的秩：" + rank.ToString();
        }

        private void rankB(object sender, RoutedEventArgs e)
        {
            if (collectB() == 1)
                return;
            var b = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build;
            var _b = b.DenseOfArray(B);
            int rank = _b.Rank();
            out_matrix.Text = "B的秩：" + rank.ToString();
        }

        private void clear_matrix(object sender, RoutedEventArgs e)
        {
            out_matrix.Text = "";
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix_1[i, j].Text = "";
                    matrix_2[i, j].Text = "";
                }
            }
        }

        #endregion

    }
}
