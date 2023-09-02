using UnityEngine;

namespace StateOfClone.Units
{
    public enum SelectionType
    {
        None, Ground, Moveable
    }

    public struct SelectionInfo
    {
        public SelectionType Type { get; private set; }
        public Vector3 Position { get; private set; }
        public Transform Transform { get; private set; }
        public float Speed { get; private set; }

        public SelectionInfo(Vector3 position)
        {
            Type = SelectionType.Ground;
            Position = position;
            Transform = null;
            Speed = 0f;
        }

        public SelectionInfo(Transform transform, float speed)
        {
            Type = SelectionType.Moveable;
            Position = transform.position;
            Transform = transform;
            Speed = speed;
        }
    }
}
