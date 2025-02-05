using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the player's movement, jumping, wall jumping, dashing, and animations.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Player's movement speed.")]
    public float moveSpeed = 6f;
    
    [Tooltip("Force applied when jumping.")]
    public float jumpForce = 6f;
    
    [Tooltip("Force applied when wall jumping.")]
    public float wallJumpForce = 7f;
    
    [Header("Dash Settings")]
    [Tooltip("Speed of dash.")]
    public float dashSpeed = 10f;
    
    [Tooltip("Duration of the dash.")]
    public float dashDuration = 0.2f;
    
    [Tooltip("Cooldown time between dashes.")]
    public float dashCooldown = 1f;

    [Header("Ground & Wall Detection")]
    [Tooltip("Layer mask to check for ground collisions.")]
    public LayerMask groundLayer;
    
    [Tooltip("Reference point to check if the player is grounded.")]
    public Transform groundCheck;
    
    [Tooltip("Radius for checking if the player is grounded.")]
    public float groundCheckRadius = 0.3f;
    
    [Tooltip("Size of the wall detection box.")]
    public Vector2 wallCheckSize = new Vector2(0.1f, 1.1f);
    
    [Tooltip("Offset for wall detection box.")]
    public Vector2 wallCheckOffset = new Vector2(0f, -0.21f);
    
    [Tooltip("Distance to check for a wall.")]
    public float wallCheckDistance = 0.38f;

    [Header("Jump Settings")]
    [Tooltip("Time cooldown between jumps.")]
    public float jumpCooldown = 0.2f;

    private float lastJumpTime = -1f; // Tracks the last jump time

    private bool isDashing;       // Whether the player is currently dashing
    private bool canDash = true;  // Whether the player can dash
    private bool isGrounded;      // Whether the player is on the ground
    private bool isTouchingWall;  // Whether the player is touching a wall
    private bool isMoving;        // Whether the player is moving

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    /// <summary>
    /// Initializes the player's components.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Handles player movement, jumping, and dashing.
    /// </summary>
    private void Update()
    {
        if (isDashing) return; // Skip update logic if dashing

        // Check ground and wall collision
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isTouchingWall = CheckWallCollision();

        HandleWallAnimation();
        
        float moveInput = Input.GetAxisRaw("Horizontal"); // Get movement input
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Apply movement
        UpdateAnimations(moveInput); // Update animations

        // Handle jumping logic
        if (Input.GetKey(KeyCode.Space) && Time.time >= lastJumpTime + jumpCooldown)
        {
            if (isGrounded) Jump();
            else if (isTouchingWall) WallJump();
        }

        // Handle dashing logic
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash(moveInput));
        }

        isMoving = IsPlayerMoving();
    }

    /// <summary>
    /// Checks if the player is currently moving.
    /// </summary>
    private bool IsPlayerMoving()
    {
        return rb.velocity != Vector2.zero;
    }

    /// <summary>
    /// Handles normal jumping.
    /// </summary>
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        lastJumpTime = Time.time;
    }

    /// <summary>
    /// Handles wall jumping.
    /// </summary>
    private void WallJump()
    {
        float wallDirection = spriteRenderer.flipX ? 1 : -1;
        rb.velocity = new Vector2(wallDirection * wallJumpForce, jumpForce);
        lastJumpTime = Time.time;
    }

    /// <summary>
    /// Performs a dash movement.
    /// </summary>
    /// <param name="moveInput">The horizontal input direction.</param>
    private IEnumerator Dash(float moveInput)
    {
        isDashing = true;
        animator.SetBool("isDashing", true);
        canDash = false;

        Vector2 dashDirection = new Vector2(moveInput != 0 ? moveInput : (spriteRenderer.flipX ? -1 : 1), 0).normalized;
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        animator.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    /// <summary>
    /// Checks if the player is colliding with a wall.
    /// </summary>
    private bool CheckWallCollision()
    {
        Vector2 origin = (Vector2)transform.position + wallCheckOffset;
        return Physics2D.BoxCast(origin, wallCheckSize, 0f, Vector2.right, wallCheckDistance, groundLayer) ||
               Physics2D.BoxCast(origin, wallCheckSize, 0f, Vector2.left, wallCheckDistance, groundLayer);
    }

    /// <summary>
    /// Updates animations based on player movement.
    /// </summary>
    /// <param name="moveInput">The horizontal movement input.</param>
    private void UpdateAnimations(float moveInput)
    {
        if (animator != null)
        {
            if (!isTouchingWall)
            {
                animator.SetBool("isRunning", Mathf.Abs(moveInput) > 0.1f);
                animator.SetBool("isIdle", Mathf.Abs(moveInput) <= 0.1f);
            }

            // Flip sprite based on movement direction
            if (moveInput < 0) spriteRenderer.flipX = true;
            else if (moveInput > 0) spriteRenderer.flipX = false;

            // Jumping animation
            if (!isGrounded && !isTouchingWall && rb.velocity.y > 0)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
            }
            // Falling animation
            else if (!isGrounded && rb.velocity.y < 0)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
            else if (isGrounded) // Landing
            {
                if (animator.GetBool("isFalling"))
                {
                    animator.SetTrigger("LandTrigger");
                }
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
            }
        }
    }

    /// <summary>
    /// Handles animation states when the player is touching a wall.
    /// </summary>
    private void HandleWallAnimation()
    {
        if (isTouchingWall)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isDashing", false);
            animator.SetBool("isTouchingWall", true);
        }
        else
        {
            animator.SetBool("isTouchingWall", false);
        }
    }

    /// <summary>
    /// Draws gizmos in the editor to visualize ground and wall detection.
    /// </summary>
    private void OnDrawGizmos()
    {
        // Ground check visualization
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Wall check visualization
        Gizmos.color = Color.red;
        Vector2 origin = (Vector2)transform.position + wallCheckOffset;
        Gizmos.DrawWireCube(origin + Vector2.right * wallCheckDistance, wallCheckSize);
        Gizmos.DrawWireCube(origin + Vector2.left * wallCheckDistance, wallCheckSize);
    }
}
