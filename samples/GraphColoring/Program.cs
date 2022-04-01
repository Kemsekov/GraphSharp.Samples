using SixLabors.ImageSharp;

ArgumentsHandler argz = new("settings.json");

var nodes = Helpers.CreateNodes(argz);

var coloring = new Algorithm(nodes,new[]{Color.Azure,Color.Yellow,Color.Red,Color.Coral,Color.Blue,Color.Pink,Color.Violet});

while(!coloring.Done())
    coloring.Propagate();

Helpers.EnsureRightColoring(nodes.Nodes);
System.Console.WriteLine($"Total colors used : {coloring.UsedColors.Count}");
foreach(var c in coloring.UsedColors)
    System.Console.WriteLine(c);

Helpers.CreateImage(argz,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdges(nodes.Nodes);
    drawer.DrawNodes(nodes.Nodes);
});
