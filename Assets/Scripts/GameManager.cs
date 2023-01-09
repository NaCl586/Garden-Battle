using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text countdownText;
    public Text timerText;
    public Text playerOneText;
    public Text playerTwoText;
    public float timer;

    private enum GMStates
    {
        countdown, play, finish
    };
    private GMStates states;

    // Start is called before the first frame update
    void Start()
    {
        formatTime();
        states = GMStates.countdown;
        PlayerController.canMove = false;
        StartCoroutine(countdown());
    }

    IEnumerator countdown()
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        countdownText.text = "2";
        yield return new WaitForSeconds(1);
        countdownText.text = "1";
        yield return new WaitForSeconds(1);
        countdownText.text = "Plant!";
        PlayerController.canMove = true;
        states = GMStates.play;
        yield return new WaitForSeconds(2);
        countdownText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if(states == GMStates.play)
        {
            formatTime();
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                StartCoroutine(timeUp());
            }
        }
    }

    void formatTime()
    {
        int seconds = (int)timer % 60;
        int minutes = (int)timer / 60;
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    IEnumerator timeUp()
    {
        Time.timeScale = 0;
        timerText.text = "0:00";
        countdownText.text = "Time Up!";
        PlayerController.canMove = false;
        states = GMStates.finish;
        yield return new WaitForSecondsRealtime(2);
        countdownText.text = "";
        yield return new WaitForSecondsRealtime(2);

        if (int.Parse(playerOneText.text) > int.Parse(playerTwoText.text))
            countdownText.text = "Player 1 Win!";
        else if (int.Parse(playerOneText.text) < int.Parse(playerTwoText.text))
            countdownText.text = "Player 2 Win!";
        else 
            countdownText.text = "Draw!";
    }
}
