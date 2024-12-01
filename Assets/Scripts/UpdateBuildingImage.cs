using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateBuildingImage : MonoBehaviour
{
    private RawImage displayImage;
    public GameObject Camera;
    private FirstPersonCamera cameraScript;
    public Texture miningIcon;
    public Texture cannonIcon;
    public Texture drillIcon;
    public Texture refineryIcon;
    public Texture transporterIcon;
    public Texture weaponIcon;

    // Start is called before the first frame update
    void Start()
    {
        // Retirve components and set the default icon
        displayImage = GetComponent<RawImage>();
        displayImage.texture = miningIcon;

        cameraScript = Camera.GetComponent<FirstPersonCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject building = cameraScript.selectedPrefab;
        // If building is null, pass an empty string into this variable
        string iconName = building != null ? building.tag : "";

        // This sets the icon based on what the given string is
        switch (iconName)
        {
            case "Cannon":
                displayImage.texture = cannonIcon;
                break;
            case "Drill":
                displayImage.texture = drillIcon;
                break;
            case "Refinery":
                displayImage.texture = refineryIcon;
                break;
            case "Transporter":
                displayImage.texture = transporterIcon;
                break;
            case "Weapon":
                displayImage.texture = weaponIcon;
                break;
            default:
                displayImage.texture = miningIcon;
                break;
        }
        
    }
}
