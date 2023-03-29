using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringParams
    {
        public float AngularVelocity { get; set; }
        public Vector3 LinearVelocity { get; set; }

        public SteeringParams()
        {
            AngularVelocity = 0f;
            LinearVelocity = Vector3.zero;
        }
    }
}
