using GraphSharp.GraphStructures;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using System.Drawing;
using GraphSharp.Nodes;
using GraphSharp.Edges;
using System.Collections.Concurrent;

public class Algorithm<TNode,TEdge> : Visitor<TNode, TEdge>
where TNode : INode
where TEdge : IEdge<TNode>
{
    public override IPropagator<TNode,TEdge> Propagator { get; }
    public GraphStructure<TNode, TEdge> Graph { get; }

    /// <summary>
    /// Count of used colors
    /// </summary>
    public IDictionary<Color,int> UsedColors {get;}
    IList<Color> Colors { get; }
    int edgesCount = 0;
    public Algorithm(GraphStructureBase<TNode,TEdge> graph,IEnumerable<Color> colors)
    {
        Graph = new(graph);
        UsedColors = new ConcurrentDictionary<Color,int>();
        foreach(var c in colors)
            UsedColors[c] = 0;
        Colors = colors.ToList();
        Propagator = new Propagator<TNode, TEdge>(this,graph);
        edgesCount = 0;
        EndVisit();
        foreach(var n in graph.Nodes){
            n.Color = Color.Empty;
        }
    }
    public override void EndVisit()
    {
        edgesCount = Graph.Nodes
            .AsParallel()
            .Where(x=>Graph.Edges[x.Id].Count()>edgesCount)
            .DefaultIfEmpty()
            .Min(x=>{
                if(x is null) return -1;
                return Graph.Edges[x.Id].Count();
            });

        if(Done()) return;
        SetPosition(ChoosePositions());
    }
    public override bool Select(TEdge edge) => true;
    public override void Visit(TNode node)
    {
        if(Done()) return;
        var nodeColor = node.Color;
        if(nodeColor==Color.Empty){
            nodeColor = ChooseColor(node);
            node.Color=nodeColor;
        }
    }
    public bool Done()=>edgesCount==-1;
    Color ChooseColor(TNode node){
        var colors = Graph.Edges[node.Id].Select(x=>x.Target.Color).Distinct();
        var color = Colors.Except(colors).FirstOrDefault();
        if(color == default){
            color = Color.FromArgb((byte)Random.Shared.Next(256),(byte)Random.Shared.Next(256),(byte)Random.Shared.Next(256));
            UsedColors[color] = 0;
            Colors.Add(color);
        }

        UsedColors[color] += 1;
        return color;
    }
    int[] ChoosePositions(){
        return Graph.Nodes
            .AsParallel()
            .Where(x=>Graph.Edges[x.Id].Count()==edgesCount)
            .Select(x=>x.Id)
            .ToArray();
    }
}