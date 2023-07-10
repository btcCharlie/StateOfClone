using UnityEngine;

namespace StateOfClone.Units
{
    public class TrackedLocomotion : Locomotion
    {
        [SerializeField] private float _inPlaceTurnAngleLimitDegrees = 30f;

        protected override void ApplySteering(Vector3 newPosition, Quaternion newRotation)
        {
            // Update the unit's position and rotation
            if (Mathf.Abs(CurrentAngularSpeedDegPerSec) < _inPlaceTurnAngleLimitDegrees)
            {
                _rb.position = newPosition;
            }
            _rb.rotation = newRotation;
        }
    }
}