using UnityEngine;

namespace StateOfClone.Units
{
    public struct SteeringParams
    {
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Speed { get; set; }

        public SteeringParams(float yaw, float speed)
        {
            Yaw = yaw;
            Pitch = 0f;
            Speed = speed;
        }

        public SteeringParams(float yaw, float pitch, float speed)
        {
            Yaw = yaw;
            Pitch = pitch;
            Speed = speed;
        }

        public static SteeringParams Zero => new(0f, 0f, 0f);

        /// <summary>
        /// Adds together the Vector2 and float components of the two objects.
        /// </summary>
        public static SteeringParams operator +(SteeringParams a, SteeringParams b)
        {
            return new SteeringParams(
                a.Yaw + b.Yaw, a.Pitch + b.Pitch, a.Speed + b.Speed
                );
        }

        /// <summary>
        /// Divides the Vector2 and float components by the float value.
        /// </summary>
        public static SteeringParams operator /(SteeringParams a, float value)
        {
            return new SteeringParams(
                a.Yaw / value, a.Pitch / value, a.Speed / value
                );
        }
    }
}
