using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateBuildingDisplay : MonoBehaviour
{
    private TextMeshProUGUI BuildingText;
    public GameObject Camera;
    private FirstPersonCamera cameraScript;

    // Start is called before the first frame update
    void Start()
    {
        BuildingText = GetComponent<TextMeshProUGUI>();
        cameraScript = Camera.GetComponent<FirstPersonCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject building = cameraScript.selectedPrefab;

        if (building != null) {
            BuildingText.text = $"Placing building: {building.tag}";
        }
        else {
            BuildingText.text = $"Mining";
        }
    }
}
