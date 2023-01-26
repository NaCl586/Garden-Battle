using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameDone : MonoBehaviour
{
    [SerializeField] private GameObject GameDoneCanvas;
    public void EndGame()
    {
        Time.timeScale = 0f;
        GameDoneCanvas.SetActive(true);
    }
    public void Restart()
    {
        GameDoneCanvas.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void MainMenu()
    {
        GameDoneCanvas.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

}
