using System.Diagnostics;
using GraphSharp.Nodes;
using GraphSharp.Edges;
using GraphSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using GraphSharp.GraphStructures;
using GraphType = GraphSharp.GraphStructures.GraphStructureBase<NodeXY,NodeConnector>;
using SampleBase;

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
    public static void EnsureRightColoring(IList<NodeXY> nodes){
        foreach(var n in nodes){
            var color = n.Color;
            if(n.Edges.Any(x=>x.Node.Color==color)){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Wrong graph coloring! {n} with color {n.Color} have edge with the same color!");               
                Console.ResetColor();
                return;
            }
        }
    }
    public static void ShiftNodesToFitInTheImage(IList<NodeXY> nodes){
        foreach(var n in nodes){
            n.X*=0.9;
            n.Y*=0.9;
            n.X+=0.05;
            n.Y+=0.05;
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
    public static void CreateImage(ArgumentsHandler argz,Action<GraphDrawer> draw)
    {
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating image...");

            using var image = new Image<Rgba32>(argz.outputResolution, argz.outputResolution);
            var drawer = new GraphDrawer(image, argz.fontSize);
            drawer.NodeSize = argz.nodeSize;
            drawer.Thickness = argz.thickness;
            draw(drawer);
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
    public static GraphType CreateNodes(ArgumentsHandler argz)
    {
        GraphStructure<NodeXY,NodeConnector>? result = default;
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating nodes...");
            var rand = new Random(argz.nodeSeed >= 0 ? argz.nodeSeed : new Random().Next());
            var conRand = new Random(argz.connectionSeed >= 0 ? argz.connectionSeed : new Random().Next());

            var config = new SampleGraphConfiguration(){
                CreateNodesRand = rand,
                CreateEdgesRand = conRand
            };
    
            result = new GraphStructure<NodeXY,NodeConnector>(config)
                .CreateNodes(argz.nodesCount);
            result.ForEach()
                .ConnectToClosest(argz.minEdges, argz.maxEdges);
            ShiftNodesToFitInTheImage(result.Nodes);
        });
        return result ?? throw new Exception("Create node failure");;
    }
}