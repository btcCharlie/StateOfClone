using UnityEngine;

namespace StateOfClone.Units
{
    [CreateAssetMenu(fileName = "UnitData", menuName = "StateOfClone/UnitData", order = 0)]
    public class UnitData : ScriptableObject
    {
        public int Speed;
        public int VisionRange;
        public int Health;
    }
}
