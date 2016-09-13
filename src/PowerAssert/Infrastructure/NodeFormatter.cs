using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerAssert.Infrastructure.Nodes;

namespace PowerAssert.Infrastructure
{
    class NodeFormatter
    {
        const char pipe = '|';
        const char dot = '.';
        const char horz = '_';
        const char lhorz = '\\';
        const char rhorz = '/';
        const char intersect = ' ';

        internal static string[] Format(Node constantNode)
        {
            List<NodeInfo> nodeInfos;
            var textLine = PrettyPrint(constantNode, out nodeInfos);

            List<StringBuilder> lines = new List<StringBuilder>();

            List<int> stalks = new List<int>();
            foreach (var info in nodeInfos.OrderBy(x => x.Location))
            {
                var line = new StringBuilder().Append(' ', info.StalkOffset);
                stalks.ForEach(x => line[x] = pipe);
                stalks.Add(info.StalkOffset);
                line.Append(info.Value);
                lines.Add(line);
            }

            if (nodeInfos.Any())
            {
                for (int currDepth = nodeInfos.Max(x => x.Depth); currDepth >= 0; currDepth--)
                {
                    var line = new StringBuilder().Append(' ', nodeInfos.Max(x => x.Location + x.Width - 1) + 1);
                    nodeInfos.ForEach(x =>
                    {
                        if (x.Depth == currDepth && x.Width > 1)
                        {
                            if (x.Width > 2)
                            {
                                line[x.Location] = lhorz;
                                line[x.Location + x.Width - 1] = rhorz;
                                for (int w = 1; w < x.Width - 1; ++w)
                                {
                                    line[x.Location + w] = horz;
                                }

                                line[x.StalkOffset] = intersect;
                            }
                            else
                            {
                                line[x.Location] = line[x.Location + 1] = horz;
                            }
                        }
                        else if (x.Depth >= currDepth)
                        {
                            line[x.Location] = line[x.Location + x.Width - 1] = dot;
                        }
                        else
                        {
                            line[x.StalkOffset] = pipe;
                        }
                    });
                    lines.Add(line);
                }
            }

            lines.Add(textLine);

            return lines
                .AsEnumerable()
                .Reverse()
                .Select(x => x.ToString().TrimEnd())
                .Where(x => x.Length > 0)
                .ToArray();
        }

        public static string PrettyPrint(Node node)
        {
            List<NodeInfo> ignored;
            return PrettyPrint(node, out ignored).ToString();
        }

        static StringBuilder PrettyPrint(Node constantNode, out List<NodeInfo> outNodeInfos)
        {
            StringBuilder textLine = new StringBuilder();
            var nodeInfos = new List<NodeInfo>();

            constantNode.Walk((text, value, depth) =>
            {
                if (value != null)
                {
                    nodeInfos.Add(new NodeInfo {Location = textLine.Length, Width = text.Length, Value = value, Depth = depth});
                }
                textLine.Append(text);
            }, 0, false);

            outNodeInfos = nodeInfos;
            return textLine;
        }

        class NodeInfo
        {
            public int Location { get; set; }
            public string Value { get; set; }
            public int Depth { get; set; }
            public int Width { get; set; }

            public int StalkOffset
            {
                get { return Location + (Width - 1)/2; }
            }
        }
    }
}