using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Grid GeneralGrid;
    public GameObject Reward;
    public Action<int> OnScoreChanged;
    [SerializeField] GameObject form;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] CinemachineImpulseSource impulseSource;
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
        if (impulseSource)
            impulseSource.GenerateImpulseWithForce(0.7f);
        StartCoroutine(OpenForm());
    }

    private IEnumerator OpenForm()
    {
        yield return new WaitForSeconds(0.7f);
        form.SetActive(true);
    }
}
