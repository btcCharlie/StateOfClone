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
        /// Maximum speed (velocity magnitude) in units per second.
        /// </summary>
        public float MaxSpeed;
        /// <summary>
        /// Maximum angular acceleration in degrees per second squared.
        /// </summary>
        public float MaxAngularAcceleration;
        /// <summary>
        /// Maximum rate of turning in degrees per second.
        /// </summary>
        public float MaxTurnRate;
        public int VisionRange;
        public int Health;
        public bool IsAirborne;

        public float FrictionCoefficient;
    }
}
