using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    //Canvas that includes all the Game Over UI
    public GameObject gameOverPanel;

    public void TriggerGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("GameOverPanel not assigned in Inspector.");
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting");
        Time.timeScale = 1f; 
        SceneManager.LoadScene(1); 
    }

    public void ExitToMenu()
    {
        Debug.Log("Exiting");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
