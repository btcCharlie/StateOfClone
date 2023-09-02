using UnityEngine;

namespace StateOfClone.Units
{
    public interface IMoveable
    {
        float CurrentSpeed { get; }
        float CurrentAngularSpeed { get; }
        Vector3 Heading { get; }
        Vector3 Velocity => CurrentSpeed * Heading;
        UnitMove UnitMove { get; }
        Transform transform { get; }
    }
}
