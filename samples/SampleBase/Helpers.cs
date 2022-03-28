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
    public static void ValidatePath(IList<NodeXY> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            var current = path[i];
            var next = path[i + 1];
            if (!current.Edges.Select(x => x.Node.Id).Contains(next.Id))
                throw new Exception("Path is not valid!");
        }
    }
    public static void PrintPath(IList<NodeXY> path)
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
    public static void CreateImage(IGraphStructure<NodeXY> nodes, IList<NodeXY>? path, ArgumentsHandler argz)
    {
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating image...");

            using var image = new Image<Rgba32>(argz.outputResolution, argz.outputResolution);
            var drawer = new GraphDrawer(image, argz.fontSize);
            drawer.NodeSize = argz.nodeSize;
            drawer.Thickness = argz.thickness;
            drawer.Clear(Color.Black);
            drawer.DrawEdges(nodes.Nodes);
            drawer.DrawNodes(nodes.Nodes);
            if (path is not null && path?.Count > 0)
            {
                drawer.DrawPath(path,Color.Wheat);
            }
            System.Console.WriteLine("Saving image...");
            image.SaveAsJpeg(argz.filename);
        });
    }
    public static float ComputePathLength(IList<NodeXY> path,Func<NodeXY,NodeXY,float> distance){
        float res = 0;
        for (int i = 0; i < path.Count - 1; i++)
        {
            var current = path[i];
            var next = path[i + 1];
            res+=distance(current,next);
        }
        return res;
    }
    public static IGraphStructure<NodeXY> CreateNodes(ArgumentsHandler argz)
    {
    GraphStructureOperation<NodeXY,NodeConnector>? result = default;
    MeasureTime(() =>
    {
        System.Console.WriteLine("Creating nodes...");
        var rand = new Random(argz.nodeSeed >= 0 ? argz.nodeSeed : new Random().Next());
        var conRand = new Random(argz.connectionSeed >= 0 ? argz.connectionSeed : new Random().Next());

        var createEdge = (NodeXY parent,NodeXY node) => new NodeConnector(parent,node);
        var createNode = (int id) => new NodeXY(id, rand.NextDouble(), rand.NextDouble());
        var distance = (NodeXY node1,NodeXY node2) => (float)((NodeXY)node1).Distance((NodeXY)node2);


        result = new GraphStructure<NodeXY,NodeConnector>(createNode,createEdge)
        {
            Distance = distance,
            Rand = conRand,
        }
            .CreateNodes(argz.nodesCount)
            .ForEach()
            .ConnectToClosest(argz.minEdges, argz.maxEdges);
    });
    return result ?? throw new Exception("Create node failure");;
}
}