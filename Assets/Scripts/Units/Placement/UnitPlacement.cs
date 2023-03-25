using UnityEngine;
using UnityEngine.InputSystem;

namespace StateOfClone.Units
{
    public class UnitPlacement : MonoBehaviour
    {
        private Camera _camera;

        private PlayerInput _playerInput;
        private InputAction _unitSpawnAction;

        [SerializeField] private LayerMask _groundLayer;

        private void Awake()
        {
            _playerInput = CustomInputManager.Instance.PlayerInput;
            _unitSpawnAction = _playerInput.actions["SpawnUnit"];
        }

        void Start()
        {
            _camera = Camera.main;
        }

        private void OnEnable()
        {
            _unitSpawnAction.performed += OnUnitSpawn;
        }

        private void OnDisable()
        {
            _unitSpawnAction.performed -= OnUnitSpawn;
        }

        private void OnUnitSpawn(InputAction.CallbackContext context)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                UnitManager.Instance.SpawnUnit(hit.point);
            }
        }
    }
}
