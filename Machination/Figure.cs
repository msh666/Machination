using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using QuickGraph;
using QuickGraph.Graphviz.Dot;

namespace Machination
{
    class Figure
    {

        public Figure(string type)
        {
            this.Type = type;
            Shape = GraphvizVertexShape.Circle;
        }

        public double Resource { get; set; }
        public string Type { get; }
        public GraphvizVertexShape Shape { get; set; }

        public virtual void IncrementResource(double inc)
        {
        }

        public virtual void AddToQueue(TaggedEdge<Figure, double> f)
        {
        }

        public virtual void InitializeQueue()
        {
        }

        public virtual TaggedEdge<Figure, double> TakeElement()
        {
            return null;
        }
    }
}
