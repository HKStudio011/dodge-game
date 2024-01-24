using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    private bool isPause, isResume, isGameOver,isDelay;
    private float timeStart, timePause;
    private PlayerContronller player;
    private int indexDelay;
    private float timeDelay;
    private SaveSystem saveSystem;
    private int numberOfPlays;
    private PlayGameServiceController playGame;
    public int timeStep;
    private bool adRequest;
    private bool continueGift;
    private bool loadAd;
    public bool IsPause { get => isPause; set => isPause = value; }
    public bool IsResume { get => isResume; set => isResume = value; }
    public bool IsGameOver { get => isGameOver; set => isGameOver = value; }
    public float TimeStart { get => timeStart; }
    public bool IsDelay { get => isDelay; set => isDelay = value; }
    public int IndexDelay { get => indexDelay;}
    public int NumberOfPlays { get => numberOfPlays;}
    public bool AdRequest { get => adRequest; set => adRequest = value; }
    public bool LoadAd { get => loadAd; set => loadAd = value; }

    // Start is called before the first frame update
    void Start()
    {
        saveSystem = new SaveSystem();
        loadAd = false;
        continueGift = true;
        IsGameOver = false;
        IsPause = false;
        IsResume = true;
        IsDelay = true;
        timeDelay = 0;
        adRequest=false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContronller>();
        playGame = GameObject.FindGameObjectWithTag("PlayGameService").GetComponent<PlayGameServiceController>();
        Time.timeScale = 1;
        timeStart = Time.time;
        numberOfPlays = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (player.IsDead)
        {
            GameOver();
            if (adRequest)
            {
                adRequest = false;
                ContinueGame();
            }
            if (saveSystem.LoadPlayer().Item2 && continueGift)
            {
                
                if (!loadAd)
                {
                    DestroyObject();
                }
                else
                {
                    ContinueGame();
                    continueGift = false;
                    loadAd = false;
                }               
            }           
        }
       
    }
    public void DestroyObject()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Threat");
        foreach (GameObject item in temp) Destroy(item);

        if (temp.Length == 0)
        {
            //uncall waitting screen
            loadAd = false;
        }
        //call waitting screen
        else loadAd = true;
    }
    public void GameOver()
    {
        Time.timeScale = 0;
        IsGameOver = true;
        IsPause=false;
        IsResume = false;
        Score();
        Achievement();
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
        IsGameOver = false;
        IsPause=true;
        IsResume=false;

    }
    public void RestartGame()
    {

        SceneManager.LoadScene(1);
        Time.timeScale = 1;
        IsGameOver =false;
        IsPause = false;
        IsResume=true;
    }
    public void ResumeGame()
    {
        IsGameOver = false;
        IsPause = false;
        IsResume = true;
    }
    public void DelayGame()
    {
        Time.timeScale = 0;
        timeDelay++;
        if (timeDelay <= 1* timeStep)
        {
            indexDelay = 1;
            return;
        }
        if (timeDelay <= 2 * timeStep)
        {
            indexDelay = 2;
            return;
        }
        if (timeDelay <= 3 * timeStep)
        {
            indexDelay = 3;
            return;
        }
        if (timeDelay <= 4 * timeStep)
        {
            indexDelay = 4;
            return;
        }
        isDelay = false;
        timeDelay = 0;
        Time.timeScale = 1;
        indexDelay = 5;
        return;
    }
    public string Score()
    {
        string score = "";
        var temp = saveSystem.LoadPlayer();
        score = ""+(Time.time - timeStart);
        if (float.Parse(score) > temp.Item3 && isGameOver)
        {
            saveSystem.SavePlayer(temp.Item1, temp.Item2, float.Parse(score));
            playGame.AddScoreToLeaderboard(score);
        }
            
        return score;
    }
    public void ContinueGame()
    {
        if(numberOfPlays>0)
        {
            numberOfPlays--;
            player.IsDead = false;
            isDelay = true;
            ResumeGame();
        }    
    }
    public void ExitGame()
    {
        SceneManager.LoadScene(0);
    }
    
    private void OnApplicationPause(bool pause)
    {
        PauseGame();
    }
    
    public void Achievement()
    {
        var temp= saveSystem.LoadPlayer();
        if (temp.Item2) playGame.UnlockAchievement("Remove ADS");
        if (temp.Item3 >= 2.0f && !temp.Item1.Contains("Newbie"))
        {
            playGame.UnlockAchievement("Newbie");
            temp.Item1.Add("Newbie");
        }
        if (temp.Item3 >= 10.0f && !temp.Item1.Contains("Pro Player"))
        {
            playGame.UnlockAchievement("Pro Player");
            temp.Item1.Add("Pro Player");
        }
        if (temp.Item3 >= 20.0f && !temp.Item1.Contains("God"))
        {
            playGame.UnlockAchievement("God");
            temp.Item1.Add("God");
        }
        saveSystem.SavePlayer(temp.Item1, temp.Item2, temp.Item3);
    }
}
