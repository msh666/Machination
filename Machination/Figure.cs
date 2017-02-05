using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Machination
{
    class Figure
    {

        public Figure(double X, double Y, string Type)
        {
            this.X = X;
            this.Y = Y;
            this.Type = Type;
        }

        public double Resource { get; }
        public double X { get; }
        public double Y { get; }
        public string Type { get; }

        public virtual void incrementResource(double inc)
        {

        }

        public virtual void add(string name, Grid myGrid)
        {

        }
    }
}
