
using System.Drawing;
using GraphSharp.Graphs;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Coloring graph...");
    var usedColors = graph.Do.ColorNodes(
        new[]{Color.Azure,Color.Yellow,Color.Red,Color.Coral,Color.Blue,Color.Aqua,Color.Violet},
        x=>x.OrderBy(x=>graph.Edges[x.Id].Count())
    );
    System.Console.WriteLine($"Total colors used : {usedColors.Where(x=>x.Value!=0).Count()}");
    foreach(var colorInfo in usedColors.OrderByDescending(x=>x.Value)){
        System.Console.WriteLine(colorInfo);
    }
    System.Console.WriteLine("Nodes colored : {0}", usedColors.Sum(x=>x.Value));
});

graph.EnsureRightColoring();
Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges,argz.thickness);
    drawer.DrawNodes(graph.Nodes,argz.nodeSize);
});
