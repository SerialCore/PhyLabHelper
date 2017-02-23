using System;

namespace 物理实验助手.Function_Class
{
    class Calculate
    {
        // 不需要从外部调用
        private char input;       //input可为字符、数字，因为input的只需要在各个函数中自由传递，所以定义为全局变量，从formula中提取字符
        private int i;            //计算式的中字符的位置
        private int length;

        // 需要从外部调用
        public string formula;                 //字符串化的计算式，输入
        public decimal _result;                 //plus()函数返回的值，即最终计算结果，输出
        public decimal multis = 180m / (decimal)Math.PI;   //用于反三角函数弧度制和角度制的设置，修正参数，外部调用
        public decimal div = 180m / (decimal)Math.PI;      //用于三角函数弧度制和角度制的设置，修正参数，外部调用
        public int out_error;                  //1.除数为零 3.不能识别输入符号 4.没有输入计算式，输出

        public Calculate()
        {

        }

        public void Input()
        {
            if (i <= length - 1)
            {
                input = formula[i];
            }
        }

        public void start()
        {
            //初始化变量
            out_error = 0;
            i = 0;
            length = formula.Length;

            // 从formula中取char，i为全局变量，并转入间接递归进行计算
            input = formula[i];
            _result = plus();
        }

        public decimal plus()
        {
            decimal result = 0.0m;
            result = multi();        //先考虑高优先级，立即转出
            while ((input == '+' || input == '-') && i != 0 && formula[i - 1] != '(')
            {
                if (input == '+')
                {
                    i++;
                    Input();
                    result += multi();
                }
                else if (input == '-')
                {
                    i++;
                    Input();
                    result -= multi();
                }
            }
            return result;
        }

        public decimal multi()
        {
            decimal div;
            decimal result = 0.0m;
            result = power();          //先考虑高优先级，立即转出
            while ((input == '*') || (input == '/'))    //如果输入的是乘除
            {
                if (input == '*')
                {
                    i++;
                    Input();
                    result *= power();
                }
                else if (input == '/')
                {
                    i++;
                    Input();
                    div = power();
                    if (div == 0)
                    {
                        out_error = 1;
                        return 0.0m;
                    }
                    result /= div;
                }
            }
            return result;
        }

        public decimal power()
        {
            decimal result = 0.0m;
            result = signal();
            while (input == '^')
            {
                if (input == '^')
                {
                    i++;
                    Input();
                    result = (decimal)System.Math.Pow((double)result, (double)signal());
                }
            }
            return result;
        }

        public decimal signal()
        {
            decimal result = 0.0m;
            if (input == '-' && (i == 0 || formula[i - 1] == '('))
            {
                i++;
                Input();
                result = -high();
            }
            else if (input == '+' && (i == 0 || formula[i - 1] == '('))
            {
                i++;
                Input();
                result = high();
            }
            else if (input != '-' && input != '+' && input != '\0')
            {
                result = high();
            }
            return result;
        }

        public decimal high()
        {
            decimal result = 0.0m;      //刚得到源码时，程序不能识别小数点，因为result的类型是int，改成decimal就可以识别小数点
            if (input == '(')      //如果输入的是'('
            {
                i++;
                Input();
                result = plus();  //先回到低级运算，等待之后升级或进行低级运算，即运行括号内的式子
                if (input == ')')
                {
                    i++;
                    Input();
                }
                else
                {
                    out_error = 3;
                    return 0.0m;
                }
            }
            else if (input >= '0' && input <= '9')
            {
                string num = "";
                for (; formula[i] >= '0' && formula[i] <= '9' || formula[i] == '.';)
                {
                    num += formula[i];
                    i++;
                    if (i > length - 1)
                        break;
                }
                try
                {
                    result = (decimal)Convert.ToDouble(num);
                }
                catch
                {
                    result = 0.0m;
                    out_error = 3;
                }
                Input();
            }
            else if (input == 'e')        //用来识别'e'
            {

                i++;
                Input();
                if (input != 'x')
                    result = (decimal)Math.E;
                else
                {
                    i += 2;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = (decimal)System.Math.Exp((double)plus());
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }
            }
            else if (input == 'p')        //用来识别'pi'   //程序运行时，似乎不能使用输入法输入'π'
            {
                i++;
                Input();
                if (input == 'i')
                {
                    result = (decimal)Math.PI;
                    i++;
                    Input();
                }
            }
            else if (input == 'l')
            {
                i++;
                Input();
                if (input == 'g')         //判断以10为底的指数
                {
                    i++;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = (decimal)System.Math.Log10((double)plus());
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }
                else if (input == 'n')    //判断以e为底的指数
                {
                    i++;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = (decimal)System.Math.Log((double)plus());
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }
            }
            else if (input == 's')    //判断sin()
            {
                i = i + 3;
                if (i <= length - 1)
                {
                    input = formula[i];
                }
                if (input == '(')
                {
                    i++;
                    Input();
                    result = checkPrecision(System.Math.Sin((double)(plus() / div)));
                    if (input == ')')
                    {
                        i++;
                        Input();
                    }
                    else
                    {
                        out_error = 3;
                        return 0.0m;
                    }
                }
                if (input == 'h')
                {
                    i++;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = checkPrecision(System.Math.Sinh((double)(plus() / div)));
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }


            }
            else if (input == 'c')      //判断cos()
            {
                i += 3;
                Input();
                if (input == '(')
                {
                    i++;
                    Input();
                    result = checkPrecision(System.Math.Cos((double)(plus() / div)));
                    if (input == ')')
                    {
                        i++;
                        Input();
                    }
                    else
                    {
                        out_error = 3;
                        return 0.0m;
                    }
                }
                if (input == 'h')
                {
                    i++;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = checkPrecision(System.Math.Cosh((double)(plus() / div)));
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }
            }
            else if (input == 't')     //判断tan()
            {
                i += 3;
                Input();
                if (input == '(')
                {
                    i++;
                    Input();
                    result = checkPrecision(System.Math.Tan((double)(plus() / div)));
                    if (input == ')')
                    {
                        i++;
                        Input();
                    }
                    else
                    {
                        out_error = 3;
                        return 0.0m;
                    }
                }
                if (input == 'h')
                {
                    i++;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = checkPrecision(System.Math.Tanh((double)(plus() / div)));
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }
            }
            else if (input == 'a')       //判断反三角函数
            {
                i += 3;
                Input();
                if (input == 's')        //判断反正弦
                {
                    i += 3;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = checkPrecision((double)multis * System.Math.Asin((double)plus()));
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }
                else if (input == 'c') //判断反余弦
                {
                    i += 3;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = checkPrecision((double)multis * System.Math.Acos((double)plus()));
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }
                }
                else if (input == 't')    //判断反正切
                {
                    i += 3;
                    Input();
                    if (input == '(')
                    {
                        i++;
                        Input();
                        result = checkPrecision((double)multis * System.Math.Atan((double)plus()));
                        if (input == ')')
                        {
                            i++;
                            Input();
                        }
                        else
                        {
                            out_error = 3;
                            return 0.0m;
                        }
                    }

                }
            }
            if (input == '!')
            {
                int sum = 1;
                for (int i = 1; i <= result; i++)
                    sum *= i;
                result = sum;
                i++;
                Input();
            }
            return result;
        }

        private decimal checkPrecision(double index)
        {
            if (index < 0.00000000000001 && index > 0)
                index = 0.0;
            return (decimal)index;
        }
    }
}
