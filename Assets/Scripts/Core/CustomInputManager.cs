using UnityEngine;
using UnityEngine.InputSystem;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(PlayerInput))]
    public class CustomInputManager : MonoBehaviour
    {
        public static CustomInputManager Instance { get; private set; }

        [field: SerializeField] public PlayerInput PlayerInput { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;
        }
    }
}
