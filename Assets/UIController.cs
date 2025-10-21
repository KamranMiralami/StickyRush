using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject infoMenu;

    public void BackToLevelSelect()
    {
        GameManager.Instance.currentLevelResults.playerQuit = true;
        GameManager.Instance.SendTelementry();
        SceneManager.LoadScene("LevelSelect");
    }

    public void OpenInfoMenu()
    {
        infoMenu.SetActive(true);
    }

    public void CloseInfoMenu()
    {
        infoMenu.SetActive(false);
    }
}
