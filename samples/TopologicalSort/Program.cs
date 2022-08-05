using System.Drawing;
using GraphSharp.Visitors;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.MakeUndirected();
graph.Do.MakeSources(65,77,39,40);

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Doing topological sort...");
    graph.Do.TopologicalSort();
});

foreach(var component in graph.Do.FindComponents().components){
    var color = Color.FromArgb(Random.Shared.Next(256),Random.Shared.Next(256),Random.Shared.Next(256));
    foreach(var node in component){
        node.Color = color;
    }
}

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawDirections(graph.Edges,argz.thickness,argz.directionLength,Color.CadetBlue);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
    drawer.DrawNodeIds(graph.Nodes,Color.Azure,argz.fontSize);
});
System.Console.WriteLine("Done");