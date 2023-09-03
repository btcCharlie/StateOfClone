using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringArrival : SteeringBehavior, ISteeringBehavior
    {
        private float _yaw, _pitch, _speed;
        private float _slowDownRadius;

        public SteeringArrival(
            UnitData ud, Locomotion locomotion, float slowDownRadius
            ) : base(ud, locomotion)
        {
            SteeringType = SteeringType.Arrival;
            _slowDownRadius = slowDownRadius;
        }

        public override SteeringParams GetSteering(
            SelectionInfo self, SelectionInfo target
            )
        {
            Vector3 direction = target.Position - self.Position;
            float distance = direction.magnitude;
            float speed =
                (distance < _slowDownRadius) ?
                _ud.MaxSpeed * (distance / _slowDownRadius) :
                _ud.MaxSpeed;
            Vector3 desiredVelocity = direction.normalized * speed;

            _yaw = CalculateYaw(desiredVelocity);
            _pitch = CalculatePitch(desiredVelocity);

            float expectedYawTurnRate =
                _locomotion.Motion.SpeedCalculator.CalculateYawTurnRate(_yaw);
            float trueMaxSpeed =
                _locomotion.Motion.SpeedCalculator.GetMaxSpeedAtTurnRate(expectedYawTurnRate);

            _speed = CalculateSpeed(desiredVelocity, trueMaxSpeed);

            return new SteeringParams(_yaw, _pitch, _speed, target.Position);
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
            return 0f;
        }

        protected override float CalculateSpeed(
            Vector3 desiredVelocity, float trueMaxSpeed
            )
        {
            return Mathf.Clamp(
                desiredVelocity.magnitude,
                -trueMaxSpeed,
                trueMaxSpeed
                );
        }
    }
}
