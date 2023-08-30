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

        public TargetInfo PredictPosition(Vector3 position, TargetInfo target)
        {
            Vector3 targetHeading = new(
                target.Transform.forward.x, 0f, target.Transform.forward.z
            );
            targetHeading = targetHeading.normalized;

            return new TargetInfo(
                target.Transform.position + targetHeading * _headingOffset
                );
        }
    }
}
