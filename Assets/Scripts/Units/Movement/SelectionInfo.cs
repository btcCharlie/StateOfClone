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
        public IMoveable Moveable { get; private set; }

        public SelectionInfo(Vector3 position)
        {
            Type = SelectionType.Ground;
            Position = position;
            Moveable = null;
        }

        public SelectionInfo(IMoveable moveable)
        {
            Type = SelectionType.Moveable;
            Position = moveable.transform.position;
            Moveable = moveable;
        }

        public override readonly string ToString()
        {
            string infoString;
            if (Type == SelectionType.Ground)
            {
                infoString = $"Type: Ground; Position: {Position.x},{Position.y},{Position.z}";
            }
            else if (Type == SelectionType.Moveable)
            {
                string temp1 = $"Name: {Moveable.transform.gameObject.name};";
                string temp2 = $"Position: {Moveable.transform.position.x},{Moveable.transform.position.y},{Moveable.transform.position.z}";
                infoString = $"Type: Moveable; {temp1}, {temp2}";
            }
            else
            {
                infoString = "Type:: None";
            }
            return infoString;
        }
    }
}
