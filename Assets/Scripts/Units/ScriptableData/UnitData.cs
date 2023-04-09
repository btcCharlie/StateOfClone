using UnityEngine;

namespace StateOfClone.Units
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "StateOfClone/UnitData", order = 0)]
    public class UnitData : ScriptableObject
    {
        public float MaxForce;
        public float MaxSpeed;
        public float MaxTurnRateDegPerSec;
        public int VisionRange;
        public int Health;
    }
}
