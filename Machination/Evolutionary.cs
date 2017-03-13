using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Newtonsoft.Json;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using SciChart.Charting.Common.Extensions;
using SciChart.Charting.Model.DataSeries;

namespace Machination
{
    class Evolutionary
    {
        private List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>> _childPopulation;
        private Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double> _population;
        private Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double> _scores;
        private AdjacencyGraph<Figure, TaggedEdge<Figure, double>> _initialGraph;
        Random _rnd = new Random();
        private static List<double> _resourceList;
        Pool _p1 = new Pool();

        public Evolutionary()
        {
            _initialGraph = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();
            _scores = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();
            _population = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();
            _childPopulation = new List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>>();
            AddElement(_p1, _initialGraph);

            //var s1 = new Source();
            //var p2 = new Pool();
            //var g1 = new Gate(true);

            //AddElement(s1, _initialGraph);
            //AddElement(p2, _initialGraph);
            //AddElement(g1, _initialGraph);

            //AddConnection(s1, g1, 2, _initialGraph);
            //AddConnection(g1, _p1, 2, _initialGraph);
            //AddConnection(g1, p2, 2, _initialGraph);


            //var d1 = new Drain();
            //AddElement(d1, _initialGraph);
            //AddConnection(_p1, d1, 1, _initialGraph);

            Run(_initialGraph, _p1);
            double scoreBest = RmseCalc();
            _population.Add(_initialGraph, scoreBest);
            var flag = 0;

            //initialize population of 1000 random mutated graphs
            for (int i = 0; i < 1000; i++)
            {
                var tempGraph = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();
                AddElement(_p1, tempGraph);
                var to = _rnd.Next(3, 5);
                for (int j = 0; j < to; j++)
                {
                    tempGraph = Mutate(tempGraph);
                }
                _childPopulation.Add(tempGraph);
            }

            //calculate fitness for initial population
            GetBestSolution(_childPopulation);
            _scores.ToList().ForEach(x => _population.Add(x.Key, x.Value));
            _scores = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();

            //The process of EP work until not find the solution
            while (scoreBest >= 0.2)
            {
                _childPopulation = new List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>>();
                var parentList = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();

                //this part need to clone population collection
                foreach (var item in _population)
                {
                    _initialGraph = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();
                    _initialGraph = item.Key;
                    var dublicateGraph = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();
                    for (int i = 0; i < _initialGraph.VertexCount; i++)
                    {
                        AddElement(_initialGraph.Vertices.ElementAt(i), dublicateGraph);
                    }
                    for (int i = 0; i < _initialGraph.EdgeCount; i++)
                    {
                        AddConnection(_initialGraph.Edges.ElementAt(i).Source, _initialGraph.Edges.ElementAt(i).Target, _initialGraph.Edges.ElementAt(i).Tag, dublicateGraph);
                    }
                    var score = item.Value;
                    parentList.Add(dublicateGraph, score);
                }

                //apply mutation for all parents
                foreach (var parent in parentList)
                {
                    _childPopulation.Add(Mutate(parent.Key));
                }
                //calculate fintess for child population
                GetBestSolution(_childPopulation);
                //put child population and parent population together
                _scores.ToList().ForEach(x => _population.Add(x.Key, x.Value));
                //sort population by fitness and take (n) elements
                _population = _population.OrderBy(x => x.Value).Take(100000).ToDictionary(x => x.Key, x => x.Value);
                 //_population = _population.Take(100).ToDictionary(x => x.Key, x => x.Value);

                 //if fitness score of first element of sorted population is better than current best fitness then new score become the best
                if (_population.First().Value < scoreBest)
                    scoreBest = _population.First().Value;
                _scores = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();
                _childPopulation = new List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>>();

                //need for debugging
                flag++;
                if (flag % 10 == 0)
                {
                    
                }
            }

            //Graphviz output

            //var graphviz = new GraphvizAlgorithm<Figure, TaggedEdge<Figure, double>>(_population.First().Key);
            //graphviz.FormatEdge += delegate (object sender, FormatEdgeEventArgs<Figure, TaggedEdge<Figure, double>> e)
            //{
            //    e.EdgeFormatter.Label.Value = e.Edge.Tag.ToString();
            //};
            //graphviz.FormatVertex +=
            //    delegate (object sender, FormatVertexEventArgs<Figure> e)
            //    {
            //        e.VertexFormatter.Label = e.Vertex.Type;
            //        e.VertexFormatter.Shape = e.Vertex.Shape;
            //        e.VertexFormatter.FixedSize = true;
            //        e.VertexFormatter.Size = new GraphvizSizeF(1f, 1f);
            //    };
            //string output = graphviz.Generate(new FileDotEngine(), "graph");
        }

        //Fitness calculation of collection
        private void GetBestSolution (List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>> childPopulation)
        {
            foreach (var elem in childPopulation)
            {
                Run(elem, _p1);
                var rmse = RmseCalc();
                    _scores.Add(elem, rmse);
            }
        }

        //Graph mutation
        private AdjacencyGraph<Figure, TaggedEdge<Figure, double>> Mutate(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> parent)
        {
            AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child = parent;
            if (child.VertexCount == 1 && child.EdgeCount == 0)
            {
                AddElement(child);
            }
            else if (child.VertexCount == 1)
            {
                switch (_rnd.Next(0, 2))
                {
                    case 0:
                        AddElement(child);
                        break;
                    case 1:
                        ChangeWeight(child);
                        break;
                }
            }
            else if (child.EdgeCount == 0)
            {
                switch (_rnd.Next(0, 2))
                {
                    case 0:
                        AddElement(child);
                        break;
                    case 1:
                        DeleteElement(child);
                        break;
                }
            }
            else
            {
                switch (_rnd.Next(0, 3))
                {
                    case 0:
                        AddElement(child);
                        break;
                    case 1:
                        DeleteElement(child);
                        break;
                    case 2:
                        ChangeWeight(child);
                        break;
                }
            }

            return child;
        }

