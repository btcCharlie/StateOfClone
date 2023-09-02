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
            return new SelectionInfo(
                target.Moveable.transform.position + target.Moveable.Heading * _headingOffset
                );
        }
    }
}
