using UnityEngine;
using System.Collections.Generic;

namespace StateOfClone.Core
{
    public static class MeshUtils
    {
        /// <summary>
        /// Merges vertices in the given mesh that occupy the same position within the specified tolerance.
        /// Returns a new mesh with the merged vertices and reduced number of vertices.
        /// </summary>
        /// <param name="mesh">The mesh to merge vertices in.</param>
        /// <param name="tolerance">The tolerance within which vertices are considered to occupy the same position.</param>
        /// <returns>A new mesh with merged vertices.</returns>
        public static Mesh MergeVertices(Mesh mesh, float tolerance = 0.001f)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            // Merge the vertices
            Vector3[] mergedVertices = MergeVertices(vertices, tolerance);

            // Map the original vertex indices to the merged vertex indices
            int[] vertexMap = MapVertices(vertices, mergedVertices, tolerance);

            // Create a new array of triangles with the merged vertices
            int[] newTriangles = MapTriangles(triangles, vertexMap);

            // Create a new mesh with the merged vertices and triangles
            Mesh newMesh = new Mesh();
            newMesh.vertices = mergedVertices;
            newMesh.triangles = newTriangles;

            // Debug.Log($"Mesh vertex count reduced from {vertices.Length} to {mergedVertices.Length} vertices. Reduction of {(float)mergedVertices.Length / vertices.Length:P2}");

            return newMesh;
        }

        private static Vector3[] MergeVertices(Vector3[] vertices, float tolerance)
        {
            var hash = new Dictionary<Vector3, int>();
            var mergedVertices = new List<Vector3>(vertices.Length);

            for (int originalVertexIndex = 0; originalVertexIndex < vertices.Length; originalVertexIndex++)
            {
                var originalVertex = vertices[originalVertexIndex];
                var roundedVertex = new Vector3(
                    Mathf.Round(originalVertex.x / tolerance) * tolerance,
                    Mathf.Round(originalVertex.y / tolerance) * tolerance,
                    Mathf.Round(originalVertex.z / tolerance) * tolerance
                    );

                if (hash.TryGetValue(roundedVertex, out int mergedVertexIndex))
                {
                    // Vertex is already merged
                }
                else
                {
                    mergedVertexIndex = mergedVertices.Count;
                    mergedVertices.Add(originalVertex);
                    hash[roundedVertex] = mergedVertexIndex;
                }
            }

            return mergedVertices.ToArray();
        }

        private static int[] MapVertices(Vector3[] originalVertices, Vector3[] mergedVertices, float tolerance)
        {
            var hash = new Dictionary<Vector3, int>();
            var vertexMap = new int[originalVertices.Length];

            for (int i = 0; i < mergedVertices.Length; i++)
            {
                var mergedVertex = mergedVertices[i];
                var roundedVertex = new Vector3(Mathf.Round(mergedVertex.x / tolerance) * tolerance,
                                                Mathf.Round(mergedVertex.y / tolerance) * tolerance,
                                                Mathf.Round(mergedVertex.z / tolerance) * tolerance);

                hash[roundedVertex] = i;
            }

            for (int i = 0; i < originalVertices.Length; i++)
            {
                var originalVertex = originalVertices[i];
                var roundedVertex = new Vector3(Mathf.Round(originalVertex.x / tolerance) * tolerance,
                                                Mathf.Round(originalVertex.y / tolerance) * tolerance,
                                                Mathf.Round(originalVertex.z / tolerance) * tolerance);

                vertexMap[i] = hash[roundedVertex];
            }

            return vertexMap;
        }

        private static int[] MapTriangles(int[] triangles, int[] vertexMap)
        {
            var newTriangles = new int[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                newTriangles[i] = vertexMap[triangles[i]];
            }

            return newTriangles;
        }
    }
}
