using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using GraphSharp.GraphStructures;
namespace SampleBase
{
    public class SampleGraphConfiguration : GraphConfiguration<NodeXY, NodeConnector>
    {

        public Random CreateNodesRand{get;set;} = new Random();
        public Random CreateEdgesRand{get;set;} = new Random();

        public SampleGraphConfiguration(Random rand) : base(rand)
        {
        }
        public override NodeConnector CreateEdge(NodeXY parent, NodeXY child)
        {
            return new NodeConnector(parent,child);
        }


        public override NodeXY CreateNode(int nodeId)
        {
            return new NodeXY(nodeId,new(CreateNodesRand.NextSingle(),CreateNodesRand.NextSingle()));
        }
    }
}