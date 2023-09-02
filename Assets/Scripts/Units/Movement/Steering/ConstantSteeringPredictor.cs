using UnityEngine;

namespace StateOfClone.Units
{
    public class ConstantSteeringPredictor : ISteeringPredictor
    {
        private float _headingOffset;

        public ConstantSteeringPredictor(float headingOffset)
        {
            _headingOffset = headingOffset;
        }

        public SelectionInfo PredictPosition(SelectionInfo self, SelectionInfo target)
        {
            Vector3 targetHeading = new(
                target.Transform.forward.x, 0f, target.Transform.forward.z
            );
            targetHeading = targetHeading.normalized;

            return new SelectionInfo(
                target.Transform.position + targetHeading * _headingOffset
                );
        }
    }
}
