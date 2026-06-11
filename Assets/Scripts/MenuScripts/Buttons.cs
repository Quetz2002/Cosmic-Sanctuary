using UnityEngine;
using UnityEngine.SceneManagement;
public class Buttons : MonoBehaviour
{
    public void BackToMenu()
    {
        //load the menu scene
        SceneManager.LoadScene("StartMenu");
    }

    public void StartGame()
        {
            //load the game scene
            SceneManager.LoadScene("Nave");
    }

    public void ExitGame()
    {
               //exit the game
        Application.Quit();
    }
}
