using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    public Transform VelocityTransform;

    private void Start()
    {
        GeneratePlanet();
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
        transform.Translate(VelocityTransform.position);
        transform.Rotate(VelocityTransform.rotation.eulerAngles);
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
}
