using UnityEngine;
using UnityEngine.InputSystem;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    /// <summary>
    /// Component that controls the singleton camera that navigates the hex map.
    /// </summary>
    public class MapCamera : MonoBehaviour
    {

        [SerializeField]
        private float stickMinZoom, stickMaxZoom;

        [SerializeField]
        private float swivelMinZoom, swivelMaxZoom;

        [SerializeField]
        private float moveSpeedMinZoom, moveSpeedMaxZoom;

        [SerializeField]
        private float rotationSpeed;

        [SerializeField]
        private HexGrid grid;

        [SerializeField] private PlayerInput playerInput;
        private InputAction scrollAction, zoomAction, zoomIncrementAction, rotateAction;
        private Vector2 moveInput;
        private float rotationInput;
        private bool shouldMove, shouldRotate, shouldZoom;

        private Transform swivel, stick;

        private float zoom = 1f;

        private float rotationAngle;

        private static MapCamera instance;

        /// <summary>
        /// Whether the singleton camera controls are locked.
        /// </summary>
        public static bool Locked
        {
            set => instance.enabled = !value;
        }

        /// <summary>
        /// Validate the position of the singleton camera.
        /// </summary>
        public static void ValidatePosition() => instance.AdjustPosition();

        private void Awake()
        {
            swivel = transform.GetChild(0);
            stick = swivel.GetChild(0);

            scrollAction = playerInput.actions["MapScroll"];
            zoomAction = playerInput.actions["MapZoom"];
            zoomIncrementAction = playerInput.actions["MapZoomIncrement"];
            rotateAction = playerInput.actions["MapRotate"];
        }

        private void Start()
        {
            AdjustZoom();
        }

        private void OnEnable()
        {
            zoomAction.performed += OnZoom;
            zoomAction.performed += OnZoomIncrement;
            zoomAction.canceled += OnZoomIncrement;
            scrollAction.performed += OnScroll;
            scrollAction.canceled += OnScroll;
            rotateAction.performed += OnRotation;
            rotateAction.canceled += OnRotation;

            instance = this;
            ValidatePosition();
        }

        private void OnDisable()
        {
            zoomAction.performed -= OnZoom;
            zoomAction.performed -= OnZoomIncrement;
            zoomAction.canceled -= OnZoomIncrement;
            scrollAction.performed -= OnScroll;
            scrollAction.canceled -= OnScroll;
            rotateAction.performed -= OnRotation;
            rotateAction.canceled -= OnRotation;
        }

        private void Update()
        {
            if (shouldMove)
                AdjustPosition();

            if (shouldRotate)
                AdjustRotation();

            if (shouldZoom)
                AdjustZoom();
        }

        // AdjustPosition in tutorial
        private void OnScroll(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            shouldMove = context.performed;
        }

        private void OnRotation(InputAction.CallbackContext context)
        {
            rotationInput = context.ReadValue<float>();
            shouldRotate = context.performed;
        }

        private void OnZoom(InputAction.CallbackContext context)
        {
            float zoomDelta = context.ReadValue<float>();

            // AdjustZoom in tutorial
            zoom = Mathf.Clamp01(zoom + zoomDelta);

            AdjustZoom();
        }

        private void OnZoomIncrement(InputAction.CallbackContext context)
        {
            shouldZoom = context.performed;
        }

        private void AdjustRotation()
        {
            rotationAngle += rotationInput * rotationSpeed * Time.deltaTime;
            if (rotationAngle < 0f)
                rotationAngle += 360f;
            else if (rotationAngle >= 360f)
                rotationAngle -= 360f;
            transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
        }

        private void AdjustPosition()
        {
            Vector3 direction = new(moveInput.x, 0f, moveInput.y);
            float damping = Mathf.Max(Mathf.Abs(moveInput.x), Mathf.Abs(moveInput.y));
            float distance =
                Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) *
                damping * Time.deltaTime;

            Vector3 position = transform.localPosition;
            position += transform.localRotation * direction * distance;
            transform.localPosition =
                grid.Wrapping ? WrapPosition(position) : ClampPosition(position);
        }

        private void AdjustZoom()
        {
            float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
            stick.localPosition = new Vector3(0f, 0f, distance);

            float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
            swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            float xMax = (grid.CellCountX - 0.5f) * HexMetrics.innerDiameter;
            position.x = Mathf.Clamp(position.x, 0f, xMax);

            float zMax = (grid.CellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);

            return position;
        }

        private Vector3 WrapPosition(Vector3 position)
        {
            float width = grid.CellCountX * HexMetrics.innerDiameter;
            while (position.x < 0f)
                position.x += width;
            while (position.x > width)
                position.x -= width;

            float zMax = (grid.CellCountZ - 1) * (1.5f * HexMetrics.outerRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);

            grid.CenterMap(position.x);
            return position;
        }
    }
}
