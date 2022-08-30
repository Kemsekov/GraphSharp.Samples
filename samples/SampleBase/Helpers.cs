using System.Diagnostics;
using GraphSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using GraphSharp.Graphs;
using Newtonsoft.Json;
using SampleBase;
using System.Text;
using GraphSharp.Common;
using System.Numerics;
using GraphSharp.GraphDrawer;
using SixLabors.ImageSharp.Processing;

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
    public static void PrintPath<TNode>(IList<TNode> path)
    where TNode : INode
    {
        System.Console.WriteLine("-------------------");
        foreach(var p in path){
            System.Console.WriteLine(p);
        }
    }
    public static void ShiftNodesToFitInTheImage<TNode>(IEnumerable<TNode> nodes)
    where TNode : INode, IPositioned
    {
        foreach(var n in nodes){
            var newPos = new Vector2(n.Position.X*0.9f+0.05f,n.Position.Y*0.9f+0.05f);
            n.Position = newPos;
        }
    }
    public static void CreateImage<TNode,TEdge>(ArgumentsHandler argz,IGraph<TNode,TEdge> graph,Action<GraphDrawer<TNode,TEdge>> draw)
    where TNode : INode
    where TEdge : IEdge
    {
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating image...");

            using var image = new Image<Rgba32>(argz.outputResolution, argz.outputResolution);
            image.Mutate(x=>{
                var shapeDrawer = new ImageSharpShapeDrawer(x,image, argz.fontSize);
                var drawer = new GraphDrawer<TNode,TEdge>(graph,shapeDrawer);
                draw(drawer);
            });
            System.Console.WriteLine("Saving image...");
            image.SaveAsJpeg(argz.filename);
        });
    }
    public static float ComputePathLength<TNode>(IEnumerable<TNode> path)
    where TNode : INode, IPositioned
    {
        float res = 0;
        var current = path.GetEnumerator();
        if(!current.MoveNext())
            return 0;
        var previous = current.Current;
        while(current.MoveNext()){
            res += (float)Vector2.Distance(previous.Position,current.Current.Position);
            previous = current.Current;
        }
        return res;
    }
    public static Graph CreateGraph(ArgumentsHandler argz)
    {
        Graph? result = default;
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating graph...");
            var rand = new Random(argz.nodeSeed >= 0 ? argz.nodeSeed : new Random().Next());
            var conRand = new Random(argz.connectionSeed >= 0 ? argz.connectionSeed : new Random().Next());

    
            result = new Graph(id=>new(id){Position=new(rand.NextSingle(),rand.NextSingle())},(n1,n2)=>new(n1,n2){Weight = (n1.Position-n2.Position).Length()});
            result.Configuration.Rand = conRand;
            result.CreateNodes(argz.nodesCount);
            result.Do.ConnectToClosest(argz.minEdges, argz.maxEdges);
        });
        return result ?? throw new Exception("Create node failure");;
    }

    /// <summary>
    /// Save graph to json file
    /// </summary>
    public static void SaveGraph(Graph graph,string filename)
    {
        MeasureTime(() =>
        {
            System.Console.WriteLine("Saving graph...");
            var to_save = new {
                Nodes=graph.Nodes,
                Edges=graph.Edges
            };
            var json = JsonConvert.SerializeObject(to_save, Formatting.Indented);
            System.IO.File.WriteAllText(filename, json);
        });
    }

    //Load graph from json file
    public static Graph LoadGraph(string filename)
    {
        throw new NotImplementedException();
    }

   

}