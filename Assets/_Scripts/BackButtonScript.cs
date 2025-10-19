using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonScript : MonoBehaviour
{
    public void BackToLevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
