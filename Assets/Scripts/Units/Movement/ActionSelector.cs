using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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
        private ISteeringBehavior _currentBehavior;
        private readonly List<ISteeringBehavior> _behaviors = new();

        private List<Vector3> _path;
        public List<ISteeringBehavior> Behaviors
        {
            get { return _behaviors; }
        }

        public ISteeringBehavior CurrentBehavior
        {
            get { return _currentBehavior; }
        }

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _locomotion = GetComponent<Locomotion>();

            _path = new List<Vector3>();
        }

        private void FixedUpdate()
        {
        }

        public void SetBehavior(ISteeringBehavior newBehavior)
        {
            _currentBehavior = newBehavior;
        }

        public void AddWaypoint(Vector3 newWaypoint)
        {
            _path.Add(newWaypoint);
        }

        public void ClearPath()
        {
            _path.Clear();
        }

        public void AddBehavior(ISteeringBehavior behavior)
        {
            _behaviors.Add(behavior);
        }

        public void AddBehavior(string behaviorType)
        {
            ISteeringBehavior newBehavior = null;

#if UNITY_EDITOR
            _unit = GetComponent<Unit>();
            _locomotion = GetComponent<Locomotion>();
#endif

            switch (behaviorType)
            {
                case "SteeringSeek":
                    newBehavior = SteeringBehaviorFactory.CreateSteeringSeek(
                        _unit.UnitData, _locomotion
                        );
                    break;
                case "SteeringArrival":
                    newBehavior = SteeringBehaviorFactory.CreateSteeringArrival(
                        _unit.UnitData, _locomotion
                        );
                    break;
                    // Add more cases for other types of steering behaviors...
            }

            if (newBehavior == null)
            {
                return;
            }

            _behaviors.Add(newBehavior);
        }

        public bool RemoveBehavior(ISteeringBehavior behavior)
        {
            return _behaviors.Remove(behavior);
        }

        public bool RemoveBehavior(string behaviorType)
        {
            ISteeringBehavior behaviorToRemove = default(SteeringBehavior);
            foreach (ISteeringBehavior behavior in _behaviors)
            {
                if (behavior.GetType().Name == behaviorType)
                {
                    behaviorToRemove = behavior;
                    break;
                }
            }

            if (behaviorToRemove != default(SteeringBehavior))
            {
                return _behaviors.Remove(behaviorToRemove);
            }

            return false;
        }
    }
}
