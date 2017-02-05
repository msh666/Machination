using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Machination
{
    class Source : Figure
    {
        private Polygon poly;
        private double left;
        private double top;
        private static string type = "Source";
        public Source(double x, double y) : base(x + 20, y + 18, type)
        {
            left = x;
            top = y;
            poly = new Polygon
            {
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
        }

        public override void add(string name, Grid myGrid)
        {           
            poly.Name = name;
            var point1 = new Point(0 + left, 35 + top);
            var point2 = new Point(20 + left, 0 + top);
            var point3 = new Point(40 + left, 35 + top);
            var myPointCollection = new PointCollection {point1, point2, point3};
            poly.Points = myPointCollection;
            myGrid.Children.Add(poly);
        }
    }
}
