using UnityEngine;

public class Enemy : MonoBehaviour
{
    //various initialization of Enemy variables, including health, speed
    //hitboxes, locations, and attack settings
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    private Animator animator;
    private Rigidbody2D rb;

    public float speed = 2f;

    public GameObject pointA;
    public GameObject pointB;
    private Transform currentPoint;

    public Transform player;
    public GameObject attackHitbox;

    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    private bool playerInRange = false;

    public int attackDamage = 10; // Enemy attack damage

    void Start()
    {
        //set max health, initialize Rigidbody2d and Animator
        //Begin to walk and switch attack hitbox off; begins cycle of patrol
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentPoint = pointB.transform;
        animator.SetBool("isWalking", true);

        SetHitboxEnabled(attackHitbox, false);
    }

    void Update()
    {
        //initial check to ensure no errors
        if (isDead) return;

        // Movement patrol
        Vector2 moveDirection = (currentPoint.position - transform.position).normalized;
        rb.linearVelocity = moveDirection * speed;

        if (moveDirection.x != 0)
            Flip(moveDirection.x);


        //Very basic patrol script, essentially moves left and right from a point B to point A
        //Still have some issues with figuring out animations, but ran out of time
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.1f)
            currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;

        animator.SetBool("isWalking", !playerInRange);

        // Attack logic
        if (playerInRange)
        {
            //is the player in range? If so is there enough time between the last attack?
            //aka is cooldown over, if so, attack.
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                AttackPlayer();
                attackTimer = attackCooldown;
            }
        }
    }

    private void Flip(float direction)
    {
        //left and right patrol flip method
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //checks if the player is touching the main sensor collider2D circle in front of the enemy
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //opposite of above script, if player is not detected, then player is not in range
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }

    void AttackPlayer()
    {
        //Added a lot of debug logs since I didn't know if things were running properly
        animator.SetBool("isAttacking", true);
        SetHitboxEnabled(attackHitbox, true);
        Debug.Log("Enemy attacking player!");
    }

    public void EndAttack()
    {
        //used to prevent a loop from occuring in the Animator, using animation events
        animator.SetBool("isAttacking", false);
        SetHitboxEnabled(attackHitbox, false);
    }

    public void SetHitboxEnabled(GameObject hitbox, bool enabled)
    {
        //Does the hitbox exist? then get hitbox component and enable.
        if (hitbox != null)
            hitbox.GetComponent<Collider2D>().enabled = enabled;
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.CompareTag("Player"))
        {
            Collider2D hitboxCollider = attackHitbox.GetComponent<Collider2D>();
            if (hitboxCollider != null && hitboxCollider.enabled)
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null)
                {
                    //makes sure player is properly assigned, and allows for dmg to be dealt
                    //assuming the player is still within range of the attack.
                    player.TakeDamage(attackDamage);
                    Debug.Log("Enemy hit the player for " + attackDamage + " damage!");
                    SetHitboxEnabled(attackHitbox, false); // Prevent repeated hits
                }
            }
        }
    }

    public void takeDamage(int damage)
    {
        //DMG method, same concept, if is dead then return from method
        if (isDead) return;
        currentHealth -= damage;

        Debug.Log("Enemy took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth > 0)
        {
            animator.SetBool("isHurt", true);
        }
        else
        {
            Die();
        }
    }

    public void EndHurt()
    {
        //Is no longer hurt, supposed to allow for an animation to play
        //but when i added additional details to the enemy movement
        //the animator broke slightly :(
        animator.SetBool("isHurt", false);
    }

    void Die()
    {
        //Was meant to allow for the portal to appear after you had beaten the first enemy
        //but I couldn't figure out how to properly implement with the time I had left
        Debug.Log("Portal Released");
        GameState.EnemyHasDied();
        isDead = true;
        Debug.Log("Enemy died!");
        animator.SetTrigger("Die");

        GetComponent<Collider2D>().enabled = false;
        rb.simulated = false;
    }

    public void DestroyAfterDeath()
    {
        //Delete enemy object after death so it doesn't mess with player movement
        Debug.Log("Destroying enemy object.");
        Destroy(gameObject);
    }
}
