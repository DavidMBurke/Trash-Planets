using UnityEngine;

public class Building : MonoBehaviour
{
    public Planet planet; // Reference to the planet
    public float groundCheckDistance = 1f; // Distance to check for ground
    private Vector3 gravityDirection;
    private Vector3 velocity;
    public float gravityMultiplier = 1.0f; // Adjust to control fall speed
    public bool isGrounded;

    void Start()
    {
        if (planet == null)
        {
            Debug.LogError("Planet reference is missing!");
            return;
        }

        AlignToPlanet(); // Initial alignment
    }

    private void Update()
    {
        Debug.DrawRay(transform.position, -gravityDirection * groundCheckDistance, Color.red);
        ApplyGravity();
        AlignToPlanet();
        MoveBuilding();
    }

    private void ApplyGravity()
    {
        if (!isGrounded && planet != null)
        {
            gravityDirection = (planet.transform.position - transform.position).normalized;
            velocity += gravityDirection * planet.gravity * gravityMultiplier * Time.deltaTime;
        }
        else
        {
            // Ensure the velocity is zero if grounded
            velocity = Vector3.zero;
        }
    }

    private void AlignToPlanet()
    {
        // Calculate gravity direction
        gravityDirection = (planet.transform.position - transform.position).normalized;

        // Align the building's up direction to be opposite to gravity
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;

        // Smoothly rotate the building to align with the planet
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
    }

    private void MoveBuilding()
    {
        // Move the building based on velocity
        transform.position += velocity * Time.deltaTime;

        // Check if the building is grounded
        isGrounded = CheckIfGrounded();

        if (isGrounded)
        {
            SnapToSurface();
        }
    }

    private bool CheckIfGrounded()
    {
        // Perform a raycast in the direction opposite to gravity
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, groundCheckDistance))
        {
            if (hit.collider.CompareTag("Planet"))
            {
                return true;
            }
        }
        return false;
    }

    private void SnapToSurface()
    {
        // Perform a raycast within a reasonable range
        if (Physics.Raycast(transform.position, -gravityDirection, out RaycastHit hit, groundCheckDistance + 0.5f))
        {
            if (hit.collider.CompareTag("Planet"))
            {
                // Place the building flush with the surface
                transform.position = hit.point + hit.normal * 0.01f; // Offset slightly to avoid collision issues

                // Reset velocity when grounded
                velocity = Vector3.zero;
            }
        }
    }
}
