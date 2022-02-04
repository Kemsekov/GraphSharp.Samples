using System.Diagnostics;
using GraphSharp.Nodes;
using GraphSharp.Edges;
using GraphSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using GraphSharp.GraphStructures;

public static class Helpers
{
    public static void MeasureTime(Action operation)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        System.Console.WriteLine("Starting operation");
        Console.ResetColor();
        var watch = new Stopwatch();
        watch.Start();
        operation();
        watch.Stop();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"End operation in {watch.ElapsedMilliseconds} Milliseconds");
        Console.ResetColor();
    }
    public static void ValidatePath(IList<INode> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            var current = path[i];
            var next = path[i + 1];
            if (!current.Edges.Select(x => x.Node.Id).Contains(next.Id))
                throw new Exception("Path is not valid!");
        }
    }
    public static void PrintPath(IList<INode> path)
    {
        System.Console.WriteLine("---Path");
        foreach (var n in path)
        {
            Console.WriteLine(n);
            foreach (var c in n.Edges)
            {
                if (c is NodeConnector con)
                    System.Console.WriteLine($"\t{con.Node} {(float)con.Weight}");
            }
        }
    }
    public static void CreateImage(IGraphStructure nodes, IList<INode>? path, ArgumentsHandler argz)
    {
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating image...");

            using var image = new Image<Rgba32>(argz.outputResolution, argz.outputResolution);
            var drawer = new GraphDrawer(image, Brushes.Solid(Color.Brown), Brushes.Solid(Color.BlueViolet), argz.fontSize);
            drawer.NodeSize = argz.nodeSize;
            drawer.Thickness = argz.thickness;
            drawer.Clear(Color.Black);
            drawer.DrawNodeConnections(nodes.Nodes);
            drawer.DrawNodes(nodes.Nodes);

            if (path?.Count > 0)
            {
                drawer.DrawLineBrush = Brushes.Solid(Color.Wheat);
                drawer.DrawPath(path);
            }
            System.Console.WriteLine("Saving image...");
            image.SaveAsJpeg("example.jpg");
        });
    }
    public static float ComputePathLength(IList<INode> path,Func<INode,INode,float> distance){
        float res = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            var current = path[i];
            var next = path[i + 1];
            res+=distance(current,next);
        }
        return res;
    }
    public static IGraphStructure CreateNodes(ArgumentsHandler argz)
    {
    GraphStructureOperation? result = default;
    Helpers.MeasureTime(() =>
    {
        System.Console.WriteLine("Creating nodes...");
        var rand = new Random(argz.nodeSeed >= 0 ? argz.nodeSeed : new Random().Next());
        var conRand = new Random(argz.connectionSeed >= 0 ? argz.connectionSeed : new Random().Next());

        result = new GraphStructure(id => new NodeXY(id, rand.NextDouble(), rand.NextDouble()), (node, parent) => new NodeConnector(node, parent), conRand)
            .CreateNodes(argz.nodesCount)
            .ForEach()
            .ConnectToClosest(argz.minEdges, argz.maxEdges, (node1, node2) => (float)((NodeXY)node1).Distance((NodeXY)node2));
    });
    return result ?? throw new Exception("Create node failure");;
}
}