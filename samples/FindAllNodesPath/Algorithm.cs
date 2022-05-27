
using System.Collections.Concurrent;
using GraphSharp.Edges;
using GraphSharp.GraphStructures;
using GraphSharp.Nodes;
using GraphSharp.Propagators;
using GraphSharp.Visitors;

public class Algorithm : Visitor<NodeXY,NodeConnector>
{
    byte[] _visited;
    public IList<NodeXY> Path;
    public bool PathDone = false;
    /// <summary>
    /// _trace[node] = parent
    /// </summary>
    IDictionary<NodeXY,NodeXY> _trace = new ConcurrentDictionary<NodeXY,NodeXY>();

    public IGraphStructure<NodeXY, NodeConnector> Graph { get; }
    public override IPropagator<NodeXY,NodeConnector> Propagator{get;}

    public Algorithm(IGraphStructure<NodeXY,NodeConnector> graph,int nodesCount)
    {
        Graph = graph;
        Propagator = new ParallelPropagator<NodeXY,NodeConnector>(this,graph);
        _visited = new byte[nodesCount];
        Path = new List<NodeXY>(nodesCount);
    }
    public override void EndVisit()
    {

    }

    public override bool Select(NodeConnector edge)
    {
        var n = edge.Target;
        return Path.Count==0 || n.Id==Path.Last().Id;
    }
    public override void Visit(NodeXY node)
    {
        if(PathDone) return;

        _visited[node.Id] = 1;
        foreach(var n in Graph.Edges[node.Id]){
            if(_visited[n.Target.Id]==0){
                _trace[n.Target] = node;
                Path.Add(n.Target);
                return;
            }
        }
        if(_trace.TryGetValue(node,out var parent))
            Path.Add(parent);
        else
            PathDone = true;

    }
}