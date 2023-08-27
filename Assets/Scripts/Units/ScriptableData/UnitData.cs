using UnityEngine;

namespace StateOfClone.Units
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "StateOfClone/UnitData", order = 0)]
    public class UnitData : ScriptableObject
    {
        /// <summary>
        /// Maximum acceleration in units per second squared.
        /// </summary>
        public float MaxAcceleration;
        /// <summary>
        /// Maximum possible speed in units per second.
        /// </summary>
        public float MaxSpeed;
        /// <summary>
        /// Speed at max turning rate in units per second.
        /// </summary>
        public float MinSpeed;
        /// <summary>
        /// Maximum angular acceleration in degrees per second squared.
        /// </summary>
        public float MaxAngularAcceleration;
        /// <summary>
        /// Maximum rate of turning in degrees per second.
        /// </summary>
        public float MaxTurnRate;
        /// <summary>
        /// Turning rate at max speed in degrees per second.
        /// </summary>
        public float MinTurnRate;
        public int VisionRange;
        public int Health;
        public bool IsAirborne;

        /// <summary>
        /// How much the vehicle should slow down its yaw turn rate when approaching desired rotation. 
        /// </summary>
        [RangeAttribute(1f, 10f)]
        public float YawCurveScale = 3f;
        /// <summary>
        /// How abruptly the speed should change for this vehicle.
        /// </summary>
        [RangeAttribute(0.005f, 2f)]
        public float SpeedCurveSlant = 0.1f;

        public float FrictionCoefficient;
    }
}
