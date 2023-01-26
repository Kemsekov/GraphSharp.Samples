using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using QuikGraph.Algorithms;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
if (graph.Edges.Count == 0)
    graph.Do.DelaunayTriangulation(x => x.Position);

var lineGraph = graph.Do.LineGraph();

var newPositions = graph.Do.PlanarRender(5);

foreach(var n in graph.Nodes)
    n.Position = newPositions[n.Id];

argz.filename="graph.jpg";
Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n1,n2)=>n1.Position=n2);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.Azure, argz.fontSize);
}, x =>x.Position);

argz.filename="linegraph.jpg";
Helpers.CreateImage(argz, lineGraph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(lineGraph.Edges, argz.thickness,Color.Orange);
    drawer.DrawNodesParallel(lineGraph.Nodes, argz.nodeSize,Color.OrangeRed);
    drawer.DrawNodeIds(lineGraph.Nodes, Color.Azure, argz.fontSize);
}, x =>{
    var p1 = graph.Nodes[x.Edge.SourceId].Position;
    var p2 = graph.Nodes[x.Edge.TargetId].Position;
    return (p1+p2)/2;
});