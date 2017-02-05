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
using SciChart.Charting.Common.Extensions;
using SciChart.Charting.Model.DataSeries;

namespace Machination
{
    class Evolutionary
    {
        //private List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>> population;
        private List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>> childPopulation;
        private Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double> population;
       // private Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double> childPopulation;
        private List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>> union;
        private Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double> scores;
        private AdjacencyGraph<Figure, TaggedEdge<Figure, double>> initialGraph;
        Random rnd = new Random();
        private static List<double> resourceList;
        Pool p1 = new Pool(0, 0);


        //public static T Clone<T>(T source)
        //{
        //    var serialized = JsonConvert.SerializeObject(source);
        //    return JsonConvert.DeserializeObject<T>(serialized);
        //}

        public static T Clone<T>(T source)
        { 
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public Evolutionary()
        {
            initialGraph = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();
            scores = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();
            population = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();
            addElement(p1, initialGraph);
            //var d1 = new Drain(0,0);
            //addElement(d1, initialGraph );
            //addConnection(p1,d1, 0, initialGraph);
            Run(initialGraph, p1);
            double scoreBest = RMSECalc();
            population.Add(initialGraph, scoreBest);
            while (scoreBest >= 1.0)
            {
                childPopulation = new List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>>();
                var parentList = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();
                //parentList = Clone(population);

                foreach (var item in population)
                {
                    initialGraph = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();
                    initialGraph = item.Key;
                    var dublicateGraph = new AdjacencyGraph<Figure, TaggedEdge<Figure, double>>();
                    for (int i = 0; i < initialGraph.VertexCount; i++)
                    {
                        addElement(initialGraph.Vertices.ElementAt(i), dublicateGraph);
                    }
                    for (int i = 0; i < initialGraph.EdgeCount; i++)
                    {
                        addConnection(initialGraph.Edges.ElementAt(i).Source, initialGraph.Edges.ElementAt(i).Target, initialGraph.Edges.ElementAt(i).Tag, dublicateGraph);
                    }
                    var score = item.Value;
                    parentList.Add(dublicateGraph, score);
                }
                //var a = population.Keys.First();
                //initialGraph.AddVertex(a.Vertices.First());
                //parentList.Add(initialGraph,0);


                foreach (var parent in parentList)
                {
                    childPopulation.Add(Mutate(parent.Key));
                }
                GetBestSolution(childPopulation);
                scores = scores.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                if (scores.First().Value < scoreBest)
                    scoreBest = scores.First().Value;
                scores.ToList().ForEach(x => population.Add(x.Key, x.Value));
                population.Take(50);
                scores = new Dictionary<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>, double>();
                childPopulation = new List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>>();
            }
        }

        private void GetBestSolution (List<AdjacencyGraph<Figure, TaggedEdge<Figure, double>>> childPopulation)
        {
            foreach (var elem in childPopulation)
            {
                Run(elem, p1);
                scores.Add(elem, RMSECalc());
            }
        }

        private AdjacencyGraph<Figure, TaggedEdge<Figure, double>> Mutate(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> parent)
        {
            AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child = parent;
            if (child.VertexCount == 1 && child.EdgeCount == 0)
            {
                addElement(child);
            }
            else if (child.VertexCount == 1)
            {
                switch (rnd.Next(0, 2))
                {
                    case 0:
                        addElement(child);
                        break;
                    case 1:
                        changeWeight(child);
                        break;
                }
            }
            else if (child.EdgeCount == 0)
            {
                switch (rnd.Next(0, 2))
                {
                    case 0:
                        addElement(child);
                        break;
                    case 1:
                        deleteElement(child);
                        break;
                }
            }
            else
            {
                switch (rnd.Next(0, 3))
                {
                    case 0:
                        addElement(child);
                        break;
                    case 1:
                        deleteElement(child);
                        break;
                    case 2:
                        changeWeight(child);
                        break;
                }
            }

            return child;
        }

        private void addElement(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child)
        {
            switch (rnd.Next(0, 3))
            {
                case 0:
                    child.AddVertex(new Pool(0, 0));
                    switch (rnd.Next(0,2))
                    {
                        case 0:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.ElementAt(rnd.Next(0, child.VertexCount - 1)), child.Vertices.Last(), 1));
                            break;
                        case 1:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.Last(), child.Vertices.ElementAt(rnd.Next(0, child.VertexCount - 1)), 1));
                            break;
                    }
                    break;
                case 1:
                    child.AddVertex(new Source(0, 0));
                    switch (rnd.Next(0, 2))
                    {
                        case 0:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.ElementAt(rnd.Next(0, child.VertexCount - 1)), child.Vertices.Last(), 1));
                            break;
                        case 1:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.Last(), child.Vertices.ElementAt(rnd.Next(0, child.VertexCount - 1)), 1));
                            break;
                    }
                    break;
                case 2:
                    child.AddVertex(new Drain(0, 0));
                    switch (rnd.Next(0, 2))
                    {
                        case 0:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.ElementAt(rnd.Next(0, child.VertexCount - 1)), child.Vertices.Last(), 1));
                            break;
                        case 1:
                            child.AddEdge(new TaggedEdge<Figure, double>(child.Vertices.Last(), child.Vertices.ElementAt(rnd.Next(0, child.VertexCount - 1)), 1));
                            break;
                    }
                    break;
            }
        }

        private void deleteElement(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child)
        {
            var randomDelete = rnd.Next(0, child.VertexCount);
            while (child.Vertices.ElementAt(randomDelete) == p1)
            {
                randomDelete = rnd.Next(0, child.VertexCount);
            }
            child.RemoveVertex(child.Vertices.ElementAt(randomDelete));
        }

        private void changeWeight(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> child)
        {
            switch (rnd.Next(0,2))
            {
                case 0:
                    child.Edges.ElementAt(rnd.Next(0, child.EdgeCount)).Tag++;
                    break;
                case 1:
                    int edgeNumber = rnd.Next(rnd.Next(0, child.EdgeCount));
                    if(child.Edges.ElementAt(edgeNumber).Tag > 0)
                        child.Edges.ElementAt(edgeNumber).Tag--;
                    break;
            }
        }
        private void Run(AdjacencyGraph<Figure, TaggedEdge<Figure, double>> g, Pool p1)
        {
            var a = 0;
            p1.Resource = 0;
            resourceList = new List<double>();
            while (a < 10)
            {
                foreach (var elem in g.Edges)
                {
                    if (elem.Source.Type == "Source")
                        elem.Target.incrementResource(elem.Tag);
                    if (elem.Target.Type == "Drain")
                        elem.Source.incrementResource(-elem.Tag);
                }
                a++;
                resourceList.Add(p1.Resource);
            }
        }

        public double RMSECalc()
        {
            XyDataSeries<double, double> idealGraph = idealResult();
            XyDataSeries<double, double> graph = currentResult();
            double score = 0;
            for (var i = 1; i <= 10; i++)
            {
                score = score + Math.Pow(idealGraph.YValues[i] - graph.YValues[i], 2);
            }
            return score = Math.Sqrt(score / 10);
        }

        private void addElement(Figure elem, AdjacencyGraph<Figure, TaggedEdge<Figure, double>> g)
        {
            g.AddVertex(elem);
        }

        private void addConnection(Figure from, Figure to, double weight, AdjacencyGraph<Figure, TaggedEdge<Figure, double>> g)
        {
            var arrow = new Arrow();
            g.AddEdge(new TaggedEdge<Figure, double>(from, to, weight));
        }

        public XyDataSeries<double, double> idealResult()
        {
            var idealData = new XyDataSeries<double, double>();

            for (int i = 0; i <= 10; i++)
            {
                idealData.Append(i, i);
            }

            return idealData;
        }
        public static XyDataSeries<double, double> currentResult()
        {
            var dataSeries = new XyDataSeries<double, double>();
            dataSeries.Append(0, 0);
            for (double i = 1; i <= resourceList.Count(); i++)
            {
                dataSeries.Append(i, resourceList[(Convert.ToInt32(i) - 1)]);
            }

            return dataSeries;
        }
    }
}
