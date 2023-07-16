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
        private SteeringBehavior _currentBehavior;

        public SteeringBehavior CurrentBehavior
        {
            get { return _currentBehavior; }
        }

        private List<Vector3> _path;

        private void Awake()
        {
            _unit = GetComponent<Unit>();

            _path = new List<Vector3>();
        }

        public void SetBehavior(Type newBehaviorType)
        {
            if (_currentBehavior != null)
            {
#if UNITY_EDITOR
                if (!EditorUtility.IsPersistent(_currentBehavior))
                {
                    DestroyImmediate(_currentBehavior);
                }
#else
                Destroy(_currentBehavior);
#endif
            }

            _currentBehavior = gameObject.AddComponent(newBehaviorType) as SteeringBehavior;
        }

        public void AddWaypoint(Vector3 newWaypoint)
        {
            _path.Add(newWaypoint);
        }

        public void ClearPath()
        {
            _path.Clear();
        }


    }
}
