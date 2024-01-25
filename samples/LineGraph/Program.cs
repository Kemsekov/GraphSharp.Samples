using System.Drawing;
using GraphSharp;
using MathNet.Numerics.LinearAlgebra.Single;

//line graph is a graph where edges becomes nodes
//and two nodes have edge if their original edges are adjacent

ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation(x => x.MapProperties().Position);
var lineGraph = graph.Do.LineGraph();

var graphPos = graph.Do.PlanarRender(5);

argz.filename="graph.jpg";
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes, argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes, Color.Azure, argz.fontSize);
}, x => (Vector)(graphPos[x.Id]*0.9f+0.05f));

argz.filename="linegraph.jpg";
Helpers.CreateImage(argz, lineGraph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(lineGraph.Edges, argz.thickness,Color.Orange);
    drawer.DrawNodesParallel(lineGraph.Nodes, argz.nodeSize,Color.OrangeRed);
    drawer.DrawNodeIds(lineGraph.Nodes, Color.Azure, argz.fontSize);
}, x =>{
    var p1 = graphPos[x.Edge.SourceId];
    var p2 = graphPos[x.Edge.TargetId];
    return (MathNet.Numerics.LinearAlgebra.Single.Vector)((p1+p2)/2*0.9f+0.05f);
});