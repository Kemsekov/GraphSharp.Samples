using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GraphSharp.Graphs;
namespace SampleBase
{
    public class SampleGraphConfiguration : IGraphConfiguration<NodeXY, NodeConnector>
    {

        public Random CreateNodesRand{get;set;} = new Random();
        public Random CreateEdgesRand{get;set;} = new Random();

        public Random Rand{get;set;}

        public SampleGraphConfiguration(Random rand)
        {
            Rand = rand;
        }
        public NodeConnector CreateEdge(NodeXY parent, NodeXY child)
        {
            return new NodeConnector(parent,child);
        }


        public NodeXY CreateNode(int nodeId)
        {
            return new NodeXY(nodeId,new(CreateNodesRand.NextSingle(),CreateNodesRand.NextSingle()));
        }

        public IEdgeSource<NodeXY, NodeConnector> CreateEdgeSource()
        {
            return new DefaultEdgeSource<NodeXY,NodeConnector>();
        }

        public INodeSource<NodeXY> CreateNodeSource()
        {
            return new DefaultNodeSource<NodeXY>(0);
        }
    }
}