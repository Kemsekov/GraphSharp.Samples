using System.Drawing;
using GraphSharp;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.MakeBidirected();
var sources = new int[]{65,77,39,40};
graph.Do.MakeSources(sources);

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Doing topological sort...");
    var sorted = graph.Do.TopologicalSort().GetSortedByCoordinate();
    foreach(var (component,coordinate) in sorted){
        foreach(var node in component){
            graph.Nodes[node].MapProperties().Position[0]= (float)coordinate;
        }
    }
});

foreach(var component in graph.Do.FindComponents().Components){
    var color = Color.FromArgb(Random.Shared.Next(256),Random.Shared.Next(256),Random.Shared.Next(256));
    foreach(var node in component){
        node.MapProperties().Color = color;
    }
}

Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawDirections(graph.Edges,argz.thickness,argz.directionLength,Color.CadetBlue);
    drawer.DrawNodesParallel(sources.Select(x=>graph.Nodes[x]),argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes,Color.Azure,argz.fontSize);
},x=> (MathNet.Numerics.LinearAlgebra.Single.Vector)(x.MapProperties().Position*0.9f+0.05f));
System.Console.WriteLine("Done");