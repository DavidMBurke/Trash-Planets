using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Refinery : Building
{
    public BuildingButton button1;
    public BuildingButton button10;
    public BuildingButton button100;
    public BuildingButton buttonCollect;
    public int trashQty;
    public int buildingMatQty;

    public void Start()
    {
        button1.function = () => AddTrash(1);
        button10.function = () => AddTrash(10);
        button100.function = () => AddTrash(100);
    }

    public void Update()
    {
        
    }

    public void AddTrash(int num)
    {
        if (interactingPlayer.trashQty < num)
        {
            trashQty = interactingPlayer.trashQty;
            interactingPlayer.trashQty = 0;
            return;
        }
        trashQty += num;
        interactingPlayer.trashQty -= num;

    }

}
