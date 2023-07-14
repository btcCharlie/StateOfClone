using UnityEngine;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(Unit), typeof(Rigidbody))]
    public abstract class SteeringBehavior : MonoBehaviour
    {
        protected Rigidbody _rb;
        protected Unit _unit;
        protected Locomotion _locomotion;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _unit = GetComponent<Unit>();
            _locomotion = GetComponent<Locomotion>();
        }

        public abstract SteeringParams GetSteering(Vector3 target);

        protected abstract float CalculateYaw(Vector3 steeringForce, Vector3 currentSpeed);

        protected abstract float CalculatePitch(Vector3 steeringForce, Vector3 currentSpeed);

        protected abstract float CalculateSpeed(Vector3 steeringForce, float trueMaxSpeed);
    }
}
