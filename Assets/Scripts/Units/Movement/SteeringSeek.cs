using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior
    {
        [SerializeField] private float yawMaxTurnRateDegrees = 30f;
        private float _yaw, _pitch, _speed;

        public override SteeringParams GetSteering(Vector3 target)
        {
            // Calculate the desired velocity
            Vector3 desiredVelocity =
                (target - _rb.position).normalized * _unit.UnitData.MaxSpeed;

            Vector3 currentVelocity = transform.forward * _locomotion.CurrentSpeed;

            // Calculate the steering force
            Vector3 steeringForce = desiredVelocity - currentVelocity;

            _yaw = CalculateYaw(desiredVelocity, currentVelocity);
            _pitch = CalculatePitch(desiredVelocity, currentVelocity);
            _speed = CalculateSpeed(desiredVelocity, currentVelocity);

            // Create and return the SteeringParams
            return new SteeringParams(_yaw, _pitch, _speed);
        }

        protected override float CalculateYaw(Vector3 desiredVelocity, Vector3 currentVelocity)
        {
            // Calculate the angle difference between the current and 
            // desired velocity along the world horizontal plane
            Vector3 currentDirection = currentVelocity.normalized;
            Vector3 desiredDirection = desiredVelocity.normalized;
            currentDirection = new(currentDirection.x, 0, currentDirection.z);
            desiredDirection = new(desiredDirection.x, 0, desiredDirection.z);
            float angleDifferenceDegrees = Vector3.SignedAngle(
                currentDirection, desiredDirection, Vector3.up
                );

            // Debug.Log($"Current: {currentDirection}, Desired: {desiredDirection}");
            // Debug.Log($"Yaw difference: {angleDifferenceDegrees}°");

            return angleDifferenceDegrees / yawMaxTurnRateDegrees;
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
