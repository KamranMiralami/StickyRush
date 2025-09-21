using System;
using System.Collections;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public Grid GeneralGrid;
    public GameObject Reward;
    public float GetGridSize()
    {
        return GeneralGrid.cellSize.x;
    }
    public static IEnumerator DoWithDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
