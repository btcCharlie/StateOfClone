using UnityEngine;

namespace StateOfClone.Units
{
    public enum TargetType
    {
        None, Ground, Selectable
    }

    public class TargetInfo
    {
        public TargetType Type { get; private set; }
        public Vector3 Position { get; private set; }
        public Transform Transform { get; private set; }

        public TargetInfo(Vector3 position)
        {
            Type = TargetType.Ground;
            Position = position;
            Transform = null;
        }

        public TargetInfo(Transform transform)
        {
            Type = TargetType.Selectable;
            Position = transform.position;
            Transform = transform;
        }
    }
}
