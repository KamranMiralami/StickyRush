using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonScript : MonoBehaviour
{
    public void BackToLevelSelect()
    {
        GameManager.Instance.OnPlayerQuit?.Invoke();
        GameManager.Instance.SendTelementry();
        SceneManager.LoadScene("LevelSelect");
    }
}
