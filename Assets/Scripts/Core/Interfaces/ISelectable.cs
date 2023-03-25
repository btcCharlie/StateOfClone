using UnityEngine;
using UnityEngine.Events;

namespace StateOfClone.Core
{
    public interface ISelectable
    {
        GameObject gameObject { get; }
        UnityEvent OnSelected { get; }
        UnityEvent OnDeselected { get; }
    }
}