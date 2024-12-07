using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Refinery : Building
{
    public BuildingButton button1;
    public BuildingButton button10;
    public BuildingButton button100;
    public BuildingButton buttonCollect;
    public TextMeshProUGUI trashQtyDisplay;
    public TextMeshProUGUI buildingMatQtyDisplay;
    public int maxTrashQty = 100;
    public int maxBuildingMatQty = 100;
    public int trashQty;
    public int buildingMatQty;
    public float processTime = 5;
    public float processTimer = 0;

    public AudioSource refinerysound;

    public void Start()
    {
        AssignButtonFunctions();
    }

    public void Update()
    {
        UpdateText();
        ProcessTrash();
    }

    public void AddTrash(int num)
    {
        if (num + trashQty > maxTrashQty)
        {
            num = maxTrashQty - trashQty;
        }
        if (interactingPlayer.trashQty < num)
        {
            trashQty = interactingPlayer.trashQty;
            interactingPlayer.trashQty = 0;
            return;
        }
        trashQty += num;
        interactingPlayer.trashQty -= num;
    }
    private void CollectMats()
    {
        interactingPlayer.building_mat_qty += buildingMatQty;
        buildingMatQty = 0;
    }

    private void UpdateText()
    {
        trashQtyDisplay.text = trashQty.ToString() + "/" + maxTrashQty.ToString();
        buildingMatQtyDisplay.text = buildingMatQty.ToString() + "/" + maxBuildingMatQty.ToString();
    }

    private void AssignButtonFunctions()
    {
        button1.function = () => AddTrash(1);
        button10.function = () => AddTrash(10);
        button100.function = () => AddTrash(100);
        buttonCollect.function = () => CollectMats();
    }

    private void ProcessTrash()
    {
        if (trashQty == 0){
            if (refinerysound.isPlaying)
                refinerysound.Pause();
            return;
        }
        if (buildingMatQty == maxBuildingMatQty)
            return;
        processTimer += Time.deltaTime;
        if (processTimer > processTime) {
            processTimer -= processTime;
            trashQty -= 1;
            buildingMatQty += 1;
            if (!refinerysound.isPlaying)
                refinerysound.Play();
        }
    }

}
