using UnityEngine;

namespace StateOfClone.Units
{
    public class HeadingSteeringPredictor : ISteeringPredictor
    {
        private float _turningParam;
        private float _maxSpeed;

        public HeadingSteeringPredictor(float turningParam, float maxSpeed)
        {
            _turningParam = turningParam;
            if (maxSpeed == 0f)
            {
                _maxSpeed = 1f;
            }
            else
            {
                _maxSpeed = maxSpeed;
            }
        }

        public SelectionInfo PredictPosition(SelectionInfo self, SelectionInfo target)
        {
            Transform selfTransform = self.Moveable.transform;
            Transform targetTransform = target.Moveable.transform;

            float relativeHeading = Vector3.Dot(
                selfTransform.forward, targetTransform.forward
                );
            float relativePosition = Vector3.Dot(
                targetTransform.forward,
                (selfTransform.position - targetTransform.position).normalized
            );
            float distance = Vector3.Distance(
                selfTransform.position, targetTransform.position
                );
            float timeToInterception = EstimateTimeToInterception(
                relativeHeading, relativePosition, distance)
                ;
            Debug.Log($"Heading: {relativeHeading}; Position: {relativePosition}; Distance: {distance}; Time: {timeToInterception}");

            float speedRatio;
            if (
                target.Moveable.CurrentSpeed == 0f &&
                target.Moveable.CurrentAngularSpeed == 0f
                )
            {
                speedRatio = 0f;
            }
            else if (target.Moveable.CurrentSpeed == 0f)
            {
                speedRatio = 1f / _maxSpeed;
            }
            else
            {
                speedRatio = target.Moveable.CurrentSpeed / _maxSpeed;
            }

            return new SelectionInfo(
                targetTransform.position +
                speedRatio * timeToInterception * target.Moveable.Heading
                );
        }

        private float EstimateTimeToInterception(
            float relativeHeading, float relativePosition, float distance
            )
        {
            if (relativeHeading == 1 && relativePosition > 0)
            {
                // aligned & ahead => already past the point of interception
                return 0f;
            }

            relativeHeading = (relativeHeading + 2f) / 2f;
            relativePosition = (relativePosition + 2f) / 2f;
            return _turningParam * distance / relativeHeading / relativePosition;
        }
    }
}
