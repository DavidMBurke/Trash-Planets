using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    //Controls
    //[Header("Player Controls")]
    //[SerializeField]
    //private PlayerInput playerControls;

    [Header("Player Components")]
    [SerializeField]
    public Transform cameraTransform;
    [SerializeField]
    private Transform orientation;
    [SerializeField]
    private Transform model;

    //Movement
    [Header("Player Movement Settings")]
    [SerializeField]
    private Transform gravityPoint;
    [SerializeField]
    private float gravityForce;
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float slowDownThreshold;
    [SerializeField]
    private float slowDownFraction;

    [Header("Jump Settings")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public float jumpForce;
    public float downForce;

    [Header("Interaction Settings")]
    public bool movementEnabled = true;


    //Audio
    public AudioSource playerFootsteps;  // Reference to the player's AudioSource

    // public AudioSource playerMining;
    public AudioSource playerJumping;
    private bool was_moving83 = false;

    // private bool was_jumping = false;

    //Internal
    private Transform playerTransform;
    private Rigidbody playerRigidbody;
    private PlayerInput playerControls;

    //Player Input
    protected InputAction Move => FindAction("Move");

    protected InputAction Jump => FindAction("Jump");

    protected InputAction FindAction(string actionName)
    {
        return this.playerControls.currentActionMap?.FindAction(actionName);
    }

    // Start is called before the first frame update
    void Start()
    {
        this.playerTransform = this.gameObject.transform;
        this.playerRigidbody = this.gameObject.GetComponent<Rigidbody>();
        this.playerControls = this.gameObject.GetComponent<PlayerInput>();
        // this.playerFootsteps = this.gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        orientGravity();
        updateOrientation();

        if (movementEnabled)
        {
            movePlayer();
            jumpPlayer();
        }

        applyGravity();
        playerDrag();

    }



    void movePlayer()
    {
        Vector2 rawInput = this.Move.ReadValue<Vector2>();
        Vector3 playerMoveInput = orientation.forward * rawInput.y + orientation.right * rawInput.x;
        playerRigidbody.AddForce(playerMoveInput * acceleration, ForceMode.Acceleration);

        var test_velocity = playerRigidbody.velocity;
        test_velocity.y = 0;

        bool is_moving23 = test_velocity.magnitude > maxSpeed / 2;
        if (is_moving23 != was_moving83 && playerFootsteps != null) {
            if (is_moving23) {
                playerFootsteps.Play();
            } else {
                playerFootsteps.Pause();
            }
        }
        was_moving83 = is_moving23;

        if (playerRigidbody.velocity.magnitude > maxSpeed)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxSpeed;
        }

    }

    void jumpPlayer()
    {
        bool grounded = Physics.Raycast(transform.position, -orientation.up, playerHeight * 0.5f + 1f, whatIsGround);

        if (grounded && this.Jump.IsPressed())
        {
            playerRigidbody.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
            if (playerJumping != null)
            {
                playerJumping.Play();
            }
        }

        if (!grounded && !this.Jump.IsPressed())
        {
            playerRigidbody.AddForce(-orientation.up * downForce, ForceMode.Force);
        }
    }

    void playerDrag()
    {
        Vector2 rawInput = this.Move.ReadValue<Vector2>();

        if (rawInput.magnitude < slowDownThreshold)
        {
            float playerForwardVelocity = Vector3.Dot(playerRigidbody.velocity, orientation.forward);
            float playerRightVelocity = Vector3.Dot(playerRigidbody.velocity, orientation.right);
            playerRigidbody.velocity = playerRigidbody.velocity - slowDownFraction * (orientation.forward * playerForwardVelocity + orientation.right * playerRightVelocity);
        }
    }

    //This function sets the player upright with respect to the gravity object defined above
    void orientGravity()
    {
        //This code was causing some weird effects, likely due to ambiguity with the up rotation
        //Vector3 directionGravity = this.playerTransform.position - gravityPoint.position;
        //this.playerTransform.up = directionGravity.normalized;

        // Rotate the player to align with the gravity direction (so they stay upright)
        //float rotationSpeed = 2f;
        Vector3 gravityDirection = (gravityPoint.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravityDirection) * this.playerTransform.rotation;
        //this.playerTransform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        this.playerTransform.rotation = targetRotation;
    }

    void applyGravity()
    {
        Vector3 gravityDirection = (this.playerTransform.position - gravityPoint.position).normalized;
        playerRigidbody.AddForce(gravityDirection * -1 * gravityForce, ForceMode.Acceleration);
    }

    Quaternion calculateVectorDifferenceRotation(Vector3 vec1, Vector3 vec2)
    {
        float angle = Vector3.Angle(vec1, vec2);
        Vector3 rotationAxis = Vector3.Cross(vec1, vec2);
        Quaternion rotation = Quaternion.AngleAxis(angle, rotationAxis);

        return rotation;
    }

    void updateOrientation()
    {
        // Normalize the input vectors to ensure they are unit vectors
        Vector3 gravityDirection = (this.playerTransform.position - gravityPoint.position).normalized;
        Vector3 camRightDirection = this.cameraTransform.right;

        // Calculate the Z-axis (perpendicular to both A and B)
        Vector3 forwardDirection = - Vector3.Cross(gravityDirection, camRightDirection).normalized;

        // Create the rotation quaternion: Look at vectorC (Z-axis) with the "up" direction being vectorA (Y-axis)
        Quaternion targetRotation = Quaternion.LookRotation(forwardDirection, gravityDirection);

        // Apply the rotation to the object's transform
        this.model.rotation = targetRotation;
    }
}
