using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    bool finishedGame;
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
        if(finishedGame) return;
        if (!finishedGame) finishedGame = true;
        if (impulseSource)
            impulseSource.GenerateImpulseWithForce(0.7f);
        currentLevelResults.win = playerWon;
        var agent = FindFirstObjectByType<AgentManager>();
        if (agent)
            currentLevelResults.isMlAgent = !agent.isRandom;
        if (int.TryParse(SceneManager.GetActiveScene().name.Split(' ')[^1], out int number))
            currentLevelResults.levelNumber = number;
        else
            currentLevelResults.levelNumber = -1;
        Debug.Log(currentLevelResults.levelNumber);
        currentLevelResults.time = startTime;
        currentLevelResults.final_score = Score;

        if (currentLevelResults.levelNumber == -1)
            StartCoroutine(ImmediatelyGoToLevelSelect());
        else
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

    IEnumerator ImmediatelyGoToLevelSelect()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("LevelSelect");
    }
}
