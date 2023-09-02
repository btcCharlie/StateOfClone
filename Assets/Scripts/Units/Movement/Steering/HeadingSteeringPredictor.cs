using UnityEngine;

namespace StateOfClone.Units
{
    public class HeadingSteeringPredictor : ISteeringPredictor
    {
        private float _turningParam;

        public HeadingSteeringPredictor(float turningParam)
        {
            _turningParam = turningParam;
        }

        public SelectionInfo PredictPosition(SelectionInfo self, SelectionInfo target)
        {
            float relativeHeading = Vector3.Dot(
                self.Transform.forward, target.Transform.forward
                );
            float relativePosition = Vector3.Dot(
                target.Transform.forward,
                (self.Transform.position - target.Transform.position).normalized
            );
            float distance = Vector3.Distance(
                self.Position, target.Transform.position
                );
            float timeToInterception = EstimateTimeToInterception(
                relativeHeading, relativePosition, distance)
                ;
            Debug.Log($"Heading: {relativeHeading}; Position: {relativePosition}; Distance: {distance}; Time: {timeToInterception}");

            Vector3 targetHeading = new(
                target.Transform.forward.x, 0f, target.Transform.forward.z
            );
            targetHeading = targetHeading.normalized;

            return new SelectionInfo(
                target.Transform.position + target.Speed * timeToInterception * targetHeading
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
