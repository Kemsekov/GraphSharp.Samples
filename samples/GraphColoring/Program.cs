
using System.Drawing;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation();
graph.Do.MakeUndirected();
// Helpers.SaveGraph(nodes,"graph.json");

// var graph = Helpers.LoadGraph("graph.json");

var coloring = new Algorithm<NodeXY,NodeConnector>(graph,new[]{Color.Azure,Color.Yellow,Color.Red,Color.Coral,Color.Blue,Color.Aqua,Color.Violet});

Helpers.MeasureTime(()=>{
    System.Console.WriteLine("Starting coloring graph...");
    for(int i = 0;i<argz.steps;i++){
        if(coloring.Done()){
            System.Console.WriteLine($"Coloring done at {i} step");
            return;
        }
        coloring.Propagate();
    }
    System.Console.WriteLine("Done all steps");
});

graph.EnsureRightColoring();

System.Console.WriteLine($"Total colors used : {coloring.UsedColors.Where(x=>x.Value!=0).Count()}");
foreach(var colorInfo in coloring.UsedColors.OrderByDescending(x=>x.Value)){
    System.Console.WriteLine(colorInfo);
}

System.Console.WriteLine("Nodes colored : {0}", coloring.UsedColors.Sum(x=>x.Value));

Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges,argz.thickness);
    drawer.DrawNodes(graph.Nodes,argz.nodeSize);
});
