using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Grid GeneralGrid;
    public Action<int> OnScoreChanged;
    [SerializeField] GameObject form;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] GameObject effectObject;
    [HideInInspector] public LevelResults currentLevelResults;
    public Action<bool> OnHitSpike;
    public Action OnTinyReward;
    public Action PlayerMadeAMove;
    public Action OnPlayerQuit;
    private float startTime;
    bool countTime;
    private void OnEnable()
    {
        base.Awake();
        currentLevelResults = new(0,0,0,0,false,0,false,false,0,0,0);
        countTime = true;
        PlayerMadeAMove += OnPlayerMoved;
        OnTinyReward += OnTinyRewardRecieved;
        OnPlayerQuit += OnPlayerQuitGame;
        OnHitSpike += OnPlayerHitSpike;
    }

    private void OnPlayerHitSpike(bool obj)
    {
        if (obj)
            currentLevelResults.spikes_removed++;
    }

    private void OnPlayerQuitGame()
    {
        currentLevelResults.playerQuit = true;
    }

    private void OnTinyRewardRecieved()
    {
        currentLevelResults.tinyRewards++;
    }

    private void OnDisable()
    {
        PlayerMadeAMove -= OnPlayerMoved;
        countTime = false;
    }
    private void OnPlayerMoved()
    {
        currentLevelResults.moves++;
    }
    private void Update()
    {
        if(countTime)
            startTime += Time.deltaTime;
    }
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
        if (scoreText)
            scoreText.text = Score.ToString();
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
    public void FinishLevel(bool playerWon)
    {
        return;
        if(form !=null) 
            form.SetActive(true);
        if (impulseSource)
            impulseSource.GenerateImpulseWithForce(0.7f);
        currentLevelResults.win = playerWon;
        currentLevelResults.isMlAgent = true;
        currentLevelResults.time = startTime;
        currentLevelResults.final_score = Score;
        StartCoroutine(OpenForm());
    }

    private IEnumerator OpenForm()
    {
        countTime = false;
        if (form == null || effectObject == null)
            yield break;
        yield return new WaitForSeconds(0.7f);
        effectObject.SetActive(true);
        form.SetActive(true);
    }
    public void SendTelementry()
    {
        APIHandler.Instance.SendData(currentLevelResults);
    }
}
