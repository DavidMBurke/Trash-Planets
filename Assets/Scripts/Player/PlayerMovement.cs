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
    private Transform cameraTransform;
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

    //Audio
    // public AudioSource playerFootsteps;  // Reference to the player's AudioSource
    // private bool was_moving83 = false;

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
        updateOrientation();
        orientGravity();
        movePlayer();
        jumpPlayer();
        applyGravity();
    }



    void movePlayer()
    {
        Vector2 rawInput = this.Move.ReadValue<Vector2>();
        Vector3 playerMoveInput = orientation.forward * rawInput.y + orientation.right * rawInput.x;
        playerRigidbody.AddForce(playerMoveInput * acceleration, ForceMode.Acceleration);

        // bool is_moving = playerRigidbody.velocity.magnitude > maxSpeed / 2;
        // bool is_moving23 = Time.time%4>2;
        // if (is_moving23 != was_moving83) {
        //     if (is_moving23) {
        //         playerFootsteps.Play();
        //     } else {
        //         playerFootsteps.Pause();
        //     }
        // }
        // was_moving83 = is_moving23;

        if (playerRigidbody.velocity.magnitude > maxSpeed)
        {
            playerRigidbody.velocity = playerRigidbody.velocity.normalized * maxSpeed;
        }

        if (rawInput.magnitude < slowDownThreshold)
        {
            float playerForwardVelocity = Vector3.Dot(playerRigidbody.velocity, orientation.forward);
            float playerRightVelocity = Vector3.Dot(playerRigidbody.velocity, orientation.right);
            playerRigidbody.velocity = playerRigidbody.velocity - slowDownFraction * (orientation.forward * playerForwardVelocity + orientation.right * playerRightVelocity);
        }

        // // Play footsteps sound if the player is moving
        // if (rawInput.magnitude > 0f && !playerFootsteps.isPlaying)
        // {
        //     playerFootsteps.Play();
        // }
        // // Stop the footsteps sound when the player stops moving
        // else if (rawInput.magnitude == 0f && playerFootsteps.isPlaying)
        // {
        //     playerFootsteps.Stop();
        // }

    }

    void jumpPlayer()
    {
        bool grounded = Physics.Raycast(transform.position, -orientation.up, playerHeight * 0.5f + 0.4f, whatIsGround);

        if (grounded && this.Jump.IsPressed())
        {
            playerRigidbody.AddForce(orientation.up * jumpForce, ForceMode.Impulse);
        }

        if (!grounded && !this.Jump.IsPressed())
        {
            playerRigidbody.AddForce(-orientation.up * downForce, ForceMode.Force);
        }
    }

    //This function sets the player upright with respect to the gravity object defined above
    void orientGravity()
    {
        Vector3 directionGravity = this.playerTransform.position - gravityPoint.position;
        this.playerTransform.up = directionGravity.normalized;
    }

    void applyGravity()
    {
        playerRigidbody.AddForce(this.playerTransform.up * -1 * gravityForce, ForceMode.Acceleration);
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
