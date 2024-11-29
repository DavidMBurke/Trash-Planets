using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : Building
{
    public BuildingButton[] CollectionButtons;
    public int maxTrash = 100;
    public int collectedTrash = 0;
    public float collectionTime = 5;
    public float elapsedTime = 0;
    public float radius = 10;

    private void Start()
    {
        AddFunctions();
    }

    private void Update()
    {
        TryMineTrash();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void AddFunctions()
    {
        foreach (var button in CollectionButtons)
        {
            button.function = () => collectTrash();
        }
    }

    private void TryMineTrash()
    {
        if (collectedTrash >= maxTrash)
        {
            elapsedTime = 0;
            return;
        }
        elapsedTime += Time.deltaTime;
        if (elapsedTime > collectionTime)
        {
            MineTrash();
            elapsedTime -= collectionTime;
        }
    }

    private void MineTrash()
    {
        List<MeshFilter> meshFilters = planet.GetMeshFiltersInRadius(transform.position, radius);
        Debug.Log(meshFilters.Count);

        foreach (MeshFilter meshFilter in meshFilters)
        {
            bool success = VertexManipulator.AutoMine(meshFilter, planet, transform.position, radius);
            if (success)
            {
                collectedTrash += 1;
                break;
            }
        }

    }

    private void collectTrash()
    {
        interactingPlayer.trashQty += collectedTrash;
        collectedTrash = 0;
    }
}
