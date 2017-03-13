using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Graphviz.Dot;

namespace Machination
{
    class Gate : Figure
    {
        private static string _type = "Gate";
        private bool _random;
        //public new double Resource { get; set; }
        private Queue<TaggedEdge<Figure, double>> _targetEdges = new Queue<TaggedEdge<Figure, double>>();       
        public Gate(bool random) : base(_type)
        {
            this._random = random;
            Resource = 0;
            Shape = GraphvizVertexShape.Box;
        }

        public override void AddToQueue(TaggedEdge<Figure, double> f)
        {
            _targetEdges.Enqueue(f);
        }

        public override void InitializeQueue()
        {
            _targetEdges = new Queue<TaggedEdge<Figure, double>>();
        }

        public override TaggedEdge<Figure, double> TakeElement()
        {
            var edge = _targetEdges.Dequeue();
            _targetEdges.Enqueue(edge);
            return edge;
        }

        public override void IncrementResource(double inc)
        {
            if (Resource + inc < 0)
                Resource = 0;
            else
                Resource += inc;
        }
    }
}
