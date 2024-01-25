using QuikGraph.Algorithms;
using System.Drawing;
using GraphSharp;
using GraphSharp.Adapters;

//Here is a good example how ToQuikGraph adapter works as a real example

ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
var quikGraph = graph.Converter.ToQuikGraph();
var pageRank = new QuikGraph.Algorithms.Ranking.PageRankAlgorithm<int, EdgeAdapter<Edge>>(quikGraph)
{
    Damping = 0.85,
    Tolerance = 0.001
};
pageRank.Compute();

var ranks = pageRank.Ranks;

//or just use built-in pagerank which is 10 times faster
// var ranks = graph.Do.PageRank().Ranks;


Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawDirectionsParallel(graph.Edges, argz.thickness, argz.directionLength, Color.Orange);
    
    foreach(var n in graph.Nodes){
        var coeff = ranks[n.Id];
        drawer.DrawNode(n,(double)(argz.nodeSize*coeff));
    }
},x=>x.MapProperties().Position);