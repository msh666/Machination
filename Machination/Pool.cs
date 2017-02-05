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
    class Pool : Figure
    {
        private Ellipse myEllipse;
        private double left;
        private double top;
        private static string type = "Pool";
        public double Resource { get; set; }
        public Pool(double x, double y) : base(x + 20, y + 20, type)
        {
            left = x;
            top = y;
            Resource = 0;
            myEllipse = new Ellipse
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Stroke = Brushes.Black,
                Width = 40,
                Height = 40
            };
        }
        public override void add(string name, Grid myGrid)
        {
            myEllipse.Name = name;
            myEllipse.Margin = new Thickness(left, top, 0, 0);
            myGrid.Children.Add(myEllipse);
        }

        public override void incrementResource(double inc)
        {
            if (Resource + inc < 0)
                Resource = 0;
            else
                Resource += inc;
        }
    }
}
