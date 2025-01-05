using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 6f;                              // Player's movement speed
    public float jumpForce = 6f;                              // Force applied when jumping
    public float wallJumpForce = 7f;                          // Force applied when wall jumping
    public float dashSpeed = 10f;                             // Speed of dash
    public float dashDuration = 0.2f;                         // Duration of the dash
    public float dashCooldown = 1f;                           // Duration of dash cooldown
    public LayerMask groundLayer;                             // Layer mask to check for ground collisions
    public Transform groundCheck;                             // Reference point to check if the player is grounded
    public float groundCheckRadius = 0.3f;                    // Radius for checking if grounded
    public Vector2 wallCheckSize = new Vector2(0.1f, 1.1f);   // Size of the wall detection box
    public Vector2 wallCheckOffset = new Vector2(0f, -0.21f); // Offset for wall detection box
    public float wallCheckDistance = 0.38f;                   // Distance to check for a wall
    public float jumpCooldown = 0.2f;                         // Time cooldown between jumps
    private float lastJumpTime = -1f;                         // Timer to track the last jump time

    private bool isDashing;                                   // Flag to check if the player is currently dashing
    private bool canDash = true;                              // Flag to control whether the player can dash again
    private bool isGrounded;                                  // Flag to check if the player is on the ground
    private bool isTouchingWall;                              // Flag to check if the player is touching a wall
    private bool isMoving;
    
    private SpriteRenderer spriteRenderer;               
    private Rigidbody2D rb;     
    private Animator animator;                               


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();                
        spriteRenderer = GetComponent<SpriteRenderer>();  
        animator = GetComponent<Animator>();           
    }

    void Update()
    {
        // Skip update if dashing
        if (isDashing) return;

        // Check if on ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Check if touching wall
        isTouchingWall = CheckWallCollision();

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

        // Get the horizontal input for movement
        float moveInput = Input.GetAxisRaw("Horizontal");

        // Update movement based on input
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Update animations based on movement input
        UpdateAnimations(moveInput);

        // Handle jumping logic (hold space to bhop)
        if (Input.GetKey(KeyCode.Space) && Time.time >= lastJumpTime + jumpCooldown)
        {
            if (isGrounded) Jump();              // If grounded, perform a jump
            else if (isTouchingWall) WallJump(); // If touching a wall, perform a wall jump
        }

        // Handle dash logic
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash) StartCoroutine(Dash(moveInput));
        
        isMoving = IsPlayerMoving();

        if (!isMoving)
        {
            yield return new WaitForSeconds(2f);
            
        }
    }

    bool IsPlayerMoving()
    {
        return rb.velocity != Vector2.zero;
    }

    // Jump logic
    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        lastJumpTime = Time.time;
    }

    // Wall jump logic
    void WallJump()
    {
 
        float wallDirection = spriteRenderer.flipX ? 1 : -1; 
        rb.linearVelocity = new Vector2(wallDirection * wallJumpForce, jumpForce); 
        lastJumpTime = Time.time;                                         
    }

    // Dash logic
    IEnumerator Dash(float moveInput)
    {
        isDashing = true;
        animator.SetBool("isDashing", true);
        canDash = false;

        // Calculate the dash direction based on the input
        Vector2 dashDirection = new Vector2(moveInput != 0 ? moveInput : 
            (spriteRenderer.flipX ? -1 : 1), 0).normalized;  // Calculate the dash direction based on the input
        rb.linearVelocity = dashDirection * dashSpeed;  

        yield return new WaitForSeconds(dashDuration);       // Wait for the dash duration

        isDashing = false;
        animator.SetBool("isDashing", false);

        yield return new WaitForSeconds(dashCooldown);       // Wait for dash cooldown
        canDash = true;  
    }

    bool CheckWallCollision()
    {
        // Check if the player is colliding with a wall using box casts
        Vector2 origin = (Vector2)transform.position + wallCheckOffset;
        return Physics2D.BoxCast(origin, wallCheckSize, 0f, Vector2.right, wallCheckDistance, groundLayer) ||
               Physics2D.BoxCast(origin, wallCheckSize, 0f, Vector2.left, wallCheckDistance, groundLayer);
    }

    void UpdateAnimations(float moveInput)
    {
        if (animator != null)
        {
            if (!isTouchingWall)
            {
                animator.SetBool("isRunning", Mathf.Abs(moveInput) > 0.1f);
                animator.SetBool("isIdle", Mathf.Abs(moveInput) <= 0.1f);   
            }

            // Flip the sprite based on movement direction
            if (moveInput < 0) spriteRenderer.flipX = true;
            else if (moveInput > 0) spriteRenderer.flipX = false;

            // Jumping animation
            if (!isGrounded && !isTouchingWall && rb.linearVelocity.y > 0)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
            }
            // Falling animation
            else if (!isGrounded && rb.linearVelocity.y < 0)
            {
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", true);
            }
            else if (isGrounded) // Landing
            {
                if (animator.GetBool("isFalling")) // Trigger landing if falling was active
                {
                    animator.SetTrigger("LandTrigger");
                }
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Debug ground check radius in the editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Debug wall check area in the editor
        Gizmos.color = Color.red;
        Vector2 origin = (Vector2)transform.position + wallCheckOffset;
        Gizmos.DrawWireCube(origin + Vector2.right * wallCheckDistance, wallCheckSize);
        Gizmos.DrawWireCube(origin + Vector2.left * wallCheckDistance, wallCheckSize);
    }
}
