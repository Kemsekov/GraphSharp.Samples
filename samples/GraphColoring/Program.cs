using SixLabors.ImageSharp;

ArgumentsHandler argz = new("settings.json");

// var nodes = Helpers.CreateGraph(argz);
// Helpers.SaveGraph(nodes,"graph.json");

var nodes = Helpers.LoadGraph("graph.json");

var coloring = new Algorithm(nodes,new[]{Color.Azure,Color.Yellow,Color.Red,Color.Coral,Color.Blue,Color.Aqua,Color.Violet});

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

Helpers.EnsureRightColoring(nodes.Nodes);

System.Console.WriteLine($"Total colors used : {coloring.UsedColors.Count}");

Helpers.CreateImage(argz,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(nodes.Nodes);
    drawer.DrawNodes(nodes.Nodes);
});
