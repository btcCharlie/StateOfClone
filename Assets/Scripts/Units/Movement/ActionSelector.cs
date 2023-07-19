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
        private ISteeringBehavior _currentBehavior;
        private readonly List<ISteeringBehavior> _behaviors = new();

        public UnitData UnitData
        {
            get { return GetComponent<Unit>().UnitData; }
        }
        public Locomotion Locomotion
        {
            get { return GetComponent<Locomotion>(); }
        }

        public ISteeringBehavior CurrentBehavior
        {
            get { return _currentBehavior; }
        }

        private List<Vector3> _path;

        private void Awake()
        {
            _unit = GetComponent<Unit>();

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

        public void RemoveBehavior(ISteeringBehavior behavior)
        {
            _behaviors.Remove(behavior);
        }
    }
}
