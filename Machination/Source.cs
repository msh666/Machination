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
    class Source : Figure
    {
        private static string _type = "Source";
        public Source() : base(_type)
        {
            Shape = GraphvizVertexShape.Triangle;
        }
    }
}
