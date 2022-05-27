using System.Diagnostics;
using GraphSharp.Nodes;
using GraphSharp.Edges;
using GraphSharp;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing.Processing;
using GraphSharp.GraphStructures;
using GraphType = GraphSharp.GraphStructures.GraphStructureBase<NodeXY,NodeConnector>;
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
    public static void ValidatePath<TNode,TEdge>(this IGraphStructure<TNode,TEdge> graph,IList<NodeXY> path)
    where TNode : INode
    where TEdge : IEdge
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            var current = path[i];
            var next = path[i + 1];
            if(!graph.Edges.TryGetEdge(current.Id,next.Id,out var edge)){
                throw new System.Exception($"Edge {current.Id}-{next.Id} not found! Path is not valid!");
            }
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
    public static void CreateImage<TNode,TEdge>(ArgumentsHandler argz,IGraphConfiguration<TNode,TEdge> configuration,Action<GraphDrawer<TNode,TEdge>> draw)
    where TNode : NodeBase<TEdge>
    where TEdge : EdgeBase<TNode>
    {
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating image...");

            using var image = new Image<Rgba32>(argz.outputResolution, argz.outputResolution);
            image.Mutate(x=>{
                var shapeDrawer = new ImageSharpShapeDrawer(x,image, argz.fontSize);
                var drawer = new GraphDrawer<TNode,TEdge>(configuration,shapeDrawer);
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
    public static GraphType CreateGraph(ArgumentsHandler argz)
    {
        GraphStructure<NodeXY,NodeConnector>? result = default;
        MeasureTime(() =>
        {
            System.Console.WriteLine("Creating nodes...");
            var rand = new Random(argz.nodeSeed >= 0 ? argz.nodeSeed : new Random().Next());
            var conRand = new Random(argz.connectionSeed >= 0 ? argz.connectionSeed : new Random().Next());

            var config = new SampleGraphConfiguration(rand){
                CreateNodesRand = rand,
                CreateEdgesRand = conRand
            };
    
            result = new GraphStructure<NodeXY,NodeConnector>(config)
                .Create(argz.nodesCount);
            result.Do.ConnectToClosest(argz.minEdges, argz.maxEdges);
        });
        return result ?? throw new Exception("Create node failure");;
    }

    /// <summary>
    /// Save graph to json file
    /// </summary>
    public static void SaveGraph(GraphType graph,string filename)
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
    public static GraphType LoadGraph(string filename)
    {
        throw new NotImplementedException();
    }

}