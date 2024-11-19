using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    public Transform playerBody;
    public Planet planet;

    private Camera cameraComponent;
    private float xRotation = 0f;
    private MeshFilter lastHighlightedMeshFilter;
    private int[] lastHighlightedTriangleIndices;
    private Color highlightColor = Color.red;

    private Dictionary<MeshFilter, Color[]> originalColorsCache = new Dictionary<MeshFilter, Color[]>();

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        HandleMeshHighlighting();
    }

    private void HandleMeshHighlighting()
    {
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;

                if (!originalColorsCache.ContainsKey(meshFilter))
                {
                    originalColorsCache[meshFilter] = mesh.colors.Length == mesh.vertexCount ? (Color[])mesh.colors.Clone() : Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();
                }

                if (lastHighlightedMeshFilter != null && lastHighlightedTriangleIndices != null)
                {
                    RestoreOriginalColors(lastHighlightedMeshFilter);
                }

                HighlightTriangle(meshFilter, hit);

                lastHighlightedMeshFilter = meshFilter;
            }
        }
        else if (lastHighlightedMeshFilter != null)
        {
            RestoreOriginalColors(lastHighlightedMeshFilter);
            lastHighlightedMeshFilter = null;
        }
    }

    private void HighlightTriangle(MeshFilter meshFilter, RaycastHit hit)
    {
        Mesh mesh = meshFilter.mesh;
        Color[] colors = mesh.colors.Length == mesh.vertexCount ? mesh.colors : Enumerable.Repeat(Color.white, mesh.vertexCount).ToArray();

        int[] triangles = mesh.triangles;
        int triangleIndex = hit.triangleIndex;

        int vert1 = triangles[triangleIndex * 3];
        int vert2 = triangles[triangleIndex * 3 + 1];
        int vert3 = triangles[triangleIndex * 3 + 2];
        lastHighlightedTriangleIndices = new int[] { vert1, vert2, vert3 };

        foreach (int index in lastHighlightedTriangleIndices)
        {
            colors[index] = highlightColor;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3[] vertices = mesh.vertices;
            Vector3 planetCenter = planet.gameObject.transform.localPosition;
            float vertHeight1 = (vertices[vert1] - planetCenter).magnitude;
            float vertHeight2 = (vertices[vert2] - planetCenter).magnitude;
            float vertHeight3 = (vertices[vert3] - planetCenter).magnitude;
            List<(int index, float height)> verts = new List<(int, float)> {
                            (vert1, vertHeight1),
                            (vert2, vertHeight2),
                            (vert3, vertHeight3)
                        };

            int highestVert = verts.OrderByDescending(x => x.height).First().index;

            vertices[highestVert] = MoveVertexTowardCenter(vertices[highestVert], planetCenter, 1f);

            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            meshFilter.mesh = mesh;

        }

        mesh.colors = colors;
        Debug.Log($"Highlighting vertices: {string.Join(", ", lastHighlightedTriangleIndices)}");
    }

    private void RestoreOriginalColors(MeshFilter meshFilter)
    {
        if (originalColorsCache.TryGetValue(meshFilter, out Color[] originalColors))
        {
            meshFilter.mesh.colors = (Color[])originalColors.Clone();
        }
    }



    private Vector3 MoveVertexTowardCenter(Vector3 vertex, Vector3 center, float amount)
    {
        float minRadius = (float)Math.Floor((planet.shapeSettings.planetRadius));
        Vector3 directionToCenter = (center - vertex).normalized;
        Vector3 newVertex = vertex + directionToCenter * amount;
        Debug.Log(minRadius);
        Debug.Log(newVertex.magnitude);
        if (newVertex.magnitude > minRadius)
        {
            return newVertex;
        }
        return vertex;
    }
}


