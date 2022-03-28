using GraphSharp.GraphStructures;
using MathNet.Numerics.LinearAlgebra.Single;

using GraphType = GraphSharp.GraphStructures.GraphStructureBase<NodeXY, NodeConnector>;
public static class MyMethods
{
    public static int ReadInt()
    {
        while (true)
            if (int.TryParse(Console.ReadLine(), out int result))
            {
                return result;
            }
            else
                System.Console.WriteLine("Not a number! Try again!");

    }
    public static Matrix ReadAdjacencyMatrix()
    {
        int size = 0;
        System.Console.WriteLine("Enter matrix size");

        size = ReadInt();
        var adj = DenseMatrix.Create(size, size, 0);
        System.Console.WriteLine("Enter adjacency matrix values as integers");
        for (int i = 0; i < size; i++)
        {
            var buffer = (Console.ReadLine() ?? "").Split(' ');
            for (int b = 0; b < size; b++)
            {
                if(int.TryParse(buffer[b],out var value)){
                    adj[i,b] = value;
                }
            }
        }
        return adj;
    }
    public static GraphStructure<NodeXY,NodeConnector> CreateGraph(){
        var rand = new Random();

        var createEdge = (NodeXY parent,NodeXY node) => new NodeConnector(parent,node);
        var createNode = (int id) => new NodeXY(id, rand.NextDouble(), rand.NextDouble());
        var graphStructure = new GraphStructure<NodeXY,NodeConnector>(createNode,createEdge);
        return graphStructure;
    }
    public static bool SameNumberOfEdges(GraphType g1,GraphType g2){
        return g1.TotalEdgesCount()==g2.TotalEdgesCount();
    }
    public static bool SameNumberOfVertices(GraphType g1,GraphType g2){
        return g1.Nodes.Count==g2.Nodes.Count;
    }
    public static bool SameDegreesOfVertices(GraphType g1,GraphType g2){
        var d1 = g1.CountDegrees();
        var d2 = g2.CountDegrees();
        for(int i = 0;i<g1.Nodes.Count;i++){
            if(d1[i]!=d2[i]) return false;
        }
        return true;
    }
    public static bool SameNumbersOfCycles(GraphType g1,GraphType g2){
        return true;
    }
    
}