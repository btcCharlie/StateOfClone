using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringArrival : SteeringBehavior
    {
        private float _yaw, _pitch, _speed;
        public float slowingDistance = 10f; // The distance at which the agent starts to slow down

        public override SteeringParams GetSteering(Vector3 target)
        {
            // Calculate the desired velocity
            Vector3 direction = target - _rb.position;
            float distance = direction.magnitude;
            float speed = (distance < slowingDistance) ? _unit.UnitData.MaxSpeed * (distance / slowingDistance) : _unit.UnitData.MaxSpeed;
            Vector3 desiredVelocity = direction.normalized * speed;

            Vector3 currentVelocity = transform.forward * _locomotion.CurrentSpeedUnitPerSec;

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
            Vector3 currentDirection = transform.forward;
            Vector3 desiredDirection = desiredVelocity.normalized;
            currentDirection = new(currentDirection.x, 0, currentDirection.z);
            desiredDirection = new(desiredDirection.x, 0, desiredDirection.z);
            float angleDifferenceDegrees = Vector3.SignedAngle(
                currentDirection, desiredDirection, Vector3.up
                );

            return angleDifferenceDegrees;
        }

        protected override float CalculatePitch(Vector3 desiredVelocity, Vector3 currentSpeed)
        {
            // For now, we're not considering pitch, so return 0
            return 0f;
        }

        protected override float CalculateSpeed(Vector3 desiredVelocity, Vector3 currentSpeed)
        {
            // For the Arrival behavior, the speed should be the magnitude of the desired velocity
            return desiredVelocity.magnitude;
        }

        // Rest of the code...
    }
}
