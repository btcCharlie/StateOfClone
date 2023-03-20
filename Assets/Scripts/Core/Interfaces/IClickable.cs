using UnityEngine.Events;

namespace StateOfClone.Core
{
    public interface IClickable
    {
        UnityEvent OnSelected { get; set; }
    }
}