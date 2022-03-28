using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphSharp.GraphStructures;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
using SixLabors.ImageSharp;

public class Algorithm : Visitor<NodeXY, NodeConnector>
{
    public override IPropagator<NodeXY> Propagator { get; }
    IList<Color> Colors { get; }
    IGraphStructure<NodeXY> Nodes;
    public Algorithm(IGraphStructure<NodeXY> nodes)
    {
        Colors = new List<Color>();
        Propagator = new ParallelPropagator<NodeXY, NodeConnector>(this);
        Propagator.SetNodes(nodes);
        Nodes = nodes;
    }
    public override void EndVisit()
    {

    }

    public override bool Select(NodeConnector edge)
    {
        return true;
    }

    public override void Visit(NodeXY node)
    {

    }
    
}