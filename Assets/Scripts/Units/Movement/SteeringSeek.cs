using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior
    {
        private Vector3 _desiredVelocity, _seekSteering;

        private Locomotion _locomotion;

        protected override void Awake()
        {
            base.Awake();
            _locomotion = GetComponent<Locomotion>();
        }

        public override Vector3 GetSteering(Vector3 target)
        {
            _desiredVelocity = Vector3.ClampMagnitude(
                target - _rb.position, _unit.UnitData.MaxSpeed
                );
            _seekSteering = _desiredVelocity - _locomotion.Velocity;

            return _seekSteering;
        }

        private void OnDrawGizmos()
        {
            if (_rb == null)
                return;

            Color ogColor = Gizmos.color;

            Vector3 offset = Vector3.up * 3f;
            Vector3 transformPosition = transform.position + offset;
            Gizmos.color = Color.grey;
            Gizmos.DrawLine(transformPosition, transformPosition + _desiredVelocity);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(from, from + _locomotion.Velocity * 10f);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(transformPosition, transformPosition + _locomotion.Velocity);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                transformPosition + _locomotion.Velocity,
                transformPosition + _locomotion.Velocity + _seekSteering
            );

            Gizmos.color = ogColor;
        }
    }
}
