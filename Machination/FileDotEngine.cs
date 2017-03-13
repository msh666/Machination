using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace Machination
{
    public sealed class FileDotEngine : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            string output = outputFileName;
            File.WriteAllText(output, dot);

            // assumes dot.exe is on the path:
            var args = string.Format(@"{0} -Tjpg -O", output);
            System.Diagnostics.Process.Start("C:/repository/Machination/packages/Graphviz.2.38.0.2/dot.exe", args);
            return output;
        }
    }
}
