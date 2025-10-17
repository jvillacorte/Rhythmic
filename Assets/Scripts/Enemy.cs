using UnityEngine;

public class Enemy : MonoBehaviour
{
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
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentPoint = pointB.transform;
        animator.SetBool("isWalking", true);

        SetHitboxEnabled(attackHitbox, false);
    }

    void Update()
    {
        if (isDead) return;

        // Movement patrol
        Vector2 moveDirection = (currentPoint.position - transform.position).normalized;
        rb.linearVelocity = moveDirection * speed;

        if (moveDirection.x != 0)
            Flip(moveDirection.x);

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.1f)
            currentPoint = (currentPoint == pointB.transform) ? pointA.transform : pointB.transform;

        animator.SetBool("isWalking", !playerInRange);

        // Attack logic
        if (playerInRange)
        {
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
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = false;
    }

    void AttackPlayer()
    {
        animator.SetBool("isAttacking", true);
        SetHitboxEnabled(attackHitbox, true);
        Debug.Log("Enemy attacking player!");
    }

    public void EndAttack()
    {
        animator.SetBool("isAttacking", false);
        SetHitboxEnabled(attackHitbox, false);
    }

    public void SetHitboxEnabled(GameObject hitbox, bool enabled)
    {
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
                    player.TakeDamage(attackDamage);
                    Debug.Log("Enemy hit the player for " + attackDamage + " damage!");
                    SetHitboxEnabled(attackHitbox, false); // Prevent repeated hits
                }
            }
        }
    }

    public void takeDamage(int damage)
    {
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
        animator.SetBool("isHurt", false);
    }

    void Die()
    {
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
        Debug.Log("Destroying enemy object.");
        Destroy(gameObject);
    }
}
