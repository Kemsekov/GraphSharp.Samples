using GraphSharp.Edges;
using GraphSharp.Nodes;

public class NodeConnector : EdgeBase<NodeXY>, IComparable<NodeConnector>
{
    public NodeConnector(NodeXY parent,NodeXY node) : base(parent,node)
    {
        if(node is NodeXY n1 && parent is NodeXY n2)
            Weight = MathF.Sqrt((float)((n1.X-n2.X)*(n1.X-n2.X)+(n1.Y-n2.Y)*(n1.Y-n2.Y)));
    }
    public SixLabors.ImageSharp.Color DirectionColor = SixLabors.ImageSharp.Color.Blue;
    public float Weight{get;set;} = 1;
    public SixLabors.ImageSharp.Color Color{get;set;} = SixLabors.ImageSharp.Color.BlueViolet;
    public int CompareTo(NodeConnector? other)
    {
        if(other is null) return -1;
        return Node.Id-other.Node.Id;        
    }
}