using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior
    {
        private float _yaw, _pitch, _speed;

        public override SteeringParams GetSteering(Vector3 target)
        {
            // Calculate the desired velocity
            Vector3 desiredVelocity =
                (target - _rb.position).normalized * _unit.UnitData.MaxSpeed;

            Vector3 currentVelocity = transform.forward * _locomotion.CurrentSpeed;

            // Calculate the steering force
            Vector3 steeringForce = desiredVelocity - currentVelocity;

            _yaw = CalculateYaw(steeringForce, currentVelocity);
            _pitch = CalculatePitch(steeringForce, currentVelocity);
            _speed = CalculateSpeed(steeringForce, currentVelocity);

            // Create and return the SteeringParams
            return new SteeringParams(_yaw, _pitch, _speed);
        }

        protected override float CalculateYaw(Vector3 steeringForce, Vector3 currentSpeed)
        {
            // Calculate the angle difference between the current and 
            // desired velocity along the world horizontal plane
            Vector3 currentDirection = new(currentSpeed.x, 0, currentSpeed.z);
            Vector3 desiredDirection = new(steeringForce.x, 0, steeringForce.z);
            float angleDifference = Vector3.SignedAngle(
                currentDirection, desiredDirection, Vector3.up
                );

            if (Mathf.Abs(angleDifference) >= _rotationAlignmentThreshold)
            {
                return angleDifference / 90f;
            }
            else
            {
                return 0f;
            }
        }

        protected override float CalculatePitch(Vector3 steeringForce, Vector3 currentSpeed)
        {
            // For now, we're not considering pitch, so return 0
            return 0f;
        }

        protected override float CalculateSpeed(Vector3 steeringForce, Vector3 currentSpeed)
        {
            // For the Seek behavior, the speed should always be the maximum speed
            return _unit.UnitData.MaxSpeed;
        }

        private void OnDrawGizmos()
        {
            if (_rb == null)
                return;

            Color ogColor = Gizmos.color;

            Vector3 offset = Vector3.up * 3f;
            Vector3 transformPosition = transform.position + offset;
            Gizmos.color = Color.grey;
            Vector3 thrustPoint = transformPosition + transform.forward * _speed;
            Gizmos.DrawLine(transformPosition, thrustPoint);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(from, from + _locomotion.Velocity * 10f);
            // Gizmos.color = Color.green;
            // Gizmos.DrawLine(transformPosition, transformPosition + _locomotion.Velocity);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                thrustPoint,
                thrustPoint + _unit.UnitData.MaxSpeed * _yaw * transform.right
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
