using System.Drawing;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulation();
var tree = graph.Do.MakeSpanningTree();

foreach(var edge in tree){
    edge.Color = Color.Azure;
}

Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz,graph.Configuration,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    // drawer.DrawDirections(graph.Edges,argz.thickness,argz.directionLength,Color.CadetBlue);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
    drawer.DrawEdgesParallel(tree,argz.thickness);
    // drawer.DrawNodeIds(graph.Nodes,Color.Azure,argz.fontSize);
});
