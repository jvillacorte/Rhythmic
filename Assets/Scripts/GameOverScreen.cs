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
            //Does game over panel exist? then enable and freeze time
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            //check to ensure that gameoverpanel is assigned in the inspector list
            Debug.LogWarning("GameOverPanel not assigned in Inspector.");
        }
    }

    public void RestartGame()
    {
        //turns time back to normal, loads scene 1 (gameplay)
        Debug.Log("Restarting");
        Time.timeScale = 1f; 
        SceneManager.LoadScene(1); 
    }

    public void ExitToMenu()
    {
        //Exits to title sceen/menu, sets time to 1/normal scale
        Debug.Log("Exiting");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Title");
    }
}
