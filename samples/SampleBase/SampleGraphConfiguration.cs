using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GraphSharp.GraphStructures.Interfaces;

namespace SampleBase
{
    public class SampleGraphConfiguration : IGraphConfiguration<NodeXY, NodeConnector>
    {
        public Random CreateNodesRand{get;set;} = new Random();
        public Random CreateEdgesRand{get;set;} = new Random();
        public Random Rand {get=>CreateEdgesRand;set{}}

        public NodeConnector CreateEdge(NodeXY parent, NodeXY child)
        {
            return new NodeConnector(parent,child);
        }

        public NodeXY CreateNode(int nodeId)
        {
            return new NodeXY(nodeId,CreateNodesRand.NextSingle(),CreateNodesRand.NextSingle());
        }

        public float Distance(NodeXY n1, NodeXY n2)
        {
            return (float)n1.Distance(n2);
        }

        public Color GetEdgeColor(NodeConnector edge)
        {
            return edge.Color.ToSystemDrawingColor();
        }

        public float GetEdgeWeight(NodeConnector edge)
        {
            return edge.Weight;
        }

        public Color GetNodeColor(NodeXY node)
        {
            return node.Color.ToSystemDrawingColor();
        }

        public float GetNodeWeight(NodeXY node)
        {
            return node.Weight;
        }

        public void SetEdgeColor(NodeConnector edge, Color color)
        {
            edge.Color = color.ToImageSharpColor();
        }

        public void SetEdgeWeight(NodeConnector edge, float weight)
        {
            edge.Weight = weight;
        }

        public void SetNodeColor(NodeXY node, Color color)
        {
            node.Color = color.ToImageSharpColor();
        }

        public void SetNodeWeight(NodeXY node, float weight)
        {
            node.Weight = weight;
        }
    }
}