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
using GraphSharp.Models;

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
            if (!current.Edges.Select(x => x.Child.Id).Contains(next.Id))
                throw new Exception("Path is not valid!");
        }
    }
    public static void EnsureRightColoring(IList<NodeXY> nodes){
        foreach(var n in nodes){
            var color = n.Color;
            if(n.Edges.Any(x=>x.Child.Color==color)){
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Wrong graph coloring! {n} with color {n.Color} have edge with the same color!");               
                Console.ResetColor();
                return;
            }
        }
    }
    public static void ShiftNodesToFitInTheImage(IList<NodeXY> nodes){
        foreach(var n in nodes){
            n.X*=0.9f;
            n.Y*=0.9f;
            n.X+=0.05f;
            n.Y+=0.05f;
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
                    System.Console.WriteLine($"\t{con.Child} {(float)con.Weight}");
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
    public static GraphType CreateGraph(ArgumentsHandler argz)
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

    /// <summary>
    /// Save graph to json file
    /// </summary>
    public static void SaveGraph(GraphType graph,string filename)
    {
        MeasureTime(() =>
        {
            System.Console.WriteLine("Saving graph...");
            var edges = graph.Nodes.SelectMany(
                x=>x.Edges.Select(
                    e=>new{
                        ParentId=x.Id,
                        ChildId=e.Child.Id,
                        Weight=e.Weight,
                        Color=e.Color.ToString()
                    }));
            var nodes = graph.Nodes.Select(x=>new{
                Id=x.Id,
                X=x.X,
                Y=x.Y,
                Weight = x.Weight,
                Color=x.Color.ToString()
            });
            var to_save = new {
                Nodes=nodes,
                Edges=edges
            };
            var json = JsonConvert.SerializeObject(to_save, Formatting.Indented);
            System.IO.File.WriteAllText(filename, json);
        });
    }

    //Load graph from json file
    public static GraphType LoadGraph(string filename)
    {
        #pragma warning disable
        GraphType? result = default;
        MeasureTime(() =>
        {
            System.Console.WriteLine("Loading graph...");
            var json = System.IO.File.ReadAllText(filename);
            var loaded = JsonConvert.DeserializeObject<dynamic>(json);
            var nodes = (loaded.Nodes as IEnumerable<dynamic>).Select(
                x=> {
                    int id = x.Id;
                    float _x = x.X;
                    float _y = x.Y;
                    string color = x.Color;
                    return new NodeXY(id,_x,_y){Color=Color.Parse(color),Weight=x.Weight};
                }
            ).ToList();
            var edges = (loaded.Edges as IEnumerable<dynamic>).Select(
                x=>{ 
                    int parentId = x.ParentId;
                    int childId = x.ChildId;
                    float weight = x.Weight;
                    string color = x.Color;
                    NodeXY parent = nodes.First(n=>n.Id==parentId);
                    NodeXY child = nodes.First(n=>n.Id==childId);
                    return new NodeConnector(parent,child){Weight=weight,Color=Color.Parse(color)};
                }
            );
            //TODO: check if graph is valid
            var res = new GraphStructure<NodeXY,NodeConnector>(new SampleGraphConfiguration());
            res.UseNodes(nodes);
            foreach(var e in edges){
                res.Nodes[e.Parent.Id].Edges.Add(e);
            }
            result = res;
        });
        return result ?? throw new ApplicationException("Could not load graph");
        #pragma warning enable
    }

}