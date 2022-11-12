using System.Drawing;

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
graph.Do.DelaunayTriangulationWithoutHull(x=>x.Position);
Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n,p)=>n.Position = p);
Helpers.CreateImage(argz,graph,drawer=>{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges,argz.thickness);
    drawer.DrawDirections(graph.Edges,argz.thickness,argz.directionLength,Color.CadetBlue);
    drawer.DrawNodesParallel(graph.Nodes,argz.nodeSize);
    // drawer.DrawNodeIds(graph.Nodes,Color.Azure,argz.fontSize);
},x=>x.Position);
