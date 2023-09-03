using Codice.Client.BaseCommands.TubeClient;
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
        public Vector3 Position
        {
            readonly get
            {
                return Type switch
                {
                    SelectionType.Ground => _position,
                    SelectionType.Moveable => Moveable.transform.position,
                    _ => Vector3.zero
                };
            }
            private set { _position = value; }
        }
        public IMoveable Moveable { get; private set; }

        Vector3 _position;

        public SelectionInfo(Vector3 position)
        {
            Type = SelectionType.Ground;
            _position = position;
            Moveable = null;
        }

        public SelectionInfo(IMoveable moveable)
        {
            Type = SelectionType.Moveable;
            _position = moveable.transform.position;
            Moveable = moveable;
        }

        public override readonly string ToString()
        {
            string infoString;
            if (Type == SelectionType.Ground)
            {
                infoString = $"Type: Ground; Position: {_position.x},{_position.y},{_position.z}";
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
