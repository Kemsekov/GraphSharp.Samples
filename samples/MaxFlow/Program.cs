using QuikGraph.Algorithms;
using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Adapters;

//Here is a good example how ToQuikGraph adapter works in a real example
ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);

//create some graph with source and sink
graph.Do.MakeBidirected();
graph.Do.TopologicalSort(65).ApplyTopologicalSort((node,pos)=>node.Position = pos,x=>x.Position.Y);
graph.Do.MakeSources(65);
//----

//find a max flow from source 65 to sink 39.
var maxFlow = graph.Do.MaxFlowEdmondsKarp(65,39);
System.Console.WriteLine("Max flow is "+maxFlow.MaxFlow);

Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n,p)=>n.Position = p);
Helpers.CreateImage(argz, graph, drawer =>
{
    var capacities = maxFlow.Capacities;
    drawer.Clear(Color.Black);
    foreach(var e in graph.Edges){
        //the more red edge is, the more capacity it has
        var thickness = Math.Min(capacities(e)/maxFlow.MaxFlow,1);
        drawer.DrawEdge(e,argz.thickness,Color.FromArgb((int)(thickness*255),0,0));
    }
    drawer.DrawNodeId(graph.Nodes[65],Color.Azure,argz.fontSize);
    drawer.DrawNodeId(graph.Nodes[39],Color.Azure,argz.fontSize);
},x=>x.Position);