using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{   
    //simple method, loads scene 1 for the game scene, and quit's application
    //for the other method (both public so used onClick with buttons)
    public void PlayGame()
    {
        Debug.Log("PlayGame called");
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Debug.Log("QUIT!! AAARGHHH");
        Application.Quit();
    }
}