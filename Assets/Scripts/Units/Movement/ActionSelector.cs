using System.Collections.Generic;
using UnityEngine;

namespace StateOfClone.Units
{
    /// <summary>
    /// Top-level selection of movement behavior.
    /// </summary>
    public class ActionSelector : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayer;

        private Unit _unit;
        private Locomotion _locomotion;
        private readonly List<ISteeringBehavior> _steeringBehaviors = new();

        public List<ISteeringBehavior> Behaviors
        {
            get { return _steeringBehaviors; }
        }

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _locomotion = GetComponent<Locomotion>();
        }

        public void AddBehavior(ISteeringBehavior behavior)
        {
            _steeringBehaviors.Add(behavior);
        }

        public void AddBehavior(SteeringType steeringType)
        {
            ISteeringBehavior newSteering = null;

#if UNITY_EDITOR
            _unit = GetComponent<Unit>();
            _locomotion = GetComponent<Locomotion>();
#endif

            if (
                steeringType == SteeringType.Pursuit ||
                steeringType == SteeringType.OffsetPursuit ||
                steeringType == SteeringType.Evasion
                )
            {
                HeadingSteeringPredictor predictor = new(
                    0.5f, _unit.UnitData.MaxSpeed
                    );
                newSteering = SteeringBehaviorFactory.CreateBehavior(
                    steeringType, _unit.UnitData, _locomotion, predictor
                    );
            }
            else if (steeringType == SteeringType.Arrival)
            {
                float parameter = 10f;
                newSteering = SteeringBehaviorFactory.CreateBehavior(
                    steeringType, _unit.UnitData, _locomotion, parameter
                    );
            }
            else
            {
                newSteering = SteeringBehaviorFactory.CreateBehavior(
                    steeringType, _unit.UnitData, _locomotion
                    );
            }

            if (newSteering == null)
            {
                return;
            }

            _steeringBehaviors.Add(newSteering);
        }

        public bool RemoveBehavior(ISteeringBehavior steering)
        {
            return _steeringBehaviors.Remove(steering);
        }

        public bool RemoveBehavior(string behaviorType)
        {
            ISteeringBehavior steeringToRemove = default(SteeringBehavior);
            foreach (ISteeringBehavior behavior in _steeringBehaviors)
            {
                if (behavior.GetType().Name == behaviorType)
                {
                    steeringToRemove = behavior;
                    break;
                }
            }

            if (steeringToRemove != default(SteeringBehavior))
            {
                return _steeringBehaviors.Remove(steeringToRemove);
            }

            return false;
        }

        public bool RemoveBehavior(SteeringType steeringType)
        {
            ISteeringBehavior steeringToRemove = default(SteeringBehavior);
            foreach (ISteeringBehavior steering in _steeringBehaviors)
            {
                if (steering.SteeringType == steeringType)
                {
                    steeringToRemove = steering;
                    break;
                }
            }

            if (steeringToRemove != default(SteeringBehavior))
            {
                return _steeringBehaviors.Remove(steeringToRemove);
            }

            return false;
        }
    }
}
