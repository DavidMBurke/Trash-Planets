using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public float gravity = 9.8f;

    public int totalTrash;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colorSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColorGenerator colorGenerator = new ColorGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 rotationalVelocity;

    public GameObject otherPlanet;

    public GameManagerScript GameManager;
    public float G = 10;
    public bool orbitToggle;
    public int trashOnPlanet;
    private float trashUpdateTime = 5f;
    private float trashUpdateTimer = 0;


    private void Start()
    {
        GeneratePlanet();
        updateAcceleration();
        trashOnPlanet = CalculateTrashCount();
    }

    void Initialize()
    {
        totalTrash = 0;
        shapeGenerator.UpdateSettings(shapeSettings);
        colorGenerator.UpdateSettings(colorSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColors();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColorSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColors();
        }
    }

    void GenerateMesh()
    {
        foreach (TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
            totalTrash += face.mesh.triangles.Length;
        }
        colorGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColors()
    {
        colorGenerator.UpdateColors();
    }

    void Update()
    {
        if (!GameManager.isPaused){
            updateAcceleration();
            velocity += acceleration;
            if (orbitToggle)
            {
                transform.position += velocity;
            }
            transform.Rotate(rotationalVelocity);
            trashUpdateTimer += Time.deltaTime;
            if (trashUpdateTimer > trashUpdateTime)
            {
                trashUpdateTimer = 0;
                trashOnPlanet = CalculateTrashCount();
            }
        }
    }

    public List<MeshFilter> GetMeshFiltersInRadius(Vector3 position, float radius)
    {
        List<MeshFilter> meshFiltersInRadius = new List<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null) continue;

            Bounds meshBounds = meshFilter.mesh.bounds;
            meshBounds = TransformBounds(meshFilter.transform, meshBounds);

            if (BoundsIntersectsSphere(meshBounds, position, radius))
            {
                meshFiltersInRadius.Add(meshFilter);
            }
        }

        return meshFiltersInRadius;
    }

    private Bounds TransformBounds(Transform transform, Bounds localBounds)
    {
        Vector3 center = transform.TransformPoint(localBounds.center);
        Vector3 extents = localBounds.extents;
        Vector3 worldExtents = Vector3.Scale(extents, transform.lossyScale);

        return new Bounds(center, worldExtents * 2);
    }

    private bool BoundsIntersectsSphere(Bounds bounds, Vector3 sphereCenter, float sphereRadius)
    {
        Vector3 closestPoint = bounds.ClosestPoint(sphereCenter);
        float distance = Vector3.Distance(closestPoint, sphereCenter);
        return distance <= sphereRadius;
    }

    void updateAcceleration()
    {
        if (orbitToggle) {
            // Debug.Log("Our pose: " + transform.position);
            // Debug.Log("Their pose: " + otherPlanet.transform.position);

            Vector3 displacement = otherPlanet.transform.position - transform.position;
            // Debug.DrawLine(transform.position, otherPlanet.transform.position, Color.green);
            float scale = (float)(G/Math.Pow(displacement.sqrMagnitude, 1.5f));
            acceleration = displacement*scale;
            // Debug.DrawLine(transform.position, transform.position + acceleration*1000, Color.red);
        }
    }

    public int CalculateTrashCount()
    {
        float planetRadius = shapeSettings.planetRadius;
        float elevationSum = 0f;

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null || meshFilter.sharedMesh == null) continue;

            Mesh mesh = meshFilter.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            Transform meshTransform = meshFilter.transform;

            foreach (Vector3 vertex in vertices)
            {
                // Transform vertex to world space
                Vector3 worldVertex = meshTransform.TransformPoint(vertex);

                // Calculate the distance from the planet center
                float distanceFromCenter = (worldVertex - transform.position).magnitude;

                // Add the excess elevation above the planet's radius
                if (distanceFromCenter > planetRadius)
                {
                    elevationSum += (distanceFromCenter - planetRadius);
                }
            }
        }

        return (int)(elevationSum / GameSettings.minedTrashRatio);
    }

}
