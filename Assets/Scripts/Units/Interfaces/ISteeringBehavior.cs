using UnityEngine;

namespace StateOfClone.Units
{
    public interface ISteeringBehavior
    {
        SteeringParams GetSteering(Vector3 position, Vector3 target);
    }
}