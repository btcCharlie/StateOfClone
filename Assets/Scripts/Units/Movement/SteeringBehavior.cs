using UnityEngine;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(Unit), typeof(Rigidbody))]
    public class SteeringBehavior : MonoBehaviour
    {
        protected Rigidbody _rb;
        protected Unit _unit;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _unit = GetComponent<Unit>();
        }

        public virtual SteeringParams GetSteering(Vector3 target)
        {
            return new SteeringParams { };
        }
    }
}
