using UnityEngine;

namespace StateOfClone.Units
{
    public struct SteeringParams
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Thrust { get; set; }

        public SteeringParams(float yaw, float thrust)
        {
            Yaw = yaw;
            Pitch = 0f;
            Thrust = thrust;
        }

        public SteeringParams(float yaw, float pitch, float thrust)
        {
            Yaw = yaw;
            Pitch = pitch;
            Thrust = thrust;
        }

        public static SteeringParams Zero => new(0f, 0f, 0f);

        /// <summary>
        /// Adds together the Vector2 and float components of the two objects.
        /// </summary>
        public static SteeringParams operator +(SteeringParams a, SteeringParams b)
        {
            return new SteeringParams(
                a.Yaw + b.Yaw, a.Pitch + b.Pitch, a.Thrust + b.Thrust
                );
        }

        /// <summary>
        /// Divides the Vector2 and float components by the float value.
        /// </summary>
        public static SteeringParams operator /(SteeringParams a, float value)
        {
            return new SteeringParams(
                a.Yaw / value, a.Pitch / value, a.Thrust / value
                );
        }
    }
}
