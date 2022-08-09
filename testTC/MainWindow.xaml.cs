using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization;
using System.Windows.Forms.DataVisualization.Charting;

namespace testTC
{

    public partial class MainWindow : Window
    {
        DispatcherTimer timer1 = new DispatcherTimer();
        Random random = new Random();
        int xLast, yLast1, yLast2 = 0;          // крайние координаты для 2-ух прямых, X общий, Y1 и Y2 различны;
        int cycle = 0;                          // id для распределения очередности выполнения циклов;

        public MainWindow()
        {
            InitializeComponent();  
            CreateChart();                      // создание чарта;
            comboBox1.Items.Add("1");           // добавление выбора в выпадающем списке для trend;
            comboBox1.Items.Add("2");
            comboBox1.SelectedIndex = 0;        // по дефолту выбран нулевой элемент в comboBox;
        }

        private void CreateChart()              // создание чарта;
        {
            chart.Series.Clear();                                                         // очистка графика;
            chart.ChartAreas.Clear();

            chart.ChartAreas.Add(new ChartArea("Default"));                               // создание области, где будут размещены 2 линии Series1 и Series2 и линии тренда;
            chart.ChartAreas.FindByName("Default").CursorX.IsUserSelectionEnabled = true; // возможность зумить чарт;
            chart.ChartAreas.FindByName("Default").CursorX.IsUserEnabled = true;          // красная черта на графике на ЛКМ;

            chart.Series.Add(new Series("Series1"));                                      // обавление 2-ух основных линий и 2 линий тренда;
            chart.Series.Add(new Series("TrendLine1"));     
            chart.Series.Add(new Series("TrendLine2"));
            chart.Series.Add(new Series("Series2"));         
            
            chart.Series["Series1"].ChartArea = "Default";                                // добавляем все линии в область Default;
            chart.Series["TrendLine1"].ChartArea = "Default";
            chart.Series["TrendLine2"].ChartArea = "Default";
            chart.Series["Series2"].ChartArea = "Default";

            chart.Series["Series1"].ChartType = SeriesChartType.Line;                     // создаем фигуры "Линия";
            chart.Series["Series2"].ChartType = SeriesChartType.Line;
            chart.Series["TrendLine2"].ChartType = SeriesChartType.Line;
            chart.Series["TrendLine1"].ChartType = SeriesChartType.Line;

            chart.Series["TrendLine1"].BorderWidth = 4;                                   // задаем толщину линий тренда;
            chart.Series["TrendLine2"].BorderWidth = 4;

            chart.Series["Series1"].Points.AddXY(xLast, yLast1);                          // начальные точки для основных 2 линий;
            chart.Series["Series2"].Points.AddXY(xLast, yLast2);
        }

        private void Timer_tick(object sender, object e)   // то, что происходит во время одного Tick;
        {
            switch (cycle)                                 // выбор очередности цикла исходя из id для распределения очередности;
            {                                              // можно не использовать switch, а просто перечислить все циклы, но тогда за один Tick будут выполняться все циклы - я просто разбил циклы на части;
                case 0:
                    firstCycle();
                    break;
                case 1:
                    secondCycle();
                    break;
                case 2:
                    thirdCycle();
                    secondCycle();
                    cycle = 0;
                    break;
                default:
                    cycle = 0;
                break;
            }
        }

        private void Timer_start()      
        {
             timer1.Interval = TimeSpan.FromMilliseconds(1000); // интервал в 1 секунду; Прорисовка линий будет с частотой в 1 секунду;
             timer1.Tick += Timer_tick;
             timer1.Start();         
        }

        private void CreateTrend(int a)     // создание линий тренда;
        {
            chart.Series["TrendLine1"].Points.AddXY(0, 0);                  // строится первая точка первой линии тренда;
            chart.Series["TrendLine1"].Points.AddXY(xLast, yLast1);         // строится вторая точка первой линии тренда;
            if (a == 1)                                                     // если в combobox выбраны 2 линии trend, то ...
            {
                chart.Series["TrendLine2"].Points.AddXY(0, 0);              // строится первая точка второй линии тренда;
                chart.Series["TrendLine2"].Points.AddXY(xLast, yLast2);     // строится вторая точка второй линии тренда;
            }  
        }

        private void Button_Click(object sender, RoutedEventArgs e)  // при клике на кнопку старта;
        {
            Timer_start();                                                      
            chart.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(yMin.Text);    // задаются минимум и максимум оси Y;
            chart.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(yMax.Text);
            chart.Series["TrendLine1"].Points.Clear();                          // очистка точек линий тренда;
            chart.Series["TrendLine2"].Points.Clear();
            chart.Invalidate();                                                 // перерисовка чарта;
        }

        private void XMovement()        // сдвиг оси X;
        {
            chart.ChartAreas[0].AxisX.Minimum = (chart.ChartAreas[0].AxisX.Maximum - Convert.ToDouble(xCount.Text));  // мин. на оси X сдигается в зависимости от макс. X;
        }

        private void firstCycle()       // первый чикл (понижение);
        {
            for (int i = 0; i < 15; i++)
            {
                xLast = xLast + 1;
                yLast1 = random.Next(yLast1 - 20, yLast1 + 5);          
                yLast2 = random.Next(yLast1 - 20, yLast1 + 5);
                chart.Series["Series1"].Points.AddXY(xLast, yLast1);     // добавление новых точек на основные линии;
                chart.Series["Series2"].Points.AddXY(xLast, yLast2);
                XMovement();                                             // вызов сдвига по оси X;
            }
            cycle = 1;                                                   // id для распределения очередности = 1Б чтобы сработал case 1 и выполнился следующий цикл;
        }

        private void secondCycle()      // второй цикл (немного прыгает, но толком не меняется);
        {
            for (int i = 0; i < 40; i++)
            {
                xLast = xLast + 1;
                yLast1 = random.Next(yLast1 - 14, yLast1 + 15);
                yLast2 = random.Next(yLast1 - 14, yLast1 + 15);
                chart.Series["Series1"].Points.AddXY(xLast, yLast1);
                chart.Series["Series2"].Points.AddXY(xLast, yLast2);
                XMovement();
            }
            cycle = 2;
        }

        private void thirdCycle()
        {
            for (int i = 0; i < 15; i++)
            {
                xLast = xLast + 1;
                yLast1 = random.Next(yLast1 - 5, yLast1 + 25);
                yLast2 = random.Next(yLast1 - 5, yLast1 + 25);
                chart.Series["Series1"].Points.AddXY(xLast, yLast1);
                chart.Series["Series2"].Points.AddXY(xLast, yLast2);
                XMovement();
            }
            cycle = 0;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            timer1.Stop();                          
            CreateTrend(comboBox1.SelectedIndex);   // передается индекс comboBox, чтобы нарисовать нужное количество линий тренда;
        }
    }
}
