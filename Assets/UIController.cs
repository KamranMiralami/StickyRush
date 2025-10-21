using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject infoMenu;

    public void BackToLevelSelect()
    {
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
