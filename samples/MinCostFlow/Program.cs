using QuikGraph.Algorithms;
using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Adapters;
using GraphSharp.Extensions;
using MathNet.Numerics.LinearAlgebra.Single;

ArgumentsHandler argz = new("settings.json");
Graph graph = Helpers.CreateGraph(argz);
graph.Clear();

//example taken from
//https://developers.google.com/optimization/flow/mincostflow
int[] startNodes = { 0, 0, 1, 1, 1, 2, 2, 3, 4 };
int[] endNodes = { 1, 2, 2, 3, 4, 3, 4, 4, 2 };
int[] capacities = { 15, 8, 20, 4, 10, 15, 4, 20, 5 };
int[] unitCosts = { 4, 4, 2, 2, 6, 1, 3, 2, 3 };

// Define an array of supplies at each node.
int[] supplies = { 20, 0, 0, -5, -15 };

var edges = startNodes.Zip(endNodes).ToArray();
graph.Converter.FromConnectionsList(edges);

foreach(var ((s,t),cap,cost) in edges.Zip(capacities,unitCosts)){
    //how much resource can be transported by edge from one node to another
    graph.Edges[s,t].MapProperties().Capacity=cap;
    //what it costs to travel one unit of resource by edge
    graph.Edges[s,t].MapProperties().Cost=cost;
}

foreach(var n in graph.Nodes){
    n.MapProperties().Supply=supplies[n.Id];
}

var pos = graph.Do.Arrange(10,getWeight:e=>1);

var result = graph.Do.MinCostMaxFlowGoogleOrTools();

foreach(var e in graph.Edges){
    System.Console.WriteLine("--------------");
    System.Console.WriteLine(e);
    System.Console.WriteLine("Flow "+result.Flow[e]);
    System.Console.WriteLine("Total cost "+result.Cost(e));
}

Helpers.CreateImage(argz, graph, drawer =>
{
    foreach(var e in graph.Edges){
        var flow = result.Flow[e];
        if(flow==0) continue;
        var thick = flow*1.0/2000;
        drawer.DrawEdge(e, thick, Color.DarkViolet);
        drawer.DrawDirection(e,thick,0.1,Color.DarkGray);
    }
    drawer.DrawNodes(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes,Color.Azure,argz.fontSize);
}, x => (Vector)(pos[x.Id]*0.9f+0.05f));