using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Player movement variables
    public Rigidbody2D rb;
    
    //adjustable speed variable
    public float moveSpeed = 5f;
    //adjustable jump force variable
    public float jumpForce = 10f;
    //public Transform component for player ground check
    public Transform groundCheck;
    //determines what counts as ground, as well as what the
    //player can jump off of
    public LayerMask groundLayer;
    //horizontal movement variable / left and right
    float horizontalMovement;
    //jumping/not jumping
    bool isGrounded;
    float groundRadius = 0.2f; // Updated ground radius
    // Animator reference
    [SerializeField] private Animator _animator;
    //Tracks which way the player is facing
    bool facingRight = true;
    // Attack hitbox reference
    public GameObject attackHitbox;

    public float attackCooldown = 0.5f; // Time in seconds between attacks
    private float lastAttackTime = 0f; // Time since the last attack was performed

    public float attackMoveMult = 0.5f;

    void Update()
    {
    // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);

        // Determine current movement speed
        float currentMoveSpeed = moveSpeed;

        // Reduce speed if attacking while grounded
        if (_animator.GetBool("isAttacking") && isGrounded)
        {
            currentMoveSpeed *= attackMoveMult;
        }

        // Smoothly set horizontal velocity
        float targetSpeed = horizontalMovement * currentMoveSpeed;
        rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetSpeed, 0.2f), rb.linearVelocity.y);

        // Set animator parameters for blend trees
        _animator.SetFloat("xVelocity", Mathf.Abs(rb.linearVelocity.x));
        _animator.SetFloat("yVelocity", rb.linearVelocity.y);
        _animator.SetBool("isJumping", !isGrounded);
        _animator.SetBool("isRunning", Mathf.Abs(horizontalMovement) > 0.01f);


        // Cancel attack if airborne
        if (_animator.GetBool("isAttacking") && !isGrounded)
        {
            _animator.SetBool("isAttacking", false);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
            //running animation
            horizontalMovement = context.ReadValue<Vector2>().x;

            if (horizontalMovement != 0)
            {
                _animator.SetBool("isRunning", true);
            }
            else
            {
                _animator.SetBool("isRunning", false);
            }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //debug log to check if jump is being called, could probably delete
        Debug.Log($"Jump called. isGrounded: {isGrounded}, performed: {context.performed}");
        if (context.performed && isGrounded)
        {
            //jumping force
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            if (Time.time >= lastAttackTime + attackCooldown && !_animator.GetBool("isAttacking"))
            {
                //attack animation, resets cooldown timer
                _animator.SetBool("isAttacking", true);
                lastAttackTime = Time.time; // reset cooldown timer
                Debug.Log("Attack performed");
            }
        }
    }

    public void endAttack()
    {
        //animation event to end attack animation, turns off hitbox
        _animator.SetBool("isAttacking", false);
    }

    void FixedUpdate()
    {
        //simple script to flip player based on direction
        if (horizontalMovement > 0 && !facingRight)
        {
            Flip();
        }
        else if (horizontalMovement < 0 && facingRight)
        {
            Flip();
        }
    }
    void Flip()
    {
        //flip method, flips player sprite
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        facingRight = !facingRight;
    }

    public void enableHitbox()
    {
        //enables player attack hitbox
        attackHitbox.GetComponent<Collider2D>().enabled = true;
    }

    public void disableHitbox()
    {
        //disables player attack hitbox
        attackHitbox.GetComponent<Collider2D>().enabled = false;
    }
}