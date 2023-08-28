using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringArrival : SteeringBehavior
    {
        private float _yaw, _pitch, _speed;
        public float slowingDistance = 10f; // The distance at which the agent starts to slow down

        public SteeringArrival(UnitData ud, Locomotion locomotion) : base(ud, locomotion)
        {
            SteeringType = SteeringType.Arrival;
        }

        public override SteeringParams GetSteering(Vector3 position, TargetInfo target)
        {
            // Calculate the desired velocity
            Vector3 direction = target.Position - position;
            float distance = direction.magnitude;
            float speed =
                (distance < slowingDistance) ?
                _ud.MaxSpeed * (distance / slowingDistance) :
                _ud.MaxSpeed;
            Vector3 desiredVelocity = direction.normalized * speed;

            _yaw = CalculateYaw(desiredVelocity);
            _pitch = CalculatePitch(desiredVelocity);

            float expectedYawTurnRate =
                _locomotion.Motion.SpeedCalculator.CalculateYawTurnRate(_yaw);
            float trueMaxSpeed =
                _locomotion.Motion.SpeedCalculator.GetMaxSpeedAtTurnRate(expectedYawTurnRate);

            _speed = CalculateSpeed(desiredVelocity, trueMaxSpeed);

            // Create and return the SteeringParams
            return new SteeringParams(_yaw, _pitch, _speed);
        }

        protected override float CalculateYaw(Vector3 desiredVelocity)
        {
            // Calculate the angle difference between the current and 
            // desired velocity along the world horizontal plane
            Vector3 currentDirection = _locomotion.transform.forward;
            Vector3 desiredDirection = desiredVelocity.normalized;
            currentDirection = new(currentDirection.x, 0, currentDirection.z);
            desiredDirection = new(desiredDirection.x, 0, desiredDirection.z);
            float angleDifferenceDegrees = Vector3.SignedAngle(
                currentDirection, desiredDirection, Vector3.up
                );

            return angleDifferenceDegrees;
        }

        protected override float CalculatePitch(Vector3 desiredVelocity)
        {
            // For now, we're not considering pitch, so return 0
            return 0f;
        }

        protected override float CalculateSpeed(Vector3 desiredVelocity, float trueMaxSpeed)
        {
            // For the Arrival behavior, the speed should be the magnitude of the desired velocity
            return Mathf.Clamp(
                desiredVelocity.magnitude,
                -trueMaxSpeed,
                trueMaxSpeed
                );
        }
    }
}
