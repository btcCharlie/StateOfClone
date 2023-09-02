using UnityEngine;

namespace StateOfClone.Units
{
    public class LinearSteeringPredictor : ISteeringPredictor
    {
        private float _turningParam;

        public LinearSteeringPredictor(float turningParam)
        {
            _turningParam = turningParam;
        }

        public SelectionInfo PredictPosition(SelectionInfo self, SelectionInfo target)
        {
            float distance = Vector3.Distance(
                self.Position, target.Transform.position
                );
            float timeToInterception = EstimateTimeToInterception(distance);

            Vector3 targetHeading = new(
                target.Transform.forward.x, 0f, target.Transform.forward.z
            );
            targetHeading = targetHeading.normalized;

            return new SelectionInfo(
                target.Transform.position + targetHeading * timeToInterception
                );
        }

        private float EstimateTimeToInterception(float distance)
        {
            return distance * _turningParam;
        }
    }
}
