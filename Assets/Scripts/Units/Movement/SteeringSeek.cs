using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior
    {
        private Vector3 _desiredVelocity, _seekSteering;

        public override Vector3 GetSteering(Vector3 target)
        {
            _desiredVelocity = Vector3.ClampMagnitude(
                target - _rb.position, _unit.UnitData.MaxSpeed
                );
            _seekSteering = _desiredVelocity - _rb.velocity;
            // _seekSteering *= 10f;


            return _seekSteering;
        }

        // private void OnDrawGizmos()
        // {
        //     if (_rb == null)
        //         return;

        //     Color ogColor = Gizmos.color;

        //     Vector3 offset = Vector3.up * 3f;
        //     Vector3 from = transform.position + offset;
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawLine(from, from + _desiredVelocity);
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawLine(from, from + _seekSteering);
        //     Gizmos.color = Color.green;
        //     Gizmos.DrawLine(from, from + _rb.velocity);

        //     Gizmos.color = ogColor;
        // }
    }
}
