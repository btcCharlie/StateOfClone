using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    [RequireComponent(typeof(MeshCollider), typeof(MeshFilter))]
    public class MapCollider : MonoBehaviour
    {
        private Mesh _colliderMesh;
        private MeshCollider _collider;

        private void Start()
        {
            Mesh originalMesh = GetComponent<MeshFilter>().sharedMesh;

        }

        public void SimplifyMesh()
        {
            Mesh simplifiedMesh = MeshUtils.MergeVertices(_colliderMesh);
        }
    }
}
