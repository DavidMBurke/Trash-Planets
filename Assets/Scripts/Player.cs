using System.Security.Cryptography;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerId;
    public int playerScore = 0;
    public int minedTrash = 0;
    public int trashQty = 0;
    public int building_mat_qty = 0;

    private void Update()
    {
        minedTrashToQty();
        updateInventory();
    }

    private void minedTrashToQty()
    {
        if (minedTrash >=  GameSettings.minedTrashRatio)
        {
            minedTrash -= GameSettings.minedTrashRatio;
            trashQty += 1;
            playerScore += 1;
        }
    }

    private void updateInventory()
    {
        GameObject.Find($"P{playerId} Trash Amount").GetComponent<TextMeshProUGUI>().text = $"{trashQty}";
        GameObject.Find($"P{playerId} Refined Amount").GetComponent<TextMeshProUGUI>().text = $"{building_mat_qty}";
    }
}
