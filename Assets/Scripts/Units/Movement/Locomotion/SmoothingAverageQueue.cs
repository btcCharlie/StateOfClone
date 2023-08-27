using UnityEngine;
using System.Collections.Generic;

namespace StateOfClone.Units
{
    public class SmoothingAverageQueue
    {
        private Queue<float> _speeds;
        private Queue<Vector3> _normals;

        private int _speedsCount, _normalsCount;

        public SmoothingAverageQueue(int speedsCount, int normalsCount)
        {
            _speeds = new Queue<float>(speedsCount);
            _normals = new Queue<Vector3>(normalsCount);
            _speedsCount = speedsCount;
            _normalsCount = normalsCount;
        }

        public void Initiliaze(Vector3 initNormal, float initSpeed = 0f)
        {
            _speeds.Clear();
            _normals.Clear();

            for (int i = 0; i < _speedsCount; i++)
            {
                _speeds.Enqueue(initSpeed);
            }
            for (int i = 0; i < _normalsCount; i++)
            {
                _normals.Enqueue(initNormal);
            }
        }

        /// <summary>
        /// Moves the moving average of normals one forward and replaces the 
        /// last element with newNormal.
        /// </summary>
        /// <param name="newNormal">The new normal to add</param>
        public void AddNewNormal(Vector3 newNormal)
        {
            if (_normals.Count != 0)
            {
                _normals.Dequeue();
            }
            _normals.Enqueue(newNormal);
        }

        /// <summary>
        /// Moves the moving average of speeds one forward and replaces the 
        /// last element with newSpeed.
        /// Never add speed calculated by the moving average! It would fall 
        /// into a cycle.
        /// </summary>
        /// <param name="newSpeed">The new speed to add</param>
        public void AddNewSpeed(float newSpeed)
        {
            if (_speeds.Count != 0)
            {
                _speeds.Dequeue();
            }
            _speeds.Enqueue(newSpeed);
        }

        private float SumSpeeds()
        {
            float sum = 0f;
            foreach (float f in _speeds)
            {
                sum += f;
            }
            return sum;
        }

        private Vector3 SumNormals()
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 v in _normals)
            {
                sum += v;
            }
            return sum;
        }

        public float AverageSpeed()
        {
            return SumSpeeds() / _speeds.Count;
        }

        public Vector3 AverageNormal()
        {
            return SumNormals() / _normals.Count;
        }
    }
}
