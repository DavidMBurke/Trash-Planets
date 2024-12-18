using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject weaponCameraPrefab;
    [SerializeField]
    private float interactDistance;
    [SerializeField]
    private LayerMask interactables;

    [Header("Interaction Settings")]
    public bool movementEnabled = true;

    //Internal
    private PlayerInput playerControls;

    protected InputAction Interact => FindAction("Interact");

    protected InputAction FindAction(string actionName)
    {
        return this.playerControls.currentActionMap?.FindAction(actionName);
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        this.playerControls = this.gameObject.GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (movementEnabled)
        {
            checkInteract();
        }

    }

    void checkInteract()
    {
        if (this.Interact.IsPressed())
        {
            RaycastHit hit;

            // Create a ray from the camera's position in the forward direction of the camera
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            // Perform the raycast and check if we hit something
            if (Physics.Raycast(ray, out hit, interactDistance, interactables))
            {
                if (hit.collider.gameObject.tag == "Weapon")
                {
                    WeaponScript weapon = hit.collider.gameObject.GetComponent<WeaponScript>();
                    weapon.startInteract(this.gameObject);
                }
            }
            else
            {

            }
        }

    }
}
