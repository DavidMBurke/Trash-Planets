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
    private Color originalColor = Color.white;
    private Color highlightColor = Color.red;
    public Material highlightMaterial;
    private float highlightIntensity = 0f;

    private void Start()
    {
        cameraComponent = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        highlightMaterial.SetFloat("_highlightIntensity", highlightIntensity);
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);

        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            MeshFilter meshFilter = hit.collider.GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;
                Vector3 planetCenter = planet.gameObject.transform.localPosition;
                Vector3[] vertices = mesh.vertices;
                Color[] colors = mesh.colors.Length == vertices.Length ? mesh.colors : new Color[vertices.Length];
                if (colors.Length != vertices.Length)
                {
                    for (int i = 0; i < vertices.Length; i++)
                    {
                        colors[i] = originalColor;
                    }
                }    


                if (lastHighlightedMeshFilter != null && lastHighlightedTriangleIndices != null)
                {
                    for (int i = 0; i < lastHighlightedTriangleIndices.Length; i++)
                    {
                        colors[lastHighlightedTriangleIndices[i]] = originalColor;
                    }
                    lastHighlightedMeshFilter.mesh.colors = colors;
                }

                int[] triangles = mesh.triangles;
                int triangleIndex = hit.triangleIndex;

                int vert1 = triangles[triangleIndex * 3];
                int vert2 = triangles[triangleIndex * 3 + 1];
                int vert3 = triangles[triangleIndex * 3 + 2];
                lastHighlightedTriangleIndices = new int[] {vert1, vert2, vert3};

                foreach (int index in lastHighlightedTriangleIndices)
                {
                    colors[index] = highlightColor;
                }

                highlightIntensity = 1f;
                highlightMaterial.SetFloat("_highlightIntensity", highlightIntensity);

                if (Input.GetMouseButtonDown(0))
                {
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

                }

                mesh.colors = colors;
                lastHighlightedMeshFilter = meshFilter;
                mesh.vertices = vertices;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                meshFilter.mesh = mesh;
            }
        }
        else
        {
            highlightIntensity = 1f;
            highlightMaterial.SetFloat("_highlightIntensity", highlightIntensity);
        }
    }

    private Vector3 MoveVertexTowardCenter(Vector3 vertex, Vector3 center, float amount) {
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
