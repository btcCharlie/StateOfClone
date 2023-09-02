using UnityEngine;

namespace StateOfClone.Units
{
    public class LinearSteeringPredictor : ISteeringPredictor
    {
        private float _turningParam;
        private float _maxSpeed;

        public LinearSteeringPredictor(float turningParam, float maxSpeed)
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
            float distance = Vector3.Distance(
                self.Position, target.Moveable.transform.position
                );
            float timeToInterception = EstimateTimeToInterception(distance);

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
                target.Moveable.transform.position +
                speedRatio * timeToInterception * target.Moveable.Heading
                );
        }

        private float EstimateTimeToInterception(float distance)
        {
            return distance * _turningParam;
        }
    }
}
