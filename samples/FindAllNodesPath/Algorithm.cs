
using System.Collections.Concurrent;
using GraphSharp.Edges;
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

    public override IPropagator<NodeXY> Propagator{get;}

    public Algorithm(int nodesCount)
    {
        Propagator = new ParallelPropagator<NodeXY,NodeConnector>(this);
        _visited = new byte[nodesCount];
        Path = new List<NodeXY>(nodesCount);
    }
    public override void EndVisit()
    {

    }

    public override bool Select(NodeConnector edge)
    {
        var n = edge.Child;
        return Path.Count==0 || n.Id==Path.Last().Id;
    }
    public override void Visit(NodeXY node)
    {
        if(PathDone) return;

        _visited[node.Id] = 1;
        foreach(var n in node.Edges){
            if(_visited[n.Child.Id]==0){
                _trace[n.Child] = node;
                Path.Add(n.Child);
                return;
            }
        }
        if(_trace.TryGetValue(node,out var parent))
            Path.Add(parent);
        else
            PathDone = true;

    }
}