using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectController : MonoBehaviour
{
    public void ChooseLevel(int levelNumber)
    {
        try { SceneManager.LoadScene("Level " + levelNumber); }
        catch { Debug.Log("Error loading level " + levelNumber); }
    }
    public void ExitGame()
    {
               Application.Quit();
    }
}
