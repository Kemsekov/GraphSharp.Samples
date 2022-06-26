using System.Drawing;
using System.Numerics;
using GraphSharp.Common;
using GraphSharp.Nodes;
using SampleBase;

/// <summary>
/// Edge class for the NodeXY class.
/// </summary>
public class NodeXY : INode
{
    public NodeXY(int id, Vector2 position)
    {
        Id = id;
        Position = position;
    }
    public int Id{get;}
    public Vector2 Position { get; set; }
    public float Weight{get;set;}
    public Color Color{get;set;} = Color.Brown;
    public float Distance(NodeXY other){
        return (other.Position-Position).Length();
    }
    public override string ToString()
    {
        return $"{Id}\t({Position.X}\t{Position.Y})";
    }
}