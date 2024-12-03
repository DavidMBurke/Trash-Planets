using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class WeaponScript : Building
{
    [SerializeField]
    private GameObject cameraPrefab;
    [SerializeField]
    protected Transform cameraAnchor;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    protected Transform originPlanet;
    [SerializeField]
    protected Transform targetPlanet;


    //Internal
    protected bool playerActive = false;
    protected GameObject currentCamera;
    private PlayerInput playerControls;
    private GameObject currentPlayer;

    protected InputAction Move => FindAction("Move");

    protected InputAction Fire => FindAction("Fire");

    protected InputAction Exit => FindAction("Exit");

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
        if (this.Exit.IsPressed())
        {
            endInteract();
        }
    }

    public virtual void startInteract(GameObject player)
    {
        currentPlayer = player;
        currentPlayer.GetComponent<PlayerMovement>().movementEnabled = false;
        currentPlayer.GetComponent<PlayerInteraction>().movementEnabled = false;
        currentPlayer.transform.Find("Virtual Camera").gameObject.SetActive(false);
        this.playerControls = player.GetComponent<PlayerInput>();
        createCamera();

        playerActive = true;
    }

    public virtual void endInteract()
    {
        currentPlayer.GetComponent<PlayerMovement>().movementEnabled = true;
        currentPlayer.GetComponent<PlayerInteraction>().movementEnabled = true;
        currentPlayer.transform.Find("Virtual Camera").gameObject.SetActive(true);
        this.playerControls = null;
        destroyCamera();


        playerActive = false;
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
