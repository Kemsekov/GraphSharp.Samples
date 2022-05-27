using System.Drawing;
using GraphSharp.Common;
using GraphSharp.Edges;
using GraphSharp.Nodes;
using SampleBase;

/// <summary>
/// Edge class for the NodeXY class.
/// </summary>
public class NodeConnector : EdgeBase<NodeXY>, IComparable<NodeConnector>, IEdgeData
{
    public NodeConnector(NodeXY parent,NodeXY node) : base(parent,node)
    {
        if(node is NodeXY n1 && parent is NodeXY n2)
            Weight = n1.Distance(n2);
    }
    public float Weight{get;set;} = 1;
    public Color Color{get;set;} = Color.BlueViolet;
    public int CompareTo(NodeConnector? other)
    {
        return other?.Target.Id.CompareTo(Target.Id) ?? throw new NullReferenceException();
    }
}