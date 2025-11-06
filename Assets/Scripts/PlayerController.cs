using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Player movement variables

    public UnityEngine.UI.Image healthBar;
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public Rigidbody2D rb;

    public int groundAttackDamage = 20;
    public int airAttackDamage = 30;

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
    public GameObject airAttackHitbox;

    public float attackCooldown = 0.5f; // Time in seconds between attacks
    private float lastAttackTime = 0f; // Time since the last attack was performed

    public float attackMoveMult = 0.5f;

    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

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

    void UpdateHealthBar()
    {
        //if health bar DOES exist, fill health bar by current/max (something like 70/100)
        if (healthBar != null)
        {
            healthBar.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        //Update's health bar based on damage taken, similar to heal using K
        if (isDead) return;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        //Method I added for health bar purposes (use J to heal)
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void Die()
    {
        //Starts the death animation for death, starting a 
        //method to destroy player object after death
        isDead = true;
        _animator.SetTrigger("Die");
        Invoke(nameof(DestroyAfterDeath), 1.2f);
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0) return;
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
        if (Time.timeScale == 0) return;

        Debug.Log($"Jump called. isGrounded: {isGrounded}, performed: {context.performed}");
        if (context.performed && isGrounded)
        {
            //jumping force
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            audioManager.PlaySFX(audioManager.jumpSound);
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (Time.timeScale == 0) return;

        
        if (context.performed)
        {
            if (Time.time >= lastAttackTime + attackCooldown && !_animator.GetBool("isAttacking"))
            {
                if (!isGrounded)
                {
                    _animator.SetBool("isAirAttacking", true);
                    audioManager.PlaySFX(audioManager.airAttackSound);
                }
                else
                {
                    _animator.SetBool("isAttacking", true);
                    audioManager.PlaySFX(audioManager.atkSound);
                }

                //attack animation, resets cooldown timer
                lastAttackTime = Time.time; // reset cooldown timer
                Debug.Log("Attack performed");
            }
        }
    }

    public void endAttack()
    {
        //animation event to end attack animation, turns off hitbox
        _animator.SetBool("isAttacking", false);
        Debug.Log("Attack ended");
    }

    public void endAirAttack()
    {
        //animation event to end air attack animation, turns off hitbox
        _animator.SetBool("isAirAttacking", false);
        Debug.Log("Air Attack ended");
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

    public void SetHitboxEnabled(GameObject hitbox, bool enabled)
    {
        hitbox.GetComponent<Collider2D>().enabled = enabled;
    }
    //Methods to enable/disable air/regular attack hitboxes
    public void EnableHitbox() => SetHitboxEnabled(attackHitbox, true);
    public void DisableHitbox() => SetHitboxEnabled(attackHitbox, false);
    public void EnableAirHitbox() => SetHitboxEnabled(airAttackHitbox, true);
    public void DisableAirHitbox() => SetHitboxEnabled(airAttackHitbox, false);

    void OnTriggerEnter2D(Collider2D collision)
    {
        //checks if the attack hits an enemy tagged with "Enemy"
        //if so, calls the takeDamage function in the enemy script
        //deals 20 damage, enemy can take 5 hits before dying
        if (collision.CompareTag("Enemy"))
        {
            if (attackHitbox.GetComponent<Collider2D>().enabled)
            {
                collision.GetComponent<Enemy>().takeDamage(groundAttackDamage);
            }
            else if (airAttackHitbox.GetComponent<Collider2D>().enabled)
            {
                collision.GetComponent<Enemy>().takeDamage(airAttackDamage);
            }
        }
    }

    public void TakeDamageInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TakeDamage(10); // Example damage amount
            Debug.Log("Damage taken via input");
        }
    }

    public void HealInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Heal(10); // Example heal amount
            Debug.Log("Healed via input");
        }
    }

    public void DestroyAfterDeath()
    {
        Debug.Log("Die called - starting death sequence.");
        SceneManager.LoadScene(2);
        Debug.Log("Load Game Over");

        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        //called at end of death animation to destroy enemy object
        //had some weird issues where it wouldn't destroy properly
        //so added a debug log to confirm it was being called
        Debug.Log("Destroying player object.");
        // Destroy(gameObject);
    }
}

