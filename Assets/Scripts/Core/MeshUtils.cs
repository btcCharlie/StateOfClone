using UnityEngine;

namespace StateOfClone.Core
{
    public static class MeshUtils
    {
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
            Mesh newMesh = new();
            newMesh.vertices = mergedVertices;
            newMesh.triangles = newTriangles;

            Debug.Log(
                $@"Mesh vertex count reduced from {vertices.Length} to 
                {mergedVertices.Length} vertices. Reduction of 
                {(float)mergedVertices.Length / vertices.Length:P2}"
                );

            return newMesh;
        }

        private static Vector3[] MergeVertices(Vector3[] vertices, float tolerance)
        {
            Vector3[] mergedVertices = new Vector3[vertices.Length];
            int newVertexIndex = 0;

            for (int originalVertexIndex = 0; originalVertexIndex < vertices.Length; originalVertexIndex++)
            {
                bool merged = false;

                for (int mergedVertexIndex = 0; mergedVertexIndex < newVertexIndex; mergedVertexIndex++)
                {
                    if (Vector3.Distance(mergedVertices[mergedVertexIndex], vertices[originalVertexIndex]) < tolerance)
                    {
                        merged = true;
                        break;
                    }
                }

                if (!merged)
                {
                    mergedVertices[newVertexIndex] = vertices[originalVertexIndex];
                    newVertexIndex++;
                }
            }

            Vector3[] result = new Vector3[newVertexIndex];
            for (int i = 0; i < newVertexIndex; i++)
            {
                result[i] = mergedVertices[i];
            }

            return result;
        }

        private static int[] MapVertices(Vector3[] originalVertices, Vector3[] mergedVertices, float tolerance)
        {
            int[] vertexMap = new int[originalVertices.Length];

            for (int i = 0; i < originalVertices.Length; i++)
            {
                for (int j = 0; j < mergedVertices.Length; j++)
                {
                    if (Vector3.Distance(originalVertices[i], mergedVertices[j]) < tolerance)
                    {
                        vertexMap[i] = j;
                        break;
                    }
                }
            }

            return vertexMap;
        }

        private static int[] MapTriangles(int[] triangles, int[] vertexMap)
        {
            int[] newTriangles = new int[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                newTriangles[i] = vertexMap[triangles[i]];
            }

            return newTriangles;
        }
    }
}
