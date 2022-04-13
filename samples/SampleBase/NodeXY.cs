using System;
using GraphSharp.Edges;
using GraphSharp.Nodes;

/// <summary>
/// Edge class for the NodeXY class.
/// </summary>
public class NodeXY : NodeBase<NodeConnector>
{
    public NodeXY(int id, float x, float y) : base(id)
    {
        X = x;
        Y = y;
    }
    public float X{get;set;}
    public float Y{get;set;}
    public float Weight{get;set;}
    public SixLabors.ImageSharp.Color Color{get;set;} = SixLabors.ImageSharp.Color.Brown;
    public float Distance(NodeXY other){
        return MathF.Sqrt((X-other.X)*(X-other.X)+(Y-other.Y)*(Y-other.Y));
    }
    public override string ToString()
    {
        return $"{Id}\t({(float)X}\t{(float)Y})";
    }
}