using UnityEngine;

namespace StateOfClone.Units
{
    public class Agent : MonoBehaviour
    {
        [field: SerializeField] public float MaxSpeed { get; set; }
        [field: SerializeField] public float MaxAcceleration { get; set; }

        public float TrueMaxSpeed { get; set; }

        public float Orientation { get; set; }
        public float Rotation { get; set; }
        public Vector3 Velocity { get; set; }
        protected SteeringParams steeringParams;

        private void Start()
        {
            Velocity = Vector3.zero;
            steeringParams = new SteeringParams();
            TrueMaxSpeed = MaxSpeed;
        }

        public void SetSteering(SteeringParams steeringParams, float weight)
        {
            // this.steeringParams.LinearVelocity += steeringParams.LinearVelocity * weight;
            // this.steeringParams.AngularVelocity += steeringParams.AngularVelocity * weight;
        }

        protected virtual void Update()
        {
            Vector3 displacement = Velocity * Time.deltaTime;
            Orientation += Rotation * Time.deltaTime;

            // limit orientation to 0-360
            if (Orientation > 360f)
            {
                Orientation -= 360f;
            }
            else if (Orientation < 0f)
            {
                Orientation += 360f;
            }

            transform.Translate(displacement, Space.World);
            transform.rotation = new Quaternion();
            transform.Rotate(Vector3.up, Orientation);
        }

        private void LateUpdate()
        {
            // Velocity += steeringParams.LinearVelocity * Time.deltaTime;
            // Rotation += steeringParams.AngularVelocity * Time.deltaTime;

            // limit velocity to max speed
            if (Velocity.magnitude > TrueMaxSpeed)
            {
                Velocity.Normalize();
                Velocity *= TrueMaxSpeed;
            }

            steeringParams = new SteeringParams();
        }

        public void SpeedReset()
        {
            MaxSpeed = TrueMaxSpeed;
        }
    }
}
