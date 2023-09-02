using UnityEngine;

namespace StateOfClone.Units
{
    public interface ISteeringPredictor
    {
        SelectionInfo PredictPosition(SelectionInfo self, SelectionInfo target);
    }
}
