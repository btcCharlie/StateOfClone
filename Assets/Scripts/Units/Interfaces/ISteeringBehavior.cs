using UnityEngine;

namespace StateOfClone.Units
{
    public interface ISteeringBehavior
    {
        SteeringType SteeringType { get; }

        SteeringParams GetSteering(SelectionInfo self, SelectionInfo target);
    }
}