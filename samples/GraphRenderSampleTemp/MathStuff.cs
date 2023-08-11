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
    public static IEnumerable<int> Factorize(int n){
        if(n==0){
            yield break;
        }
        // Print the number of 2s that divide n
        while (n % 2 == 0)
        {
            yield return 2;
            n /= 2;
        }
 
        // n must be odd at this point. So we can
        // skip one element (Note i = i +2)
        for (int i = 3; i <= Math.Sqrt(n); i+= 2)
        {
            // While i divides n, print i and divide n
            while (n % i == 0)
            {
                yield return i;
                n /= i;
            }
        }
 
        // This condition is to handle the case when
        // n is a prime number greater than 2
        if (n > 2)
            yield return n;
    }
}