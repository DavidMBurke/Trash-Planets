using System.Collections.Generic;
using UnityEngine;

public class VertexManipulator : MonoBehaviour
{
    public static void ExpandVerticesFromTriangle(MeshFilter meshFilter, Vector3 planetCenter, int triangleIndex, int trashCount, int elevation, int plateauSize)
    {
        if (meshFilter == null || meshFilter.mesh == null)
        {
            Debug.LogWarning("Invalid mesh filter or mesh.");
            return;
        }

        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Transform vertices to world space
        Vector3[] worldVertices = new Vector3[vertices.Length];
        Transform meshTransform = meshFilter.transform;
        for (int i = 0; i < vertices.Length; i++)
        {
            worldVertices[i] = meshTransform.TransformPoint(vertices[i]);
        }

        // Get the starting triangle's vertex indices
        int startVert1 = triangles[triangleIndex * 3];
        int startVert2 = triangles[triangleIndex * 3 + 1];
        int startVert3 = triangles[triangleIndex * 3 + 2];

        HashSet<int> visitedVertices = new HashSet<int>();
        Queue<int> currentLayer = new Queue<int>();
        Queue<int> nextLayer = new Queue<int>();

        // Initialize with the starting triangle vertices
        currentLayer.Enqueue(startVert1);
        currentLayer.Enqueue(startVert2);
        currentLayer.Enqueue(startVert3);
        visitedVertices.Add(startVert1);
        visitedVertices.Add(startVert2);
        visitedVertices.Add(startVert3);

        int elevatedVertexCount = 0;
        int plateauCounter = 0;

        // BFS to elevate vertices layer by layer
        while (currentLayer.Count > 0 && elevatedVertexCount < trashCount)
        {
            while (currentLayer.Count > 0)
            {
                int currentVertex = currentLayer.Dequeue();
                Vector3 directionToCenter = (worldVertices[currentVertex] - planetCenter).normalized;

                // Elevate the vertex
                worldVertices[currentVertex] += directionToCenter * elevation;
                elevatedVertexCount++;

                // Stop further adjustments if the total elevated vertices reach `x`
                if (elevatedVertexCount >= trashCount)
                    break;

                // Find neighboring vertices and add them to the next layer
                for (int j = 0; j < triangles.Length / 3; j++)
                {
                    int vert1 = triangles[j * 3];
                    int vert2 = triangles[j * 3 + 1];
                    int vert3 = triangles[j * 3 + 2];

                    if (vert1 == currentVertex || vert2 == currentVertex || vert3 == currentVertex)
                    {
                        TryEnqueueVertex(vert1, visitedVertices, nextLayer);
                        TryEnqueueVertex(vert2, visitedVertices, nextLayer);
                        TryEnqueueVertex(vert3, visitedVertices, nextLayer);
                    }
                }
            }

            // Move to the next layer
            var temp = currentLayer;
            currentLayer = nextLayer;
            nextLayer = temp;

            // Clear the next layer queue
            nextLayer.Clear();

            // Decrease elevation for the next layer if plateau size reached

            if (plateauCounter >= plateauSize)
            {
                elevation = Mathf.Max(1, elevation - 1); // Ensure elevation doesn't go below 1
                plateauCounter = 0;                
            } else
            {
                plateauCounter++;
            }
        }

        // Transform vertices back to local space and update the mesh
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = meshTransform.InverseTransformPoint(worldVertices[i]);
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Update the MeshCollider if present
        MeshCollider collider = meshFilter.GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.sharedMesh = null;
            collider.sharedMesh = mesh;
        }
    }

    private static void TryEnqueueVertex(int vertexIndex, HashSet<int> visitedVertices, Queue<int> queue)
    {
        if (!visitedVertices.Contains(vertexIndex))
        {
            queue.Enqueue(vertexIndex);
            visitedVertices.Add(vertexIndex);
        }
    }
}
