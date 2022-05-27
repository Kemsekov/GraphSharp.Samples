using System.Collections.Concurrent;
using GraphSharp.Edges;
using GraphSharp.GraphStructures;
using GraphSharp.Nodes;
using GraphSharp.Propagators;
using GraphSharp.Visitors;
public class Algorithm : Visitor<NodeXY,NodeConnector>
{
    /// <summary>
    /// _path[node] = parent 
    /// </summary>
    IDictionary<NodeXY, NodeXY> _path = new ConcurrentDictionary<NodeXY, NodeXY>();
    /// <summary>
    /// what is the length of path from startNode to some other node so far.  
    /// </summary>
    IDictionary<NodeXY, double> _pathLength = new ConcurrentDictionary<NodeXY, double>();
    NodeXY _startNode;

    public override IPropagator<NodeXY,NodeConnector> Propagator{get;}

    /// <param name="startNode">Node from which we need to find a shortest path</param>
    public Algorithm(NodeXY startNode, IGraphStructure<NodeXY,NodeConnector> graph)
    {
        this._startNode = startNode;
        _pathLength[startNode] = 0;
        Propagator = new ParallelPropagator<NodeXY,NodeConnector>(this,graph);
    }
    public override void EndVisit()
    {
    }

    public override bool Select(NodeConnector Edge)
    {
        bool updatePath = true;
        if (Edge is NodeConnector connection)
        {
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
        }
        return true;
    }

    public override void Visit(NodeXY node)
    {
        //do nothing. We do not actually need to do anything here.
    }

    /// <param name="end"></param>
    /// <returns>Null if path not found</returns>
    public List<NodeXY>? GetPath(NodeXY end)
    {
        if (!_path.ContainsKey(end)) return null;
        var path = new List<NodeXY>();
        while (true)
            if (_path.TryGetValue(end, out NodeXY? parent))
            {
                path.Add(end);
                end = parent;
            }
            else break;
        path.Add(_startNode);
        path.Reverse();
        return path;
    }
    public double GetPathLength(NodeXY node){
        if(_pathLength.TryGetValue(node,out double length)){
            return length;
        }
        return 0;
    }
}