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
using QuickGraph;
using SciChart.Charting.Model.DataSeries;


namespace Machination
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static List<double> _graphList;
        AdjacencyGraph<Figure, TaggedEdge<Figure, double>> _g;
        private Pool _p1;
        Random _rnd = new Random();
        public MainWindow()
        {
            InitializeComponent();
            //var s1 = new Source(0, 0);
            //var d1 = new Drain(200, 200);
            //p1 = new Pool(100, 100);
            //g = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();

            //addElement("s1", myGrid, s1);
            //addElement("d1", myGrid, d1);
            //addElement("p1", myGrid, p1);
            //addConnection(s1, p1, myGrid, 2);
            //addConnection(p1, d1, myGrid, 1);

            //Run();
            //BestSolution();



            //Evolutionary programming
            Evolutionary evo = new Evolutionary();

        }

        private void BestSolution()
        {
            List<double> rmseList = new List<double>();
            List<List<double>> solutions = new List<List<double>>();
            for (var i = 0; i < 1000; i++)
            {
                _p1.Resource = 0;
                Run();
                solutions.Add(_graphList);
                rmseList.Add(RmseCalc());
            }
            double solution = rmseList.Min();
            var a = rmseList.IndexOf(solution);
            _graphList = solutions[a];
        }

        private void Run()
        {
            var a = 0;
            _graphList = new List<double>();
            while (a < 10)
            {
                foreach (var elem in _g.Edges)
                {
                    if (elem.Source.Type == "Source")
                        elem.Target.IncrementResource(_rnd.Next(1, 5));
                    if (elem.Target.Type == "Drain")
                        elem.Source.IncrementResource(-_rnd.Next(1, 5));
                }
                a++;
                _graphList.Add(_p1.Resource);
            }
        }

        public double RmseCalc()
        {
            XyDataSeries<double, double> idealGraph = DrawIdeal();
            XyDataSeries<double, double> graph = Draw();
            double score = 0;
            for (var i = 1; i <= 10; i++)
            {
                score = score + Math.Pow(idealGraph.YValues[i] - graph.YValues[i], 2);
            }
            return score = Math.Sqrt(score/10);
        }

        private void AddElement(string name, Grid myGrid, Figure elem)
        {
           // elem.add(name, myGrid);
            _g.AddVertex(elem);
        }

        private void AddConnection(Figure from, Figure to, Grid myGrid, double weight)
        {
            //var arrow = new Arrow();
            //arrow.addArrow(from, to, myGrid);
            _g.AddEdge(new TaggedEdge<Figure, double>(from, to, weight));
        }

        public static XyDataSeries<double, double> Draw()
        {
            var dataSeries = new XyDataSeries<double, double>();
            dataSeries.Append(0, 0);
            for (double i = 1; i <= _graphList.Count(); i++)
            {
                dataSeries.Append(i , _graphList[(Convert.ToInt32(i) - 1)]);
            }

            return dataSeries;
        }

        public XyDataSeries<double, double> DrawIdeal()
        {
            var idealData = new XyDataSeries<double, double>();

            idealData.Append(0, 0);
            for (int i = 1; i <= 10; i++)
            {
                idealData.Append(i, i);
            }

            return idealData;
        }

        private void ShowDiagram(object sender, RoutedEventArgs e)
        {
            //var cd = new ChartDisplay();
           // cd.Show();
            double score = RmseCalc();
        }       
    }
}
