using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Grid GeneralGrid;
    public Action<float> OnScoreChanged;
    [SerializeField] FormController form;
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
        currentLevelResults = new();
        countTime = true;
        PlayerMadeAMove += OnPlayerMoved;
        OnTinyReward += OnTinyRewardRecieved;
        OnPlayerQuit += OnPlayerQuitGame;
        OnHitSpike += OnPlayerHitSpike;
        OnScoreChanged += ScoreChanged;
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
    public float Score
    {
        get { return _score; } 
        private set 
        { 
            _score = value; 
            _score = Mathf.Clamp(_score, 0f, float.MaxValue);
            var shownScore = Mathf.RoundToInt(_score * 100f) / 100f;
            OnScoreChanged?.Invoke(shownScore);
        }
    }
    float _score;
    public void GivePlayerReward(float val)
    {
        Score += val;
        if(Score <= 0)
        {
            FinishLevel(false);
        }
    }
    public void ScoreChanged(float val)
    {
        if (scoreText != null)
            scoreText.text = val.ToString();
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
        if (impulseSource)
            impulseSource.GenerateImpulseWithForce(0.7f);
        currentLevelResults.win = playerWon;
        var agent = FindFirstObjectByType<AgentManager>();
        currentLevelResults.isMlAgent = !agent.isRandom;
        currentLevelResults.levelNumber = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex-1;
        currentLevelResults.time = startTime;
        currentLevelResults.final_score = Score;
        StartCoroutine(OpenForm(playerWon));
    }

    private IEnumerator OpenForm(bool playerWon)
    {
        countTime = false;
        if (effectObject != null)
        {
            yield return new WaitForSeconds(0.7f);
            effectObject.SetActive(true);
        }
        if (form != null)
        {
            yield return new WaitForSeconds(2f);
            form.OpenForm(playerWon);
        }
    }
    public void SendTelementry()
    {
        if(APIHandler.Instance != null)
            APIHandler.Instance.SendData(currentLevelResults);
    }
}
