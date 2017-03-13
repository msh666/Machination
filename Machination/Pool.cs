using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using QuickGraph.Graphviz.Dot;

namespace Machination
{
    class Pool : Figure
    {
        private static string _type = "Pool";
        //public double Resource { get; set; }
        public Pool() : base( _type)
        {
            Resource = 0;
            Shape = GraphvizVertexShape.Circle;
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
