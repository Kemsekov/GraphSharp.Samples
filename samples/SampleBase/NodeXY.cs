using System;
using GraphSharp.Edges;
using GraphSharp.Nodes;

/// <summary>
/// Edge class for the NodeXY class.
/// </summary>
public class NodeXY : NodeBase<NodeConnector>
{
    public NodeXY(int id, double x, double y) : base(id)
    {
        X = x;
        Y = y;
    }
    public double X{get;set;}
    public double Y{get;set;}
    public float Weight{get;set;}
    public SixLabors.ImageSharp.Color Color{get;set;} = SixLabors.ImageSharp.Color.Brown;
    public double Distance(NodeXY other){
        return Math.Sqrt((X-other.X)*(X-other.X)+(Y-other.Y)*(Y-other.Y));
    }
    public override string ToString()
    {
        return $"{Id}\t({(float)X}\t{(float)Y})";
    }
}