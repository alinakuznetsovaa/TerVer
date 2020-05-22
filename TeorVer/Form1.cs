using System.Windows.Forms;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ZedGraph;
using System;

namespace TeorVer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        void ShellSort(double[] mass)
        {
            int i, j, step;
            double tmp1;
            int n = mass.Length;
            for (step = n / 2; step > 0; step /= 2)
            {
                for (i = step; i < n; i++)
                {
                    tmp1 = mass[i];
                    for (j = i; j >= step; j -= step)
                    {
                        if (tmp1 < mass[j - step])
                        {
                            mass[j] = mass[j - step];
                        }
                        else
                        {
                            break;
                        }
                    }
                    mass[j] = tmp1;
                }
            }
        }


        private double Gamma(double Num)
        {
            if (Num > 1)
            {
                double Number = (Num - 1) * Gamma(Num - 1);
                return Number;
            }
            if (Num == 1.0)
            {
                return 1.0;
            }

            return Math.Pow(3.14159, 0.5);
        }

        private double Hi_Quadrat(double R0, double k)
        {
            double gamma = Gamma((k - 1.0) / 2.0);
            double result = 0;
            double exp2 = 1;
            for (int i = 0; i < k - 1; i++)
            {
                exp2 *= 2.0;
            }
            exp2 = Math.Pow(exp2, 0.5);
            if (k == 2)
            {
                for (int i = 2; i <= 1000; i++)
                {
                    result += (Math.Exp(-R0 * ((double)i - 1.0) / 1000) * Math.Pow(R0 * ((double)i - 1.0) / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma +
                               Math.Exp(-R0 * (double)i / 1000) * Math.Pow(R0 * (double)i / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma) * R0 / 2000;
                }
                return result;
            }
            for (int i = 1; i <= 1000; i++)
            {
                result += (Math.Exp(-R0 * ((double)i - 1.0) / 1000) * Math.Pow(R0 * ((double)i - 1.0) / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma +
                           Math.Exp(-R0 * (double)i / 1000) * Math.Pow(R0 * (double)i / 1000, (k - 1.0) / 2 - 1) / exp2 / gamma) * R0 / 2000;
            }
            return result;
        }




        private void button1_Click(object sender, EventArgs e)
        {

            //1 лаба
            dataGridView1.Rows.Clear();

            Random rnd = new Random(DateTime.Now.Second);
            double rv = 0.0;

            double sigma = Convert.ToDouble(textBox1.Text);
            int Number_steps = Convert.ToInt32(textBox2.Text);
            double[] y = new double[Number_steps];

            for (int i = 0; i < Number_steps; i++)
            {

                y[i] = sigma * Math.Sqrt(-2 * Math.Log(rnd.NextDouble()));
            }

            ShellSort(y);
            for (int i = 0; i < y.Length; i++)
            {
                dataGridView1.Rows.Add();

                dataGridView1.Rows[i].Cells[0].Value = i;
                dataGridView1.Rows[i].Cells[1].Value = y[i];
            }


            // 2 лаба
            double v_s = 0;
            for (int i = 0; i < y.Length; i++)
            {
                v_s += y[i];
            }
            v_s = v_s / Number_steps; //выборочное среднее

            double v_d = 0;
            for (int i = 0; i < y.Length; i++)
            {
                v_d += Math.Pow((y[i] - v_s), 2);
            }
            v_d = v_d / Number_steps;//выборочная дисперсия

            double r_v = y[y.Length - 1] - y[0];//размах выборки

            double me = 0; //выборочная медиана
            int k = Number_steps / 2;
            if (Number_steps % 2 == 1)
            {
                me = y[k];
            }
            else
            {
                me = y[k - 1] + y[k];
                me /= 2;
            }

            double mo = Math.Sqrt(Math.PI / 2) * sigma;//мат ожидание
            double th = mo - v_s;
            if (th < 0)
            {
                th = -th;
            }
            double dis = (2 - (Math.PI / 2)) * sigma * sigma;//дисперсия
            double td = dis - v_d;
            if (td < 0)
            {
                td = -td;
            }
            dataGridView2.Rows.Add(mo, v_s, th, dis, v_d, td, me, r_v);

            Draw(zedGraphControl1, y);

            
        }



    




       
            

        private void Draw(ZedGraphControl pane,  double[] vals)
        {
            double sigma = Convert.ToDouble(textBox1.Text);
            int N = Convert.ToInt32(textBox3.Text);
            int n = vals.Length;
            double[] interval = new double[N + 1];
            double[] value = new double[N];
            double[] f = new double[N];

            //строим функции распределения

            ZedGraph.PointPairList F_list = new ZedGraph.PointPairList();
            ZedGraph.PointPairList Fn_list = new ZedGraph.PointPairList();

            zedGraphControl1.GraphPane.Title.Text = "График функций распределения";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "X";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "F(x)";

            double D = 0.0; // Мера расхождения
            

            double h = vals[n - 1] / 1000;

            int sum = 0;
            for (int i = 0; i < 1000; i++)
            {
                sum = 0;
                for (int j = 0; j < n; j++)
                {
                    double temp = vals[0] + h * i;
                    if (vals[j] < vals[0] + h * i)
                        sum++;
                }
                Fn_list.Add(vals[0] + h * i, (double)sum / (double)n);
                F_list.Add(vals[0] + h * i, 1 - Math.Exp(-(vals[0] +h * i) * (vals[0]+h * i) / (2 * sigma * sigma)));
                D = Math.Max(D, Math.Abs((double)sum / (double)n - (1 - Math.Exp(-(vals[0]+h * i) *( vals[0]+h * i) / (2 * sigma * sigma)))));

            }
            zedGraphControl1.GraphPane.CurveList.Clear();

            textBox4.Text = D.ToString();
            ZedGraph.LineItem CurveF = zedGraphControl1.GraphPane.AddCurve("F", F_list, Color.FromName("Red"), ZedGraph.SymbolType.None);
            ZedGraph.LineItem CurveFn = zedGraphControl1.GraphPane.AddCurve("Fn", Fn_list, Color.FromName("Blue"), ZedGraph.SymbolType.None);


            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();


            //гистограмма

            double h2 = (vals[n - 1] - vals[0]) / N;



            for (int i = 0; i < N; i++)
            {
                interval[i] = vals[0] + (double)i * h2;


            }
            interval[N] = vals[n - 1];


            int sum2;
            for (int i = 0; i < N; i++)
            {
                sum2 = 0;
                for (int j = 0; j < n; j++)
                {
                    if ((interval[i] < vals[j]) && (vals[j] <= interval[i + 1]))
                        sum2++;
                }

                value[i] = (double)sum2 / (h2 * (double)n);
            }

            GraphPane pane1 = zedGraphControl2.GraphPane;
            pane1.CurveList.Clear();

            BarItem curve1 = pane1.AddBar(null, null, value, Color.Blue);
            zedGraphControl2.GraphPane.Title.Text = "Гистограмма";

            pane1.YAxis.Scale.Min = 0.0;
            pane1.YAxis.Scale.Max = value.Max() + 0.001;
            pane1.BarSettings.MinClusterGap = 0.0f;

            zedGraphControl2.AxisChange();
            zedGraphControl2.Invalidate();

            //3 таблица

            double max = 0.0;
            for (int i = 0; i < N; i++)
            {
                dataGridView3.ColumnCount = N;
                dataGridView3.RowCount = 3;
                dataGridView3.Rows[0].HeaderCell.Value = "z [ j ] ";
                dataGridView3.Rows[1].HeaderCell.Value = "f ( z [ j ] )";
                dataGridView3.Rows[2].HeaderCell.Value = "n [ j ] / n ∆[ j ] ";


                dataGridView3.Columns[i].HeaderText = string.Format("z" + (i + 1), i);
                dataGridView3.Rows[0].Cells[i].Value = interval[i] + h2 * 0.5;
                f[i] = ((interval[i] + h2 * 0.5) * Math.Exp(-(interval[i] + h2 * 0.5) * (interval[i] + h2 * 0.5) / (2 * sigma * sigma))) / (sigma * sigma);
                dataGridView3.Rows[1].Cells[i].Value = f[i];
                dataGridView3.Rows[2].Cells[i].Value = value[i];
                if (Math.Abs(value[i] - f[i]) > max)
                    max = Math.Abs(value[i] - f[i]);
            }
            textBox5.Text = max.ToString();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //3 лаба
            dataGridView5.Rows.Clear();


            int Number_steps = Convert.ToInt32(textBox2.Text);
            double sigma = Convert.ToDouble(textBox1.Text);
            double[] y = new double[Number_steps];

            double[] NumberExpInDistance = new Double[dataGridView4.RowCount];
            for (int i = 0; i < Number_steps; i++)
            {
                y[i] = (double)dataGridView1.Rows[i].Cells[1].Value;
            }

            double[] q = new Double[dataGridView4.RowCount ];
          
            //Вычисление R0
            double R0 = 0;
            double sum=0;


            q[0] = 1 - Math.Exp(-Convert.ToDouble(dataGridView4[0, 0].Value) * Convert.ToDouble(dataGridView4[0, 0].Value) / (2 * sigma * sigma));
            sum += q[0];
            for (int i = 1; i < dataGridView4.RowCount-1 ; i++)
            {

                q[i] = -Math.Exp(-Convert.ToDouble(dataGridView4[0, i].Value) * Convert.ToDouble(dataGridView4[0, i].Value) / (2 * sigma * sigma))
                      + Math.Exp(-Convert.ToDouble(dataGridView4[0, i - 1].Value) * Convert.ToDouble(dataGridView4[0, i - 1].Value) / (2 * sigma * sigma));
                sum += q[i];
            }



            /*
            double delta = Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount - 1].Value) - Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount - 2].Value);
            for (int i = 1; i <= 1000; i++)
            {
                double z1 = Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount - 1].Value) + delta * (i - 1) / 1000;
                double z2 = Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount - 1].Value) + delta * i / 1000;

                q[dataGridView4.RowCount - 1] += (z1 * Math.Exp(-z1 * z1 / (2 * sigma * sigma)) / (sigma * sigma) + z2 * Math.Exp(-z2 * z2 / (2 * sigma * sigma)) / (sigma * sigma)) * delta / 2000;
            }
            */



            q[q.Length - 1] = 1 - sum;
            //Math.Exp(-Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount -1].Value) *
                  //    Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount-1 ].Value) / (2 * sigma * sigma));
            NumberExpInDistance[0] = 0;
            double alpha = Convert.ToDouble(textBox6.Text);
            int Numflag = 0;
            for (int j = Numflag; j < y.Length; j++)
            {
                if (y[j] <= Convert.ToDouble(dataGridView4[0, 0].Value))
                {
                    NumberExpInDistance[0] += 1;
                }
                else
                {
                    Numflag = j;
                    break;
                }
            }
            for (int i = 1; i < dataGridView4.RowCount - 1; i++)
            {
                for (int j = Numflag; j < y.Length; j++)
                {
                    if (y[j] <= Convert.ToDouble(dataGridView4[0, i].Value) && y[j] > Convert.ToDouble(dataGridView4[0, i - 1].Value))
                    {
                        NumberExpInDistance[i] += 1;
                    }
                    else
                    {
                        Numflag = j;
                        break;
                    }
                }
            }
            for (int j = Numflag; j < y.Length; j++)
            {
                if (y[j] > Convert.ToDouble(dataGridView4[0, dataGridView4.RowCount-1].Value))
                {
                    NumberExpInDistance[NumberExpInDistance.Length - 1]+=1;
                }
            }
            for (int i = 0; i < q.Length; i++)
            {
                if (q[i] != 0)
                {
                    R0 += Math.Pow(NumberExpInDistance[i] - (double)Number_steps * q[i], 2) / ((double)Number_steps * q[i]);
                }
            }
            //Вывод
            double FR = 1 - Hi_Quadrat(R0, q.Length);
            textBox7.Text = Convert.ToString(FR);
            for (int i = 0; i < q.Length; i++)
            {
                dataGridView5.Rows.Add(i, q[i], NumberExpInDistance[i]);
            }
            if (FR > alpha)
            {
                textBox8.Text = "Гипотеза принята";
            }
            else
            {
                textBox8.Text = "Гипотеза отвергнута";
            }

        }
    }
}
