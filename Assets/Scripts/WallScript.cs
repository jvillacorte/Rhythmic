using UnityEngine;

public class WallScript : MonoBehaviour
{
    //Meant to be added to a wall that would disappear on enemy death
    //Ran out of time to properly utilize
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
