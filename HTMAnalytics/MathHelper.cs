using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTM.HTMAnalytics
{

    public class MathHelper
    {
        public static Random r = new Random();
        public static double Normal(double mu, double sig)
        {

            double u, v, x, y, q;
            do
            {
                u = r.NextDouble();
                v = 1.7156 * (r.NextDouble() - 0.5);
                x = u - 0.449871;
                y = Math.Abs(v) + 0.386595;
                q = Math.Sqrt(x) + y * (0.19600 * y - 0.25472 * x);
            } while (q > 0.27597 && (q > 0.27846 || Math.Sqrt(v) > -4.0 * Math.Log(u) * Math.Sqrt(u)));
            return mu + sig * v / u;
        }
    }
}
