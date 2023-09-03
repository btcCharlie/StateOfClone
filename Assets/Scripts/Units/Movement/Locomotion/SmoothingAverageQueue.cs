using UnityEngine;
using System.Collections.Generic;

namespace StateOfClone.Units
{
    public class SmoothingAverageQueue
    {
        private Queue<float> _speeds;
        private Queue<float> _angulars;
        private Queue<Vector3> _normals;

        private int _speedsCount, _normalsCount, _angularsCount;

        public SmoothingAverageQueue(
            int speedsCount, int normalsCount, int angularsCount
            )
        {
            _speeds = new Queue<float>(speedsCount);
            _normals = new Queue<Vector3>(normalsCount);
            _angulars = new Queue<float>(angularsCount);
            _speedsCount = speedsCount;
            _normalsCount = normalsCount;
            _angularsCount = angularsCount;
        }

        public void Initiliaze(
            Vector3 initNormal, float initSpeed = 0f, float initAngular = 0f
            )
        {
            _speeds.Clear();
            _normals.Clear();
            _angulars.Clear();

            for (int i = 0; i < _speedsCount; i++)
            {
                _speeds.Enqueue(initSpeed);
            }
            for (int i = 0; i < _normalsCount; i++)
            {
                _normals.Enqueue(initNormal);
            }
            for (int i = 0; i < _angularsCount; i++)
            {
                _angulars.Enqueue(initAngular);
            }
        }

        /// <summary>
        /// Moves the moving average of normals one forward and replaces the 
        /// last element with newNormal.
        /// </summary>
        /// <param name="newNormal">The new normal to add</param>
        public void AddNewNormal(Vector3 newNormal)
        {
            if (_normals.Count >= _normalsCount)
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
            if (_speeds.Count >= _speedsCount)
            {
                _speeds.Dequeue();
            }
            _speeds.Enqueue(newSpeed);
        }

        /// <summary>
        /// Moves the moving average of angular speeds one forward and replaces the 
        /// last element with newSpeed.
        /// Never add speed calculated by the moving average! It would fall 
        /// into a cycle.
        /// </summary>
        /// <param name="newAngular">The new speed to add</param>
        public void AddNewAngular(float newAngular)
        {
            if (_angulars.Count >= _angularsCount)
            {
                _angulars.Dequeue();
            }
            _angulars.Enqueue(newAngular);
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

        private float SumAngulars()
        {
            float sum = 0f;
            foreach (float v in _angulars)
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

        public float AverageAngular()
        {
            return SumAngulars() / _angulars.Count;
        }
    }
}
