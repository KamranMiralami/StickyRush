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
    public void FinishLevel(bool playerWon)
    {
        if(form !=null) 
            form.SetActive(true);
    }
}
