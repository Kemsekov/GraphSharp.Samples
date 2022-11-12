using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class MathStuff
{
    public static double smoothstep(double edge0, double edge1, double x)
    {
        // Scale, bias and saturate x to 0..1 range
        x = Math.Clamp((x - edge0) / (edge1 - edge0), 0.0f, 1.0f);
        // Evaluate polynomial
        return x * x * (3 - 2 * x);
    }
}