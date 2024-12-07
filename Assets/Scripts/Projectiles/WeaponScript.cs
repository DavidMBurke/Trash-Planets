using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class WeaponScript : Building
{
    [Header("Weapon Settings")]
    [SerializeField]
    protected Transform cameraAnchor;
    [SerializeField]
    private float rotationSpeed;
    //[SerializeField]
    //protected Transform originPlanet;
    //[SerializeField]
    //protected Transform targetPlanet;


    //Internal
    protected bool playerActive = false;
    protected GameObject currentCamera;
    private PlayerInput playerControls;
    protected GameObject currentPlayer;
    private GameObject cameraPrefab;
    private GameObject playerOutputCamera;

    private float interactCooldown;

    protected InputAction Move => FindAction("Move");

    protected InputAction Fire => FindAction("Fire");

    protected InputAction Exit => FindAction("Exit");

    protected InputAction Target => FindAction("Target");

    protected InputAction FindAction(string actionName)
    {
        return this.playerControls.currentActionMap?.FindAction(actionName);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void weaponRotation()
    {
        Vector2 rawInput = this.Move.ReadValue<Vector2>();
        transform.Rotate(transform.up * rotationSpeed * rawInput.x, Space.World);
    }

    protected void checkExit()
    {
        if (this.Exit.IsPressed() && Time.time > interactCooldown)
        {
            endInteract();
        }
    }

    public virtual void startInteract(GameObject player)
    {
        if (Time.time <= interactCooldown)
        {
            return;
        }

        currentPlayer = player;

        //Disable other actions
        currentPlayer.GetComponent<PlayerMovement>().movementEnabled = false;

        PlayerInteraction currentPlayerInteraction = currentPlayer.GetComponent<PlayerInteraction>();
        currentPlayerInteraction.movementEnabled = false;
        this.cameraPrefab = currentPlayerInteraction.weaponCameraPrefab;
        this.playerOutputCamera = currentPlayerInteraction.playerCamera.gameObject;
        this.playerOutputCamera.GetComponent<FirstPersonCamera>().controlEnabled = false;

        currentPlayer.transform.Find("Virtual Camera").gameObject.SetActive(false);
        this.playerControls = player.GetComponent<PlayerInput>();
        createCamera();

        interactCooldown = Time.time + 2;
        playerActive = true;
    }

    public virtual void endInteract()
    {
        currentPlayer.GetComponent<PlayerMovement>().movementEnabled = true;

        PlayerInteraction currentPlayerInteraction = currentPlayer.GetComponent<PlayerInteraction>();
        currentPlayerInteraction.movementEnabled = true;
        this.cameraPrefab = null;
        this.playerOutputCamera.GetComponent<FirstPersonCamera>().controlEnabled = true;
        this.playerOutputCamera = null;

        currentPlayer.transform.Find("Virtual Camera").gameObject.SetActive(true);
        this.playerControls = null;
        destroyCamera();

        interactCooldown = Time.time + 2;
        playerActive = false;

        currentPlayer = null;
    }

    void createCamera()
    {
        this.currentCamera = Instantiate(cameraPrefab, cameraAnchor.position, cameraAnchor.rotation);
        this.currentCamera.transform.SetParent(cameraAnchor);

        CinemachineVirtualCamera cineCamera = this.currentCamera.GetComponent<CinemachineVirtualCamera>();
        cineCamera.Follow = this.cameraAnchor;
    }

    void destroyCamera()
    {
        Destroy(this.currentCamera);
    }
}
