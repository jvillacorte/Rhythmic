using UnityEngine;

public class WallScript : MonoBehaviour
{
    private void OnEnable()
    {
        GameState.OnEnemyKilled += HandleEnemyKilled;
    }

    private void OnDisable()
    {
        GameState.OnEnemyKilled -= HandleEnemyKilled;
    }

    private void HandleEnemyKilled()
    {
        gameObject.SetActive(true); // Activate the wall when enemy dies
    }
}
