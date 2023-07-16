using UnityEngine;

namespace StateOfClone.Units
{
    public interface ISteeringBehavior
    {
        SteeringParams GetSteering(Vector3 position, Vector3 target);
    }

    public abstract class SteeringBehavior : ISteeringBehavior
    {
        // protected Rigidbody _rb;
        // protected Unit _unit;
        protected UnitData _ud;
        protected Locomotion _locomotion;

        public SteeringBehavior(UnitData ud, Locomotion locomotion)
        {
            _ud = ud;
            _locomotion = locomotion;
        }

        public abstract SteeringParams GetSteering(Vector3 position, Vector3 target);

        protected abstract float CalculateYaw(Vector3 steeringForce);

        protected abstract float CalculatePitch(Vector3 steeringForce);

        protected abstract float CalculateSpeed(Vector3 steeringForce, float trueMaxSpeed);
    }
}
