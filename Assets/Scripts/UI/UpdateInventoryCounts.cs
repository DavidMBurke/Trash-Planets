using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateInventoryCounts : MonoBehaviour
{
    private TextMeshProUGUI InventoryText;
    public GameObject PlayerObject;
    private Player playerScript;

    // Start is called before the first frame update
    void Start()
    {
        InventoryText = GetComponent<TextMeshProUGUI>();
        if (InventoryText == null)
        {
            Debug.LogError("InventoryCounts object is missing a TextMeshProUGUI component.");
            return;
        }

        playerScript = PlayerObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        int trash = playerScript.trashQty;
        int building_mats = playerScript.building_mat_qty;

        InventoryText.text = $"Trash:\t\t\t{trash}\nBuilding Materials:\t{building_mats}";
        
    }
}
