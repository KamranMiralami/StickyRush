using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FormController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WonOrLost;
    [SerializeField] TMP_InputField inputField1;
    [SerializeField] TMP_InputField inputField2;
    [SerializeField] TMP_InputField inputField3;
    [SerializeField] TMP_InputField inputField4;
    [SerializeField] TMP_InputField inputField5;
    public void OpenForm(bool playerWon)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        if(playerWon)
        {
            WonOrLost.text = "You Win!";
        }
        else
        {
            WonOrLost.text = "You Lost!";
        }
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager.Instance.SendTelementry();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SubmitForm()
    {
        Time.timeScale = 1f;
        string text1 = inputField1.text;
        string text2 = inputField2.text;
        string text3 = inputField3.text;
        string text4 = inputField4.text;
        string text5 = inputField5.text;
        if (int.TryParse(text1, out int number1))
        {
            GameManager.Instance.currentLevelResults.Q1 = number1;
        }
        if (int.TryParse(text2, out int number2))
        {
            GameManager.Instance.currentLevelResults.Q2 = number2;
        }
        if (int.TryParse(text3, out int number3))
        {
            GameManager.Instance.currentLevelResults.Q3 = number3;
        }
        if (int.TryParse(text4, out int number4))
        {
            GameManager.Instance.currentLevelResults.Q4 = number4;
        }
        if (int.TryParse(text5, out int number5))
        {
            GameManager.Instance.currentLevelResults.Q5 = number5;
        }
        GameManager.Instance.SendTelementry();
        SceneManager.LoadScene("LevelSelect");
    }
}
