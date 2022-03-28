using GraphSharp;
using MathNet.Numerics.LinearAlgebra.Single;
using GraphSharp.GraphStructures;
ArgumentsHandler argz = new("settings.json");

// var adj = MyHelpers.ReadAdjacencyMatrix();

var adj1 = DenseMatrix.OfArray(
    new float[,]{
        {0,1,0,1},
        {2,0,0,1},
        {0,1,0,2},
        {1,0,0,0}
    });

var adj2 = DenseMatrix.OfArray(
    new float[,]{
        {0,1,0,1},
        {2,0,0,1},
        {0,1,0,2},
        {1,0,0,0}
    });

var graph1 = MyMethods
    .CreateGraph()
    .FromAdjacencyMatrix(adj1);
var graph2 = MyMethods
    .CreateGraph()
    .FromAdjacencyMatrix(adj2);

Helpers.CreateImage(graph1,null,argz);
