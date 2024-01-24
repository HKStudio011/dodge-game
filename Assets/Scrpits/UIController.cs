using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public GameObject waitingScreen;
    public GameObject menu;
    public GameObject delay;
    public GameObject groupButtion;
    public GameObject timeText;
    public GameObject timeScore;
    public GameObject highTimeText;
    public GameObject buttonContinue;
    public GameObject buttonADS;
    public GameObject delayScreen;
    private GameController controller;
    private SaveSystem saveSystem;
    // Start is called before the first frame update
    void Start()
    {
        saveSystem = new SaveSystem();
        controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        buttonADS.GetComponent<Button>().interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.IsResume)
        {
           Resume();
        }
        if (controller.IsPause)
        {
            Pause(); 
        }
        if (controller.IsGameOver)
        {
            GameOver();
        }
        
        
    }

    public void setInteractableADSButton()
    {
        buttonADS.GetComponent<Button>().interactable = false;
    }
    public void Restart()
    {
        controller.RestartGame();
    }
    public void Pause()
    {
        controller.PauseGame();
        waitingScreen.SetActive(false);
        menu.SetActive(true);
        groupButtion.SetActive(false);
        buttonContinue.SetActive(true);
        buttonADS.SetActive(false);
        timeScore.GetComponent<TMPro.TextMeshProUGUI>().text = "Time: " + controller.Score()+"s";
        highTimeText.GetComponent<TMPro.TextMeshProUGUI>().text = "High Time: " + saveSystem.LoadPlayer().Item3 + "s";
    }
    public void GameOver()
    {
            controller.GameOver();
         if(controller.LoadAd)
        {
            menu.SetActive(false);
            groupButtion.SetActive(false);
            waitingScreen.SetActive(true);
        }
        else
        {
            waitingScreen.SetActive(false);
            menu.SetActive(true);
            groupButtion.SetActive(false);
            buttonContinue.SetActive(false);
            if (controller.NumberOfPlays > 0) buttonADS.SetActive(true);
            else buttonADS.SetActive(false);
            timeScore.GetComponent<TMPro.TextMeshProUGUI>().text = "Time: " + controller.Score() + "s";
            highTimeText.GetComponent<TMPro.TextMeshProUGUI>().text = "High Time: " + saveSystem.LoadPlayer().Item3 + "s";
        }
            
    }
    public void Continue()
    {
        controller.IsDelay = true;
    }
    public void Resume()
    {
        controller.ResumeGame();
        waitingScreen.SetActive(false);
        menu.SetActive(false);
        groupButtion.SetActive(true);
        timeText.GetComponent<TMPro.TextMeshProUGUI>().text = "Time: " + controller.Score()+"s";
        if (controller.IsDelay)
        {
            Delay();
        }
    }
    public void Delay()
    {
        controller.DelayGame();
        switch(controller.IndexDelay)
        {
            case 1:
                {
                    delayScreen.SetActive(true);
                    delayScreen.GetComponent<TMPro.TextMeshProUGUI>().text = "3";
                    break;
                }
            case 2:
                {
                    delayScreen.SetActive(true);
                    delayScreen.GetComponent<TMPro.TextMeshProUGUI>().text = "2";
                    break;
                }
            case 3:
                {
                    delayScreen.SetActive(true);
                    delayScreen.GetComponent<TMPro.TextMeshProUGUI>().text = "1";
                    break;
                }
            case 4:
                {
                    delayScreen.SetActive(true);
                    delayScreen.GetComponent<TMPro.TextMeshProUGUI>().text = "Go";
                    break;
                }
            default: delayScreen.SetActive(false); break;
        }

    }
    public void Exit()
    {
        controller.ExitGame();
    }
}
