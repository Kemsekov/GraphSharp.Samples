using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphSharp.GraphStructures;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
public class Algorithm : Visitor<NodeXY, NodeConnector>
{
    public override IPropagator<NodeXY, NodeConnector> Propagator { get; }
    public IList<IList<NodeXY>> Layers { get; }
    public bool Done { get; private set; } = false;
    public const byte Added = 4;
    public Algorithm(GraphStructureBase<NodeXY, NodeConnector> graph)
    {
        Propagator = new ParallelPropagator<NodeXY, NodeConnector>(this, graph);
        var incomingEdges = graph.CountIncomingEdges();
        var startingNodes = new List<int>();
        foreach (var count in incomingEdges)
        {
            if (count.Value == 0)
            {
                startingNodes.Add(count.Key);
            }
        }
        Layers = new List<IList<NodeXY>>();
        Propagator.SetPosition(startingNodes.ToArray());
        this.EndVisit();
    }

    public override void EndVisit()
    {
        if (Done) return;
        if (Layers.Count > 0)
        {
            if (Layers[^1].Count == 0)
            {
                this.Done = true;
                return;
            }
        }
        Layers.Add(new List<NodeXY>());
    }

    public override bool Select(NodeConnector edge)
    {
        if (Done) return false;
        if (Propagator.IsNodeInState(edge.Target.Id, Added))
        {
            return false;
        }
        Propagator.SetNodeState(edge.Target.Id, Added);
        return true;
    }

    public override void Visit(NodeXY node)
    {
        if (Done) return;
        lock (Layers)
            Layers[^1].Add(node);
    }

    public void DoTopologicalSort()
    {
        if (!Done) return;

        var nodePositionShift = 1.0f / (Layers.Count() - 2);
        var nodePosition = 0.01f;

        foreach (var layer in Layers)
        {
            foreach (var node in layer)
            {
                node.Position = new(nodePosition, node.Position.Y);
            }
            nodePosition += nodePositionShift;
        }
    }
}