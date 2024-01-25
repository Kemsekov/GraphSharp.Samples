//Cliques condensation graph
//it created new graph by condensing biggest cliques into nodes
//so new graph's nodes is cliques in original graph
using System.Drawing;
using GraphSharp;
using MathNet.Numerics.LinearAlgebra.Single;


ArgumentsHandler argz = new("settings.json");

var g = Helpers.CreateGraph(argz);
g.Do.MakeDirected();

var cliquesCondensed = g.Do.CondenseCliques();

argz.filename = "graph.jpg";
Helpers.CreateImage(argz, g, drawer =>
{
    drawer.Clear(Color.Black);

    drawer.DrawEdgesParallel(g.Edges, argz.thickness, Color.DarkViolet);
    //draw each clique in different color
    foreach(var n in cliquesCondensed.Nodes){
        var color = Color.FromArgb(Random.Shared.Next(256),Random.Shared.Next(256),Random.Shared.Next(256));
        n.MapProperties().Color=color;
        foreach(var node in n.Component.Nodes){
            drawer.DrawNode(g.Nodes[node.Id],argz.nodeSize,color);
        }
    }
}, x =>g.Nodes[x.Id].MapProperties().Position);

argz.filename = "condensedCliques.jpg";
Helpers.CreateImage(argz, g, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(cliquesCondensed.Edges, argz.thickness, Color.DarkViolet);
    drawer.DrawNodesParallel(cliquesCondensed.Nodes,argz.nodeSize);
}, x =>{
    var components = cliquesCondensed.Nodes[x.Id].Component;
    var avgVec = components.Nodes.First().MapProperties().Position.Map(i=>0.0f);
    foreach(var n in components.Nodes){
        var pos = n.MapProperties().Position;
        avgVec+=pos;
    }
    return (Vector)(avgVec /components.Nodes.Count());
});
