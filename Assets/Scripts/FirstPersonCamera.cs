using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class FirstPersonCamera : MonoBehaviour
{
    public Player player;
    public Planet planet;

    private Camera playerCamera;

    // For mesh highlighting logic
    private MeshFilter lastHighlightedMeshFilter;
    private int[] lastHighlightedTriangleIndices;
    private Color highlightColor = Color.red;
    private Dictionary<MeshFilter, Color[]> originalColorsCache = new Dictionary<MeshFilter, Color[]>();

    // For building placement logic
    public GameObject[] buildingPrefabs;
    public GameObject selectedPrefab;
    public Material previewMaterial;
    public float maxPlacementDistance = 50f;
    public float buildingPlacementVertOffset = 0.1f;
    private LayerMask placementLayerMask;
    private GameObject previewBuilding;
    private bool canPlace;
    private int selectedBuildingIndex = 0;

    // For refining logic
    private bool isRefining;
    private float refining_time_total = 3;
    private float refining_time_current = 0;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        placementLayerMask = LayerMask.GetMask("PlanetSurface");
    }

    private void Update()
    {
        HandleRefining();
        if (isRefining) return;
        HandleMining();
        HandleBuildingSelection();
        HandlePlacementPreview();
    }

    /// <summary>
    /// Refining logic
    /// </summary>
    private void HandleRefining()
    {
        if (player.trash_qty == 0)
        {
            isRefining = false;
            refining_time_current = 0;
            return;
        }

        if (Input.GetKey(KeyCode.R))
        {
            isRefining = true;
            refining_time_current += Time.deltaTime;
            if (refining_time_current >= refining_time_total)
            {
                player.trash_qty -= 1;
                player.building_mat_qty += 1;
                refining_time_current = 0;
            }
        }
        else
        {
            isRefining = false;
            refining_time_current = 0;
        }
    }



    /// <summary>
    /// Mining Logic
    /// </summary>
    private void HandleMining()
    {
        if (selectedPrefab != null)
        {
            return;
        }
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
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
        int vert1, vert2, vert3;

        try
        {
            vert1 = triangles[triangleIndex * 3];
            vert2 = triangles[triangleIndex * 3 + 1];
            vert3 = triangles[triangleIndex * 3 + 2];
        } 
        catch 
        { 
            return; 
        }

        lastHighlightedTriangleIndices = new int[] { vert1, vert2, vert3 };

        foreach (int index in lastHighlightedTriangleIndices)
        {
            colors[index] = highlightColor;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3[] vertices = mesh.vertices;
            Vector3 planetCenter = planet.gameObject.transform.position;
            Transform meshTransform = meshFilter.transform;

            Vector3 worldVert1 = meshTransform.TransformPoint(vertices[vert1]);
            Vector3 worldVert2 = meshTransform.TransformPoint(vertices[vert2]);
            Vector3 worldVert3 = meshTransform.TransformPoint(vertices[vert3]);

            float vertHeight1 = (worldVert1 - planetCenter).magnitude;
            float vertHeight2 = (worldVert2 - planetCenter).magnitude;
            float vertHeight3 = (worldVert3 - planetCenter).magnitude;

            List<(int index, Vector3 worldPos, float height)> verts = new List<(int, Vector3, float)> {
                            (vert1, worldVert1, vertHeight1),
                            (vert2, worldVert2, vertHeight2),
                            (vert3, worldVert3, vertHeight3)
                        };

            (int index, Vector3 worldPos, float height) highestVert = verts.OrderByDescending(x => x.height).First();

            (Vector3 modifiedWorldVert, bool successfulMine) = Mine(highestVert.worldPos, planetCenter, 1f);

            if (successfulMine)
            {
                vertices[highestVert.index] = meshTransform.InverseTransformPoint(modifiedWorldVert);

                mesh.vertices = vertices;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                meshFilter.mesh = mesh;
                MeshCollider meshCollider = meshFilter.GetComponent<MeshCollider>();
                meshCollider.sharedMesh = null;
                meshCollider.sharedMesh = mesh;
                player.trash_qty += 1;
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 planetCenter = planet.gameObject.transform.position;
            VertexManipulator.ExpandVerticesFromTriangle(meshFilter, planetCenter, triangleIndex, 1000, 3, 5);
        }

        mesh.colors = colors;
    }

    private void RestoreOriginalColors(MeshFilter meshFilter)
    {
        if (originalColorsCache.TryGetValue(meshFilter, out Color[] originalColors))
        {
            meshFilter.mesh.colors = (Color[])originalColors.Clone();
        }
    }



    private (Vector3 newVertex, bool succesfulMine) Mine(Vector3 vertex, Vector3 center, float amount)
    {
        float minRadius = (float)Math.Floor((planet.shapeSettings.planetRadius));
        Vector3 directionToCenter = (center - vertex).normalized;
        Vector3 newVertex = vertex + directionToCenter * amount;
        float newDistance = (float)Math.Floor((newVertex - center).magnitude);

        if (newDistance <= minRadius)
        {
            return (vertex, false);
        }
        return (newVertex, true);
    }

    /// <summary>
    /// Object Placement Logic
    /// </summary>

    private void HandlePlacementPreview()
    {
        if (selectedPrefab == null)
        {
            if (previewBuilding != null)
            {
                Destroy(previewBuilding);
                previewBuilding = null;
            }
            return;
        }
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxPlacementDistance, placementLayerMask))
        {
            if (previewBuilding == null || previewBuilding.name != selectedPrefab.name + "(Clone)")
            {
                if (previewBuilding != null) DestroyImmediate(previewBuilding, true);
                previewBuilding = Instantiate(selectedPrefab);

                Building buildingComponent = previewBuilding.GetComponent<Building>();
                if (buildingComponent != null)
                {
                    buildingComponent.planet = planet;
                }

                SetPreviewMaterial(previewBuilding);
            }


            previewBuilding.transform.position = hit.point + hit.normal * buildingPlacementVertOffset;
            previewBuilding.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);

            bool hasNoCollisions = !Physics.CheckBox(previewBuilding.transform.position, previewBuilding.GetComponent<Collider>().bounds.extents, previewBuilding.transform.rotation);
            bool canAfford = previewBuilding.GetComponent<Building>().cost <= player.building_mat_qty;
            bool canPlace = hasNoCollisions && canAfford;

            if (canPlace)
            {
                previewBuilding.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                previewBuilding.GetComponent<Renderer>().material.color = Color.red;
            }


            if (Input.GetMouseButtonDown(0) && canPlace)
            {
                PlaceBuilding();
            }
        }
        else if (previewBuilding != null)
        {
            DestroyImmediate(previewBuilding, true);
        }
    }

    private void HandleBuildingSelection()
    {
        if (buildingPrefabs.Length == 0) return;

        float scrollInput = Input.mouseScrollDelta.y;
        if (scrollInput > 0 || Input.GetKeyDown(KeyCode.RightBracket))
        {
            selectedBuildingIndex = (selectedBuildingIndex + 1) % buildingPrefabs.Length;
        }
        if (scrollInput < 0 || Input.GetKeyDown(KeyCode.LeftBracket))
        {
            selectedBuildingIndex = (selectedBuildingIndex - 1 + buildingPrefabs.Length) % buildingPrefabs.Length;
        }

        if (buildingPrefabs[selectedBuildingIndex] != null)
        {
            selectedPrefab = buildingPrefabs[selectedBuildingIndex];
        } else
        {
            selectedPrefab = null;
            if (previewBuilding != null)
            {
                Destroy(previewBuilding);
                previewBuilding = null;
            }
        }

    }
    private void PlaceBuilding()
    {
        GameObject placedBuilding = Instantiate(selectedPrefab, previewBuilding.transform.position, previewBuilding.transform.rotation);
        placedBuilding.GetComponent<Renderer>().material = null;

        Building buildingComponent = placedBuilding.GetComponent<Building>();
        player.building_mat_qty -= buildingComponent.cost;
        if (buildingComponent != null)
        {
            buildingComponent.planet = planet;
        }
    }

    private void SetPreviewMaterial(GameObject obj)
    {

        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            var materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterial;
            }
            renderer.materials = materials;
        }
    }
}



