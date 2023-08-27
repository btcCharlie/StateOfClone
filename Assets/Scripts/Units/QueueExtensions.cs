using UnityEngine;
using System.Collections.Generic;

namespace StateOfClone.Units
{
    public static class QueueExtensions
    {
        public static float Sum(this Queue<float> queue)
        {
            float sum = 0f;
            foreach (float f in queue)
            {
                sum += f;
            }
            return sum;
        }

        public static Vector3 Sum(this Queue<Vector3> queue)
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 v in queue)
            {
                sum += v;
            }
            return sum;
        }
    }
}
