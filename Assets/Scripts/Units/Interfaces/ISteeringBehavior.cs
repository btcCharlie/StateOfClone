using UnityEngine;

namespace StateOfClone.Units
{
    public interface ISteeringBehavior
    {
        SteeringType SteeringType { get; }

        SteeringParams GetSteering(Vector3 position, TargetInfo target);
    }
}