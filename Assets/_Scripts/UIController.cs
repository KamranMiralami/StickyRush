using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject infoMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] Slider volumeSlider;
    bool settingsOpen;
    
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

    public void ClickSettingsButton()
    {
        settingsOpen = !settingsOpen;
        settingsMenu.SetActive(settingsOpen);

        if (settingsOpen)
            volumeSlider.value = MusicPlayer.instance.GetVolume();
    }

    public void SetVolume()
    {
        MusicPlayer.instance.SetVolume(volumeSlider.value);
    }
}
