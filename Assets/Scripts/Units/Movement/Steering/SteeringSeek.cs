using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringSeek : SteeringBehavior, ISteeringBehavior
    {
        private float _yaw, _pitch, _speed;

        public SteeringSeek(UnitData ud, Locomotion locomotion) : base(ud, locomotion)
        {
        }

        public override SteeringParams GetSteering(Vector3 position, Vector3 target)
        {
            // Calculate the desired velocity
            Vector3 desiredVelocity =
                (target - position).normalized * _ud.MaxSpeed;

            _yaw = CalculateYaw(desiredVelocity);
            _pitch = CalculatePitch(desiredVelocity);

            float expectedYawTurnRate =
                _locomotion.GetTurnRateFromDeviation(_yaw);
            float trueMaxSpeed =
                _locomotion.GetMaxSpeedAtTurnRate(expectedYawTurnRate);

            _speed = CalculateSpeed(desiredVelocity, trueMaxSpeed);

            // Create and return the SteeringParams
            return new SteeringParams(_yaw, _pitch, _speed);
        }

        protected override float CalculateYaw(Vector3 desiredVelocity)
        {
            // Calculate the angle difference between the current and 
            // desired velocity along the world horizontal plane
            Vector3 currentDirection = _locomotion.CurrentVelocity.normalized;
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
            // For the Seek behavior, the speed should always be the maximum speed
            return _ud.MaxSpeed;
        }

        // private void OnDrawGizmos()
        // {
        //     if (_locomotion == null)
        //         return;

        //     Color ogColor = Gizmos.color;

        //     Vector3 offset = Vector3.up * 3f;
        //     Vector3 transformPosition = transform.position + offset;
        //     Gizmos.color = Color.grey;
        //     Vector3 thrustPoint = transformPosition + transform.forward * _speed;
        //     Gizmos.DrawLine(transformPosition, thrustPoint);
        //     Gizmos.color = Color.blue;
        //     Gizmos.DrawLine(
        //         thrustPoint,
        //         thrustPoint + _ud.MaxSpeed * _yaw * transform.right
        //     );
        //     Gizmos.color = Color.black;
        //     Gizmos.DrawLine(
        //         thrustPoint,
        //         thrustPoint + transform.up * _pitch
        //     );

        //     Gizmos.color = ogColor;
        // }
    }
}