using QuikGraph.Algorithms;
using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Adapters;
using GraphSharp.Extensions;

//Here is a good example how ToQuikGraph adapter works in a real example
ArgumentsHandler argz = new("settings.json");

Graph<Node,Edge> graph = Helpers.CreateGraph(argz);
var biggest = graph.Do.Induce(graph.Do.FindStronglyConnectedComponentsTarjan().Components.MaxBy(x=>x.nodes.Count()).nodes.Select(x=>x.Id));

biggest.Do.MakeDirected();
graph = biggest;

var sources = biggest.Nodes.Where(n=>biggest.Edges.IsSource(n.Id)).ToList();
var sinks = biggest.Nodes.Where(n=>biggest.Edges.IsSink(n.Id)).ToList();

var superSource = graph.Configuration.CreateNode(graph.Nodes.MaxNodeId+1);
var superSink = graph.Configuration.CreateNode(graph.Nodes.MaxNodeId+2);

graph.Nodes.Add(superSink);
graph.Nodes.Add(superSource);

foreach(var source in sources){
    var e = graph.Configuration.CreateEdge(superSource,source);
    graph.Edges.Add(e);
}

foreach(var sink in sinks){
    var e = graph.Configuration.CreateEdge(sink,superSink);
    graph.Edges.Add(e);
}

foreach(var e in graph.Edges){
    e.Properties["cap"]=Random.Shared.NextDouble()*5;
}

System.Console.WriteLine(graph.Do.FindStronglyConnectedComponentsTarjan().Components.Count());
var flow = graph.Do.MaxFlowEdmondsKarp(superSource.Id,superSink.Id);
var f = new MaxFlowConvergence<Edge>(graph.Edges,superSource.Id,superSink.Id,e=>(double)e.Properties["cap"]);

System.Console.WriteLine(f.Run());
System.Console.WriteLine(f.Run());
System.Console.WriteLine(f.Run());

foreach(var n in graph.Nodes){
    if(n.Id==superSink.Id || n.Id==superSource.Id) continue;
    var outE = graph.Edges.OutEdges(n.Id);   
    var inE = graph.Edges.InEdges(n.Id);
    var f1 = outE.Sum(e=>f.EdgeFlow[e]);
    var f2 = inE.Sum(e=>f.EdgeFlow[e]);
    if(Math.Abs(1-f1/f2)>0.001){
        System.Console.WriteLine("FUCK");
    }
}

var sourceFlow = f.Flow(superSource.Id);
var sinkFlow = f.Flow(superSink.Id);

if(Math.Abs(1-sourceFlow/sinkFlow)>0.001)
    System.Console.WriteLine("fuck");


// graph.Do.ConnectRandomly(1, 2);

var arrange = new GraphArrange<Node, Edge>(biggest);
for (int i = 0; i < 100; i++)
{
    arrange.ComputeStep();
}
var positions = arrange;



Helpers.CreateImage(argz, biggest, drawer =>
{
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness, Color.DarkBlue);
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
}, x => positions[x.Id]);