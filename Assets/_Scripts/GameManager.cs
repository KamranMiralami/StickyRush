using System;
using System.Collections;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Grid GeneralGrid;
    public GameObject Reward;
    public Action<int> OnScoreChanged;
    [SerializeField] GameObject form;
    public int Score
    {
        get { return _score; } 
        private set 
        { 
            _score = value; 
            OnScoreChanged?.Invoke(_score);
        }
    }
    int _score;
    public void GivePlayerReward(int val)
    {
        Score += val;
    }
    public float GetGridSize()
    {
        return GeneralGrid.cellSize.x;
    }
    public static IEnumerator DoWithDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
<<<<<<< Updated upstream

    
    public void FinishLevel()
=======
    public void FinishLevel(bool playerWon)
    {
        if(form !=null) 
            form.SetActive(false);
        if (impulseSource)
            impulseSource.GenerateImpulseWithForce(0.7f);
        currentLevelResults.win = playerWon;
        var agent = FindFirstObjectByType<AgentManager>();
        currentLevelResults.isMlAgent = !agent.isRandom;
        currentLevelResults.levelNumber = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex-1;
        currentLevelResults.time = startTime;
        currentLevelResults.final_score = Score;
        StartCoroutine(OpenForm());
    }

    private IEnumerator OpenForm()
    {
        countTime = false;
        if (form == null || effectObject == null)
            yield break;
        yield return new WaitForSeconds(2.5f);
        effectObject.SetActive(true);
        form.SetActive(true);
    }
    public void SendTelementry()
>>>>>>> Stashed changes
    {
        StartCoroutine(DoWithDelay(2f, () => form.SetActive(true)));
    }
}