        private void AddElement(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child)
        {
            switch (_rnd.Next(0, 4))
            {
                case 0:
                    child.AddVertex(new Pool());
                    switch (_rnd.Next(0,2))
                    {
                        case 0:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), child.Vertices.Last(), 1)); //Add new element from the left side
                            break;
                        case 1:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.Last(), child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), 1)); //Add new element from the right side
                            break;
                    }
                    break;
                case 1:
                    child.AddVertex(new Source());
                    switch (_rnd.Next(0, 2))
                    {
                        case 0:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), child.Vertices.Last(), 1));
                            break;
                        case 1:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.Last(), child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), 1));
                            break;
                    }
                    break;
                case 2:
                    child.AddVertex(new Drain());
                    switch (_rnd.Next(0, 2))
                    {
                        case 0:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), child.Vertices.Last(), 1));
                            break;
                        case 1:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.Last(), child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), 1));
                            break;
                    }
                    break;
                case 3:
                    child.AddVertex(new Gate(true));
                    switch (_rnd.Next(0, 2))
                    {
                        case 0:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), child.Vertices.Last(), 1));
                            break;
                        case 1:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.Last(), child.Vertices.ElementAt(_rnd.Next(0, child.VertexCount - 1)), 1));
                            break;
                    }
                    break;
            }
        }

        private void DeleteElement(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child)
        {
            var randomDelete = _rnd.Next(0, child.VertexCount);
            while (child.Vertices.ElementAt(randomDelete) == _p1)
            {
                randomDelete = _rnd.Next(0, child.VertexCount);
            }
            child.RemoveVertex(child.Vertices.ElementAt(randomDelete));
        }

        private void ChangeWeight(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child)
        {
            switch (_rnd.Next(0,2))
            {
                case 0:
                    child.Edges.ElementAt(_rnd.Next(0, child.EdgeCount)).Tag++;
                    break;
                case 1:
                    int edgeNumber = _rnd.Next(_rnd.Next(0, child.EdgeCount));
                    if(child.Edges.ElementAt(edgeNumber).Tag > 0)
                        child.Edges.ElementAt(edgeNumber).Tag--;
                    break;
            }
        }

        //Simulate process of graph execution
        private void Run(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> g, Pool p1)
        {
            var a = 0;
            p1.Resource = 0;
            _resourceList = new List<double>();

            //Serch for Gates and their outputs
            foreach (var vertex in g.Vertices)
            {
                if (vertex.Type == "Gate")
                {
                    var v = vertex; 
                    v.InitializeQueue();
                    foreach (var edge in g.Edges)
                    {
                        if (edge.Source == vertex)
                        {
                            for (int i = 0; i < edge.Tag; i++)
                            {
                                vertex.AddToQueue(edge);
                            }
                        }
                    }
                }
            }


            while (a < 10)
            {
                foreach (var elem in g.Edges)
                {
                    if (elem.Source.Type == "Source")
                        elem.Target.IncrementResource(elem.Tag);
                    if (elem.Target.Type == "Drain")
                        elem.Source.IncrementResource(-elem.Tag);
                    if (elem.Source.Type == "Gate")
                    {
                        if (elem.Tag != 0)
                        {
                            while (elem.Source.Resource > 0)
                            {
                                var edge = elem.Source.TakeElement();
                                edge.Target.IncrementResource(1);
                                elem.Source.IncrementResource(-1);
                            }
                        }
                    }
                }
                a++;
                _resourceList.Add(p1.Resource);
            }
        }

        //Calculate rmse for graph
        public double RmseCalc()
        {
            XyDataSeries<double, double> idealGraph = IdealResult();
            XyDataSeries<double, double> graph = CurrentResult();
            double score = 0;
            for (var i = 1; i <= 10; i++)
            {
                score = score + Math.Pow(idealGraph.YValues[i] - graph.YValues[i], 2);
            }
            return score = Math.Sqrt(score / 10);
        }

        private void AddElement(Figure elem, AdjacencyGraph<Figure, TaggedEdge<Figure, double>> g)
        {
            g.AddVertex(elem);
        }

        private void AddConnection(Figure from, Figure to, double weight, AdjacencyGraph<Figure, TaggedEdge<Figure, double>> g)
        {
           // var arrow = new Arrow();
            g.AddEdge(new TaggedEdge<Figure, double>(from, to, weight));
        }

        //Initialize ideal input graph
        public XyDataSeries<double, double> IdealResult()
        {
            var idealData = new XyDataSeries<double, double>();

            //for (int i = 0; i <= 10; i++)
            //{
            //    idealData.Append(i, i);
            //}
            idealData.Append(0, 0);
            idealData.Append(1, 2);
            idealData.Append(2, 2);
            idealData.Append(3, 4);
            idealData.Append(4, 4);
            idealData.Append(5, 6);
            idealData.Append(6, 6);
            idealData.Append(7, 8);
            idealData.Append(8, 8);
            idealData.Append(9, 10);
            idealData.Append(10, 10);
            return idealData;
        }
        //Build 2 demention chart for graph to compare it with input graph
        public static XyDataSeries<double, double> CurrentResult()
        {
            var dataSeries = new XyDataSeries<double, double>();
            dataSeries.Append(0, 0);
            for (double i = 1; i <= _resourceList.Count(); i++)
            {
                dataSeries.Append(i, _resourceList[(Convert.ToInt32(i) - 1)]);
            }

            return dataSeries;
        }
    }
}
