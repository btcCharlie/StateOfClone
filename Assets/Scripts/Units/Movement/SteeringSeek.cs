using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior
    {
        private Vector3 _desiredVelocity, _seekSteering;
        private float _yaw, _pitch, _thrust;

        private Locomotion _locomotion;

        protected override void Awake()
        {
            base.Awake();
            _locomotion = GetComponent<Locomotion>();
        }

        public override SteeringParams GetSteering(Vector3 target)
        {
            _desiredVelocity = Vector3.ClampMagnitude(
                target - _rb.position, _unit.UnitData.MaxSpeed
                );
            _seekSteering = _desiredVelocity - _locomotion.Velocity;

            //TODO: correct calculation of yaw and roll - perhaps limit to just 
            //TODO: yaw for now, beware of when unit moves directly away

            _yaw = _desiredVelocity.x - _locomotion.Velocity.x;
            _pitch = _desiredVelocity.z - _locomotion.Velocity.z;
            // _turning = new(
            //     _locomotion.Velocity.x - _desiredVelocity.x,
            //     _locomotion.Velocity.z - _desiredVelocity.z
            // );
            _thrust = _unit.UnitData.MaxSpeed;

            return new SteeringParams(_yaw, _pitch, _thrust);
        }

        private void OnDrawGizmos()
        {
            if (_rb == null)
                return;

            Color ogColor = Gizmos.color;

            Vector3 offset = Vector3.up * 3f;
            Vector3 transformPosition = transform.position + offset;
            Gizmos.color = Color.grey;
            Vector3 thrustPoint = transformPosition + transform.forward * _thrust;
            Gizmos.DrawLine(transformPosition, thrustPoint);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(from, from + _locomotion.Velocity * 10f);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(transformPosition, transformPosition + _locomotion.Velocity);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                thrustPoint,
                thrustPoint + -transform.right * _yaw
            );
            Gizmos.color = Color.black;
            Gizmos.DrawLine(
                thrustPoint,
                thrustPoint + transform.up * _pitch
            );

            Gizmos.color = ogColor;
        }
    }
}
