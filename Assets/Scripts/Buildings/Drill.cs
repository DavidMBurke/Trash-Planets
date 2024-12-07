using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : Building
{
    public BuildingButton[] CollectionButtons;
    public int maxTrash = 100;
    public int minedTrash = 0;
    public int storedTrash = 0;
    public float collectionTime = 5;
    public float elapsedTime = 0;
    public float radius = 10;
    public AudioSource drillsound;

    private void Start()
    {
        AddFunctions();
    }

    private void Update()
    {
        TryMineTrash();
        while (minedTrash > GameSettings.minedTrashRatio)
        {
            minedTrash -= GameSettings.minedTrashRatio;
            storedTrash += 1;
        }
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
        if (storedTrash >= maxTrash)
        {
            elapsedTime = 0;
            return;
        }
        elapsedTime += Time.deltaTime;
        while (elapsedTime > collectionTime)
        {
            MineTrash();
            elapsedTime -= collectionTime;
        }
    }

    private void MineTrash()
    {
        List<MeshFilter> meshFilters = planet.GetMeshFiltersInRadius(transform.position, radius);

        foreach (MeshFilter meshFilter in meshFilters)
        {
            bool success = VertexManipulator.AutoMine(meshFilter, planet, transform.position, radius);
            if (success)
            {
                minedTrash += 1;
                if (!drillsound.isPlaying)
                    drillsound.Play();
                break;
            }
        }

    }

    private void collectTrash()
    {
        interactingPlayer.trashQty += minedTrash;
        minedTrash = 0;
    }
}
