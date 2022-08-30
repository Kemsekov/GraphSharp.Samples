using QuikGraph.Algorithms;
using System.Drawing;
using GraphSharp;
using GraphSharp.Adapters;

//Here is a good example how ToQuikGraph adapter works in a real example
ArgumentsHandler argz = new("settings.json");
var graph = Helpers.CreateGraph(argz);
var quikGraph = new ToQuikGraphAdapter<Node,Edge>(graph);
var pageRank = new QuikGraph.Algorithms.Ranking.PageRankAlgorithm<Node,EdgeAdapter<Node,Edge>>(quikGraph);
pageRank.Damping = 0.85;
pageRank.Tolerance = 0.001;
pageRank.Compute();
var ranks = pageRank.Ranks;
Helpers.ShiftNodesToFitInTheImage(graph.Nodes);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawDirectionsParallel(graph.Edges, argz.thickness, argz.directionLength, Color.Orange);
    
    foreach(var n in graph.Nodes){
        var coeff = ranks[n];
        drawer.DrawNode(n,(float)(argz.nodeSize*coeff));
    }
});