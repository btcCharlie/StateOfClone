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
        private readonly List<ISteeringBehavior> _steeringBehaviors = new();

        private List<Vector3> _path;
        public List<ISteeringBehavior> Behaviors
        {
            get { return _steeringBehaviors; }
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
            _steeringBehaviors.Add(behavior);
        }

        public void AddBehavior(SteeringType steeringType)
        {
            ISteeringBehavior newSteering = null;

#if UNITY_EDITOR
            _unit = GetComponent<Unit>();
            _locomotion = GetComponent<Locomotion>();
#endif

            newSteering = SteeringBehaviorFactory.CreateBehavior(
                steeringType, _unit.UnitData, _locomotion
            );

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
