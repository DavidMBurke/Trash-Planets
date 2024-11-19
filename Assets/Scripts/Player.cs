using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Planet planet;
    public float speed = 6.0f;
    public float jumpForce = 500.0f;
    private CharacterController controller;
    private Vector3 moveDirection;
    private Vector3 gravityDirection;
    public bool isGrounded;
    public float groundCheckDistance = 1f;
    public float rocketCooldown = 1f;
    public float rocketMax = 1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        moveDirection = (transform.forward * vertical + transform.right * horizontal).normalized * speed;

        isGrounded = CheckIfGrounded();

        if (Input.GetButton("Jump") && rocketCooldown <= rocketMax && rocketCooldown >0)
        {
            moveDirection += -gravityDirection * planet.gravity * jumpForce * Time.deltaTime;
            rocketCooldown -= Time.deltaTime;
        }
        if (rocketCooldown < rocketMax && !Input.GetButton("Jump")) {
            rocketCooldown = Math.Min(rocketCooldown + Time.deltaTime, rocketMax);
        }


        if (!isGrounded && planet != null)
        {
            gravityDirection = (planet.transform.position - transform.position).normalized;
            moveDirection += gravityDirection * planet.gravity * Time.deltaTime;

            Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }

        controller.Move(moveDirection * Time.deltaTime);
    }

    private bool CheckIfGrounded()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, groundCheckDistance))
        {
            if (hit.collider.CompareTag("Planet"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Set isGrounded to true if touching the planet
        if (hit.collider.CompareTag("Planet"))
        {
            isGrounded = true;
        }
    }

}
