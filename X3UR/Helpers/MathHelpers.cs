using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace X3UR.Helpers;

public class MathHelpers {
    /// <summary>
    /// Returns a random long type value of two long types
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static long RandomLong(long min, long max) {
        byte[] buf = new byte[8];
        Random random = new Random();
        random.NextBytes(buf);
        long randomLong = BitConverter.ToInt64(buf, 0);

        return Math.Abs(randomLong % (max - min)) + min;
    }

    /// <summary>
    /// Calculates the distance between two positions (pos1 and pos2).
    /// </summary>
    /// <param name="pos1X"></param>
    /// <param name="pos1Y"></param>
    /// <param name="pos2X"></param>
    /// <param name="pos2Y"></param>
    /// <returns></returns>
    public static float DistanceOfTwoPoints2D(byte pos1X, byte pos1Y, byte pos2X, byte pos2Y) {
        return (float)Math.Round(Math.Sqrt((pos1X - pos2X) * (pos1X - pos2X) + (pos1Y - pos2Y) * (pos1Y - pos2Y)), 4);
    }
}