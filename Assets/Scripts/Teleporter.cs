using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Transform teleportTarget; // Reference to the target teleport location

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Teleport the player to the target location
            collision.transform.position = teleportTarget.position;
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Reset player's velocity upon teleportation
            }
        }
    }
}
