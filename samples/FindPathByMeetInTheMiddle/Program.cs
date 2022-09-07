using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
//this program showing how to find the shortest path between two nodes
//by summing and comparing sum of visited path


ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulationWithoutHull();

var startNode = argz.node1 % graph.Nodes.Count;
var endNode = argz.node2 % graph.Nodes.Count;
IList<Node> path1 = new List<Node>();
IList<Node> path2 = new List<Node>();

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by meeting in the middle parallel (wheat)...");
    path1= graph.Do.FindPathByMeetInTheMiddleParallel(startNode,endNode);
    System.Console.WriteLine(path1.Count);
    graph.ValidatePath(path1);
});

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Finding path by parallel any path finder (orange)...");
    path2 = graph.Do.FindAnyPathParallel(startNode,endNode);
    System.Console.WriteLine(path2.Count);
    graph.ValidatePath(path2);
});

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNode(graph.Nodes[startNode],argz.nodeSize);
    drawer.DrawNode(graph.Nodes[endNode],argz.nodeSize);
    if (path1?.Count > 0)
    {
        drawer.DrawPath(path1,Color.Wheat,argz.thickness);
    }
    if (path2?.Count > 0)
    {
        drawer.DrawPath(path2,Color.Orange,argz.thickness);
    }
});

