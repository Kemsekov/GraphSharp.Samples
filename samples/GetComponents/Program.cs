using System.Drawing;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.CheckForIntegrity();
Helpers.MeasureTime(() =>
{
    System.Console.WriteLine("Finding components...");
    var components = graph.Do.GetComponents();
    System.Console.WriteLine($"Found {components.Count()} components");
    foreach (var c in components)
    {
        var color = Color.FromArgb(Random.Shared.Next(256), Random.Shared.Next(256), Random.Shared.Next(256));
        foreach (var n in c.Nodes)
        {
            n.Color = color;
        }
    }
});
Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
    // drawer.DrawNodeIds(graph.Nodes,Color.Azure,argz.fontSize);
});
