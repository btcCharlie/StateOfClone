using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.GameMap
{
    [RequireComponent(typeof(MeshCollider), typeof(MeshFilter))]
    public class MapCollider : MonoBehaviour
    {
        private MeshCollider _meshCollider;

        private void Awake()
        {
            _meshCollider = GetComponent<MeshCollider>();
        }

        private void Start()
        {
        }

        public void GenerateColliderMesh()
        {
            Mesh colliderMesh = MeshUtils.MergeVertices(
                GetComponent<MeshFilter>().sharedMesh
                );
            colliderMesh.name = "Collider Mesh";
            _meshCollider.sharedMesh = colliderMesh;
        }
    }
}
