using UnityEngine;
using UnityEngine.SceneManagement;
public class Buttons : MonoBehaviour
{

    public CanvasGroup canvas;
    public void BackToMenu()
    {
        //load the menu scene
        SceneManager.LoadScene("StartMenu");
    }

    public void StartGame()
        {
            //load the game scene
            SceneManager.LoadScene("MainShip");
    }

    public void ExitGame()
    {
               //exit the game
        Application.Quit();
    }

    public void CanvasOn()
    {
        canvas.alpha = 1;
    }

    public void CanvasOff()
    {
        canvas.alpha = 0;
    }
}
