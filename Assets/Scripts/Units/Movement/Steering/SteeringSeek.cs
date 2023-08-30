using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior, ISteeringBehavior
    {
        private float _yaw, _pitch, _speed;

        public SteeringSeek(UnitData ud, Locomotion locomotion) : base(ud, locomotion)
        {
            SteeringType = SteeringType.Seek;
        }

        public override SteeringParams GetSteering(Vector3 position, TargetInfo target)
        {
            Vector3 desiredVelocity =
                (target.Position - position).normalized * _ud.MaxSpeed;

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
            Vector3 desiredDirection = new(
                desiredVelocity.x,
                0f,
                desiredVelocity.z
                );
            Vector3 currentDirection = new(
                _locomotion.transform.forward.x,
                0f,
                _locomotion.transform.forward.z
                );
            currentDirection = currentDirection.normalized;
            desiredDirection = desiredDirection.normalized;

            return Vector3.SignedAngle(
                currentDirection, desiredDirection, Vector3.up
                );
        }

        protected override float CalculatePitch(Vector3 desiredVelocity)
        {
            // For now, we're not considering pitch, so return 0
            return 0f;
        }

        protected override float CalculateSpeed(
            Vector3 desiredVelocity, float trueMaxSpeed
            )
        {
            return _ud.MaxSpeed;
        }
    }
}
