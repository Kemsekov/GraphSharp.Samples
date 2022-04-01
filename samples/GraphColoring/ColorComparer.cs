using System.Numerics;
using SixLabors.ImageSharp;

class ColorComparer : IComparer<Color>
{
    public int Compare(Color x, Color y)
    {
        return x==y ? 0 : 1;
    }
}