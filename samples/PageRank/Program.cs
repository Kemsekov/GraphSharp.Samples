using QuikGraph.Algorithms;
using System.Drawing;
using GraphSharp;
using GraphSharp.Adapters;

//Here is a good example how ToQuikGraph adapter works in a real example
ArgumentsHandler argz = new("settings.json");

var graph = Helpers.CreateGraph(argz);
var quikGraph = graph.Converter.ToQuikGraph();
var pageRank = new QuikGraph.Algorithms.Ranking.PageRankAlgorithm<int,EdgeAdapter<Edge>>(quikGraph);
pageRank.Damping = 0.85;
pageRank.Tolerance = 0.001;
pageRank.Compute();

var ranks = pageRank.Ranks;

//or just
// var ranks = graph.Do.PageRank().Ranks;


Helpers.ShiftNodesToFitInTheImage(graph.Nodes,x=>x.Position,(n,p)=>n.Position = p);
Helpers.CreateImage(argz, graph, drawer =>
{
    drawer.Clear(Color.Black);
    drawer.DrawEdgesParallel(graph.Edges, argz.thickness);
    drawer.DrawDirectionsParallel(graph.Edges, argz.thickness, argz.directionLength, Color.Orange);
    
    foreach(var n in graph.Nodes){
        var coeff = ranks[n.Id];
        drawer.DrawNode(n,(double)(argz.nodeSize*coeff));
    }
},x=>x.Position);