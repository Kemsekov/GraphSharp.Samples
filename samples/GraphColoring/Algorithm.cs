using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphSharp.GraphStructures;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using GraphSharp.Extensions;
using System.Drawing;

public class Algorithm : Visitor<NodeXY, NodeConnector>
{
    public override IPropagator<NodeXY,NodeConnector> Propagator { get; }
    public IGraphStructure<NodeXY, NodeConnector> Graph { get; }

    /// <summary>
    /// List of used colors
    /// </summary>
    public HashSet<Color> UsedColors {get;}
    IList<Color> Colors { get; }
    int edgesCount = 0;
    Color _emptyColor;
    public Algorithm(IGraphStructure<NodeXY,NodeConnector> graph,IEnumerable<Color> colors)
    {
        Graph = graph;
        UsedColors = new();
        Colors = colors.ToList();
        _emptyColor = colors.First();
        Propagator = new Propagator<NodeXY, NodeConnector>(this,graph);
        EndVisit();
        foreach(var n in graph.Nodes)
            n.Color = _emptyColor;
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
    public override bool Select(NodeConnector edge) => true;
    public override void Visit(NodeXY node)
    {
        if(Done()) return;
        if(node.Color==_emptyColor){
            node.Color = ChooseColor(node);
        }
    }
    public bool Done()=>edgesCount==-1;
    Color ChooseColor(NodeXY node){
        var colors = Graph.Edges[node.Id].Select(x=>x.Target.Color).Distinct();
        var color = Colors.Except(colors).FirstOrDefault();
        if(color == default){
            color = Color.FromArgb((byte)Random.Shared.Next(256),(byte)Random.Shared.Next(256),(byte)Random.Shared.Next(256));
            Colors.Add(color);
        }
        UsedColors.Add(color);
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