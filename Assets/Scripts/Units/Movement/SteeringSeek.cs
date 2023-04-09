using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior
    {
        public override Vector3 GetSteering(Vector3 target)
        {
            Vector3 desiredVelocity = (target - _rb.position).normalized * _unit.UnitData.MaxSpeed;
            return (desiredVelocity - _rb.velocity) * 10f;
        }
    }
}
