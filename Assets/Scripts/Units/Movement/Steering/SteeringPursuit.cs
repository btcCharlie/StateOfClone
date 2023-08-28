using StateOfClone.Core;
using UnityEngine;

namespace StateOfClone.Units
{
    public class SteeringPursuit : SteeringBehavior, ISteeringBehavior
    {
        public class SteeringPredictor
        {
            private float _turningParam;

            public SteeringPredictor(float turningParam)
            {
                _turningParam = turningParam;
            }

            public TargetInfo PredictPosition(Vector3 position, TargetInfo target)
            {
                float distance = Vector3.Distance(
                    position, target.Transform.position
                    );
                float timeToInterception = EstimateTimeToInterception(distance);

                Vector3 targetHeading = new(
                    target.Transform.forward.x, 0f, target.Transform.forward.z
                );
                targetHeading = targetHeading.normalized;

                return new TargetInfo(
                    target.Transform.position + targetHeading * timeToInterception
                    );
            }

            private float EstimateTimeToInterception(float distance)
            {
                return distance * _turningParam;
            }
        }

        private float _yaw, _pitch, _speed;
        private SteeringSeek _steeringSeek;
        private SteeringPredictor _predictor;

        public SteeringPursuit(UnitData ud, Locomotion locomotion) : base(ud, locomotion)
        {
            SteeringType = SteeringType.Pursuit;
            _steeringSeek = new(ud, locomotion);
            _predictor = new(0.3f);
        }

        public override SteeringParams GetSteering(Vector3 position, TargetInfo target)
        {
            target = target.Type switch
            {
                TargetType.Selectable => _predictor.PredictPosition(position, target),
                _ => target
            };

            SteeringParams sp = _steeringSeek.GetSteering(position, target);
            sp.Target = target.Position;
            return sp;
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
