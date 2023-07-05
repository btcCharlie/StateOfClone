using UnityEngine;

namespace StateOfClone.Units
{
    public struct SteeringParams
    {
        public Vector2 Turning { get; set; }
        public float Thrust { get; set; }

        public SteeringParams(Vector2 turning, float thrust)
        {
            Turning = turning;
            Thrust = thrust;
        }

        public static SteeringParams Zero => new(Vector2.zero, 0f);
    }
}
