using System.Drawing;
using GraphSharp;
using GraphSharp.Graphs;
using QuikGraph.Algorithms;

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
if (graph.Edges.Count == 0)
    graph.Do.DelaunayTriangulation(x => x.Position);

var lineGraph = graph.Do.LineGraph();
var inverseLineGraph = lineGraph.Do.InverseLineGraph();
foreach(var e in inverseLineGraph.Edges){
    System.Console.WriteLine("Base "+e.BaseNode.Id);
    System.Console.WriteLine(e.SourceId);
    System.Console.WriteLine(e.TargetId);
    System.Console.WriteLine("-----------");
}
var graphPositions = graph.Do.PlanarRender(5);
var inverseGraphPositions = inverseLineGraph.Do.PlanarRender(5);
Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n1,n2)=>n1.Position=n2);

argz.filename="graph.jpg";
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.Azure, argz.fontSize);
}, x =>graphPositions[x.Id]);

argz.filename="linegraph.jpg";
Helpers.CreateImage(argz, lineGraph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(lineGraph.Edges, argz.thickness,Color.Orange);
    drawer.DrawNodesParallel(lineGraph.Nodes, argz.nodeSize,Color.OrangeRed);
    drawer.DrawNodeIds(lineGraph.Nodes, Color.Azure, argz.fontSize);
}, x =>{
    var p1 = graphPositions[x.Edge.SourceId];
    var p2 = graphPositions[x.Edge.TargetId];
    return (p1+p2)/2;
});

argz.filename="inverselinegraph.jpg";
Helpers.CreateImage(argz, inverseLineGraph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(inverseLineGraph.Edges, argz.thickness,Color.Orange);
    drawer.DrawNodesParallel(inverseLineGraph.Nodes, argz.nodeSize,Color.OrangeRed);
    drawer.DrawNodeIds(inverseLineGraph.Nodes, Color.Azure, argz.fontSize);
}, x =>{
    return inverseGraphPositions[x.Id];
});