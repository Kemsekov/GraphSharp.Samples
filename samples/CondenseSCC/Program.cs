//Strongly connected components condensation graph
//it created new graph by condensing SCC's into nodes
//so new graph's nodes is SCC in original graph
using System.Drawing;
using GraphSharp;
using MathNet.Numerics.LinearAlgebra.Single;


ArgumentsHandler argz = new("settings.json");

var g = Helpers.CreateGraph(argz);
g.Do.MakeDirected();

var sccCondensed = g.Do.CondenseSCC();

argz.filename = "graph.jpg";
Helpers.CreateImage(argz, g, drawer =>
{
    drawer.Clear(Color.Black);

    drawer.DrawEdgesParallel(g.Edges, argz.thickness, Color.DarkViolet);
    drawer.DrawDirectionsParallel(g.Edges,argz.thickness,argz.directionLength,Color.Gray);
    //draw each SCC in different color
    foreach(var n in sccCondensed.Nodes){
        var color = Color.FromArgb(Random.Shared.Next(256),Random.Shared.Next(256),Random.Shared.Next(256));
        n.MapProperties().Color=color;
        foreach(var node in n.Component.Nodes){
            drawer.DrawNode(g.Nodes[node.Id],argz.nodeSize,color);
        }
    }
}, x =>g.Nodes[x.Id].MapProperties().Position);

argz.filename = "condensedSCC.jpg";
Helpers.CreateImage(argz, sccCondensed, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(sccCondensed.Edges, argz.thickness, Color.DarkViolet);
    drawer.DrawDirectionsParallel(sccCondensed.Edges,argz.thickness,argz.directionLength,Color.Gray);
    drawer.DrawNodesParallel(sccCondensed.Nodes,argz.nodeSize);
}, x =>{
    var components = sccCondensed.Nodes[x.Id].Component;
    var avgVec = components.Nodes.First().MapProperties().Position.Map(i=>0.0f);
    foreach(var n in components.Nodes){
        var pos = n.MapProperties().Position;
        avgVec+=pos;
    }
    return (Vector)(avgVec /components.Nodes.Count());
});
