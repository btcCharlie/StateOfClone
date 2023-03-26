using UnityEngine;
using UnityEngine.InputSystem;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(PlayerInput))]
    public class CustomInputManager : MonoBehaviour
    {
        public static CustomInputManager Instance { get; private set; }

        [field: SerializeField] public PlayerInput PlayerInput { get; private set; }

        private InputAction _ctrlEnableAction, _shiftEnableAction;

        [SerializeField] private string[] _ctrlEnableActionNames, _shiftEnableActionNames;
        private InputAction[] _ctrlEnableActions, _shiftEnableActions;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this;

            _ctrlEnableAction = PlayerInput.actions["CtrlEnable"];
            _ctrlEnableActions = new InputAction[_ctrlEnableActionNames.Length];
            for (int i = 0; i < _ctrlEnableActionNames.Length; i++)
                _ctrlEnableActions[i] = PlayerInput.actions[_ctrlEnableActionNames[i]];

            _shiftEnableAction = PlayerInput.actions["ShiftEnable"];
            _shiftEnableActions = new InputAction[_shiftEnableActionNames.Length];
            for (int i = 0; i < _shiftEnableActionNames.Length; i++)
                _shiftEnableActions[i] = PlayerInput.actions[_shiftEnableActionNames[i]];
        }

        private void OnEnable()
        {
            _ctrlEnableAction.started += OnCtrlEnableStarted;
            _ctrlEnableAction.canceled += OnCtrlEnableCanceled;

            _shiftEnableAction.started += OnShiftEnableStarted;
            _shiftEnableAction.canceled += OnShiftEnableCanceled;
        }

        private void OnDisable()
        {
            _ctrlEnableAction.started -= OnCtrlEnableStarted;
            _ctrlEnableAction.canceled -= OnCtrlEnableCanceled;

            _shiftEnableAction.started -= OnShiftEnableStarted;
            _shiftEnableAction.canceled -= OnShiftEnableCanceled;
        }

        private void OnCtrlEnableStarted(InputAction.CallbackContext context)
        {
            foreach (InputAction action in _ctrlEnableActions)
                action.Disable();
        }

        private void OnCtrlEnableCanceled(InputAction.CallbackContext context)
        {
            foreach (InputAction action in _ctrlEnableActions)
                action.Enable();
        }

        private void OnShiftEnableStarted(InputAction.CallbackContext context)
        {
            foreach (InputAction action in _shiftEnableActions)
                action.Disable();
        }

        private void OnShiftEnableCanceled(InputAction.CallbackContext context)
        {
            foreach (InputAction action in _shiftEnableActions)
                action.Enable();
        }
    }
}
