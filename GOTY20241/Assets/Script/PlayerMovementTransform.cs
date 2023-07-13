using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTransform : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private Transform groundCheck;
    private float groundCheckRadius = 0.2f;
    private Vector2 velocity;
    private bool isJumping;

    private void Awake()
    {
        groundCheck = transform.Find("GroundCheck");
    }

    private void Update()
    {
        // Check if the player is grounded
        isGrounded = CheckGrounded();

        // Move horizontally
        float moveDirection = Input.GetAxis("Horizontal");
        velocity.x = moveDirection * moveSpeed;

        // Jump
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            isJumping = true;
            velocity.y = jumpForce;
        }

        // Apply gravity
        velocity.y += Physics2D.gravity.y/100 * Time.deltaTime;

        // Move the player
        Vector3 deltaPosition = velocity * Time.deltaTime;
        transform.Translate(deltaPosition);

        // Reset jumping flag
        if (isGrounded)
        {
            isJumping = false;
        }
    }

    private bool CheckGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }
}
