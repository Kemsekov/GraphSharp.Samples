
using System;
using System.Drawing;
using System.Linq;
using GraphSharp;
using GraphSharp.Common;
using GraphSharp.Graphs;
using GraphSharp.Visitors;
namespace Test;
public class NeuralEdge : IEdge
{
    public int SourceId{get;set;}
    public int TargetId{get;set;}
    public double Weight{get;set;}
    public double Activeness {get;set;}
    public bool IsChosen{get;set;}
    public Color Color { get => IsChosen ? Color.FromArgb(255,0,0) : Color.FromArgb(0,(int)(Activeness*255),255); set => throw new System.NotImplementedException(); }
    public NeuralEdge(int source, int target,double activeness){
        SourceId = source;
        TargetId=target;
        Activeness = activeness;
    }
    public IEdge Clone()
    {
        return new NeuralEdge(SourceId,TargetId,Activeness){Weight=Weight};
    }
}

public class NeuralVisitor<TNode> : IVisitor<TNode, NeuralEdge>
where TNode : INode
{
    public NeuralVisitor(IImmutableGraph<TNode,NeuralEdge> graph){
        Graph = graph;
    }
    public IImmutableGraph<TNode, NeuralEdge> Graph { get; }
    public double MaxWeight = 0.95;
    public double ActivenessFadingStrength=0.96;

    public void End()
    {
 
    }

    public bool Select(EdgeSelect<NeuralEdge> edge)
    {
        if(edge.Edge.IsChosen){
            // TODO: make some theory of how to increase edge weight depending on activeness
            edge.Edge.Weight+=edge.Edge.Activeness;
            edge.Edge.IsChosen = false;
            edge.Edge.Activeness=1;
            return true;
        }
        return false;
    }
    public void Start()
    {
       foreach(var e in Graph.Edges){
            e.Activeness*=ActivenessFadingStrength;
        }
    }

    public void Visit(TNode node)
    {
        var edges = Graph.Edges.OutEdges(node.Id);
        var sum = edges.Sum(x=>x.Weight);
        
        var weight = Random.Shared.NextDouble();
        foreach(var e in edges.OrderBy(x=>-x.Weight)){
            e.Weight/=sum;
            if(e.Weight>MaxWeight)
                e.Weight=MaxWeight;
            
            if(weight<=e.Weight){
                e.IsChosen=true;
                return;
            }
            weight-=e.Weight;
        }
        //----experemental
        // foreach(var e in Graph.Edges.OutEdges(node.Id)){
            // e.Activeness*=0.5;
        // }
    }
}