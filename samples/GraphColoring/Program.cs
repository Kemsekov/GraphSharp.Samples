
using System.Drawing;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
// Helpers.SaveGraph(nodes,"graph.json");

// var graph = Helpers.LoadGraph("graph.json");

var coloring = new Algorithm(graph,new[]{Color.Azure,Color.Yellow,Color.Red,Color.Coral,Color.Blue,Color.Aqua,Color.Violet});

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

System.Console.WriteLine($"Total colors used : {coloring.UsedColors.Count}");

Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(graph.Edges,argz.thickness);
    drawer.DrawNodes(graph.Nodes,argz.nodeSize);
});
