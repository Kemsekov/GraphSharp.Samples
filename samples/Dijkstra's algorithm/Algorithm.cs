using System.Collections.Concurrent;
using GraphSharp.Edges;
using GraphSharp.GraphStructures;
using GraphSharp.Nodes;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
public class Algorithm<TNode, TEdge> : Visitor<TNode, TEdge>
where TNode : INode
where TEdge : IEdge<TNode>
{
    /// <summary>
    /// _path[node] = parent 
    /// </summary>
    IDictionary<TNode, TNode> _path = new ConcurrentDictionary<TNode, TNode>();
    /// <summary>
    /// what is the length of path from startNode to some other node so far.  
    /// </summary>
    IDictionary<TNode, double> _pathLength = new ConcurrentDictionary<TNode, double>();
    IGraphStructure<TNode, TEdge> _graph;
    TNode _startNode;

    public override PropagatorBase<TNode, TEdge> Propagator { get; }

    /// <param name="startNode">Node from which we need to find a shortest path</param>
    public Algorithm(TNode startNode, IGraphStructure<TNode, TEdge> graph)
    {
        this._graph = graph;
        this._startNode = startNode;
        _pathLength[startNode] = 0;
        Propagator = new ParallelPropagator<TNode, TEdge>(this, graph);
    }
    public override void EndVisit()
    {
    }

    public override bool Select(TEdge connection)
    {
        bool updatePath = true;
        var pathLength = _pathLength[connection.Source] + connection.Weight;

        if (_pathLength.TryGetValue(connection.Target, out double pathSoFar))
        {
            if (pathSoFar <= pathLength)
            {
                updatePath = false;
            }
        }
        if (updatePath)
        {
            _pathLength[connection.Target] = pathLength;
            _path[connection.Target] = connection.Source;
        }
        return true;
    }

    public override void Visit(TNode node)
    {
    }

    /// <param name="end"></param>
    /// <returns>Null if path not found</returns>
    public List<TNode>? GetPath(TNode end)
    {
        if (!_path.ContainsKey(end)) return null;
        var path = new List<TNode>();
        while (true)
            if (_path.TryGetValue(end, out TNode? parent))
            {
                path.Add(end);
                end = parent;
            }
            else break;
        path.Add(_startNode);
        path.Reverse();
        return path;
    }
    public double GetPathLength(TNode node)
    {
        if (_pathLength.TryGetValue(node, out double length))
        {
            return length;
        }
        return 0;
    }
}