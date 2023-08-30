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
}
