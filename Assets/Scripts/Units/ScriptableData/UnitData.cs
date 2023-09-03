using UnityEngine;

namespace StateOfClone.Units
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "StateOfClone/UnitData", order = 0)]
    public class UnitData : ScriptableObject
    {
        /// <summary>
        /// [unit/s/s] Maximum acceleration.
        /// </summary>
        [Tooltip("[unit/s/s] Maximum acceleration.")]
        public float MaxAcceleration;
        /// <summary>
        /// [unit/s] Maximum possible speed.
        /// </summary>
        [Tooltip("[unit/s] Maximum possible speed.")]
        public float MaxSpeed;
        /// <summary>
        /// [unit/s] Speed at max turning rate.
        /// </summary>
        [Tooltip("[unit/s] Speed at max turning rate.")]
        public float MinSpeed;
        /// <summary>
        /// [deg/s/s] Maximum angular acceleration.
        /// </summary>
        [Tooltip("[deg/s/s] Maximum angular acceleration.")]
        public float MaxAngularAcceleration;
        /// <summary>
        /// [deg/s] Maximum rate of turning.
        /// </summary>
        [Tooltip("[deg/s] Maximum rate of turning.")]
        public float MaxTurnRate;
        /// <summary>
        /// [deg/s] Turning rate at max speed.
        /// </summary>
        [Tooltip("[deg/s] Turning rate at max speed.")]
        public float MinTurnRate;
        /// <summary>
        /// [deg] Above this angle to target, a supported vehicle type turns in place.
        /// </summary>
        [Tooltip("[deg] Above this angle to target, a supported vehicle type turns in place.")]
        public float InPlaceTurnLimit;
        public int VisionRange;
        public int Health;
        public bool IsAirborne;

        /// <summary>
        /// How much the vehicle should slow down its yaw turn rate when approaching desired rotation. 
        /// </summary>
        [Tooltip("[] How much the vehicle should slow down its yaw turn rate when approaching desired rotation.")]
        [Range(1f, 10f)]
        public float YawCurveScale = 3f;
        /// <summary>
        /// How abruptly the speed should change for this vehicle.
        /// </summary>
        [Tooltip("[] How abruptly the speed should change for this vehicle.")]
        [Range(0.005f, 2f)]
        public float SpeedCurveSlant = 0.1f;

        /// <summary>
        /// [] How many recent normals should be stored for smoothing?
        /// </summary>
        [Tooltip("[] How many recent normals should be stored for smoothing?")]
        [Range(1, 50)]
        public int SmoothingNormalsCount = 10;
        /// <summary>
        /// [] How many recent speeds should be stored for smoothing?
        /// </summary>
        [Tooltip("[] How many recent speeds should be stored for smoothing?")]
        [Range(1, 50)]
        public int SmoothingSpeedsCount = 10;
        /// <summary>
        /// [] How many recent angular speeds should be stored for smoothing?
        /// </summary>
        [Tooltip("[] How many recent angular speeds should be stored for smoothing?")]
        [Range(1, 50)]
        public int SmoothingAngularsCount = 10;

        /// <summary>
        /// [unit] Range of the main weapon.
        /// </summary>
        [Tooltip("[unit] Range of the main weapon.")]
        [Range(0, 1000)]
        public float MainWeaponRange = 100f;

        public float FrictionCoefficient;
    }
}
