using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphSharp.GraphStructures;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
public class Algorithm : Visitor<NodeXY, NodeConnector>
{
    public override PropagatorBase<NodeXY, NodeConnector> Propagator { get; }
    public Algorithm(GraphStructureBase<NodeXY, NodeConnector> graph)
    {
        Propagator = new ParallelPropagator<NodeXY, NodeConnector>(this, graph);
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