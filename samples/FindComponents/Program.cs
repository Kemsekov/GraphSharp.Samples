using System.Drawing;
using GraphSharp.Graphs;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding components...");
    var componentsResult = graph.Do.FindComponents();
    System.Console.WriteLine($"Found {componentsResult.Components.Count()} components");
    foreach (var c in componentsResult.Components)
    {
        var color = Color.FromArgb(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
        foreach (var n in c)
        {
            n.Color = color;
        }
    }
});
Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n,p)=>n.Position = p);
Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
},x=>x.Position);
