using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphSharp.GraphStructures;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using SixLabors.ImageSharp;
using GraphSharp.Extensions;

public class Algorithm : Visitor<NodeXY, NodeConnector>
{
    public override IPropagator<NodeXY> Propagator { get; }
    /// <summary>
    /// List of used colors
    /// </summary>
    public HashSet<Color> UsedColors {get;}
    IList<Color> Colors { get; }
    int edgesCount = 0;
    IList<NodeXY> Nodes;
    Color _emptyColor;
    public Algorithm(GraphStructureBase<NodeXY,NodeConnector> nodes,IEnumerable<Color> colors)
    {
        UsedColors = new();
        Colors = colors.ToList();
        _emptyColor = colors.First();
        Propagator = new Propagator<NodeXY, NodeConnector>(this);
        Propagator.SetNodes(nodes);
        Nodes = nodes.Nodes;
        EndVisit();
        foreach(var n in Nodes)
            n.Color = _emptyColor;
    }
    public override void EndVisit()
    {
        edgesCount = Nodes
            .AsParallel()
            .Where(x=>x.Edges.Count>edgesCount)
            .DefaultIfEmpty()
            .Min(x=>x?.Edges.Count ?? -1);
        
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
        var colors = node.Edges.Select(x=>x.Child.Color).Distinct();
        var color = Colors.Except(colors).FirstOrDefault();
        if(color == default){
            color = Color.FromRgb((byte)Random.Shared.Next(),(byte)Random.Shared.Next(),(byte)Random.Shared.Next());
            Colors.Add(color);
        }
        UsedColors.Add(color);
        return color;
    }
    int[] ChoosePositions(){
        return Nodes
            .AsParallel()
            .Where(x=>x.Edges.Count==edgesCount)
            .Select(x=>x.Id)
            .ToArray();
    }
}