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

        private List<Vector3> _path;

        private void Awake()
        {
            _unit = GetComponent<Unit>();

            _path = new List<Vector3>();
        }

        public void AddWaypoint(Vector3 newWaypoint)
        {
            _path.Add(newWaypoint);
        }

        public void ClearPath()
        {
            _path.Clear();
        }

        private void OnDrawGizmosSelected()
        {
            if (_path == null || _path.Count == 0)
                return;

            // draw the waypoints from path as small red spheres with the 
            // currently active waypoint (the last one) as a larger red sphere
            // also, draw a line between the waypoints, ending at the transform's
            // current position
            Color prevColor = Gizmos.color;
            Gizmos.color = Color.red;
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawSphere(_path[i], 0.5f);
                Gizmos.DrawLine(_path[i], _path[i + 1]);
            }
            Gizmos.DrawSphere(_path[^1], 1f);
            Gizmos.DrawLine(transform.position, _path[^1]);
            Gizmos.color = prevColor;
        }
    }
}
