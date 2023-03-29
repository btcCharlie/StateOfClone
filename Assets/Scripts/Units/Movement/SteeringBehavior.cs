using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class SteeringBehavior : MonoBehaviour
    {
        public float Weight { get; set; }

        public GameObject Target { get; set; }

        public Vector3 Destination { get; set; }

        protected Agent agent;
        protected UnitData unitData;

        public float MaxSpeed { get; set; }
        public float MaxAcceleration { get; set; }
        public float MaxRotation { get; set; }
        public float maxAngularAcceleration { get; set; }

        protected virtual void Awake()
        {
            agent = GetComponent<Agent>();
            unitData = GetComponent<Unit>().UnitData;
        }

        protected virtual void Update()
        {
            agent.SetSteering(GetSteering(), Weight);
        }

        public float MapToRange(float rotation)
        {
            rotation %= 360f;
            if (Mathf.Abs(rotation) > 180f)
            {
                if (rotation < 0f)
                {
                    rotation += 360f;
                }
                else
                {
                    rotation -= 360f;
                }
            }

            return rotation;
        }

        public virtual SteeringParams GetSteering()
        {
            return new SteeringParams();
        }
    }
}
