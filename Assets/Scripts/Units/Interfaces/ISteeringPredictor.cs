using UnityEngine;

namespace StateOfClone.Units
{
    public interface ISteeringPredictor
    {
        TargetInfo PredictPosition(Vector3 position, TargetInfo target);
    }
}
