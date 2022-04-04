using System;
using System.Collections.Generic;
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
            return new NodeXY(nodeId,CreateNodesRand.NextDouble(),CreateNodesRand.NextDouble());
        }

        public float Distance(NodeXY n1, NodeXY n2)
        {
            return (float)n1.Distance(n2);
        }

        public float GetEdgeWeight(NodeConnector edge)
        {
            return edge.Weight;
        }

        public float GetNodeWeight(NodeXY node)
        {
            return node.Weight;
        }

        public void SetEdgeWeight(NodeConnector edge, float weight)
        {
            edge.Weight = weight;
        }

        public void SetNodeWeight(NodeXY node, float weight)
        {
            node.Weight = weight;
        }
    }
}