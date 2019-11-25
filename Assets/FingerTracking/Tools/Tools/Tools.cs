using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools
{
    public static class Colors
    {
        public static Color RedYellowRed(float t)
        {
            if (t > 1) return Color.blue;
            if (t < 0) return Color.magenta;

            if (t < 0.5f)
            {
                t = t / 0.5f;
                return t * Color.yellow + (1.0f - t) * Color.red;
            }
            else
            {
                t = (t - 0.5f) / 0.5f;
                return t * Color.green + (1.0f - t) * Color.yellow;
            }

        }
    }

    public static class Debugging
    {
        public static void DrawDashedLine(Vector3 p0, Vector3 p1, Color color, float dashLength)
        {
            Vector3 p = p0;
            Vector3 step = (p1 - p0).normalized * dashLength;
            while(Vector3.Distance(p,p0) < Vector3.Distance(p0, p1))
            {
                Debug.DrawLine(p, p+step, color);
                p += 2*step;
            }
        }
    }

}

