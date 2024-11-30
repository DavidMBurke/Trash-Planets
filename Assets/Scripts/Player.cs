using UnityEngine;

public class Player : MonoBehaviour
{
    public int minedTrash = 0;
    public int trashQty = 0;
    public int building_mat_qty = 0;

    private void Update()
    {
        minedTrashToQty();
    }

    private void minedTrashToQty()
    {
        if (minedTrash >=  GameSettings.minedTrashRatio)
        {
            minedTrash -= GameSettings.minedTrashRatio;
            trashQty += 1;
        }
    }
}
