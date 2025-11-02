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
    [SerializeField] TMP_InputField inputField6;
    [SerializeField] TMP_InputField inputField7;
    [SerializeField] TMP_InputField inputField8;
    [SerializeField] TMP_InputField inputField9;
    [SerializeField] TMP_InputField inputField10;
    [SerializeField] TMP_InputField inputField11;
    public void OpenForm(bool playerWon, string reason)
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        if(playerWon)
        {
            WonOrLost.text = "You Win!";
        }
        else
        {
            WonOrLost.text = "You Lost! "+reason;
        }
    }
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        GameManager.Instance.currentLevelResults.playerQuit = true;
        SetQuestionScores();
        GameManager.Instance.SendTelementry();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void SubmitForm()
    {
        Time.timeScale = 1f;
        SetQuestionScores();
        GameManager.Instance.SendTelementry();
        SceneManager.LoadScene("LevelSelect");
    }
    public void SetQuestionScores()
    {
        string text1 = inputField1.text;
        string text2 = inputField2.text;
        string text3 = inputField3.text;
        string text4 = inputField4.text;
        string text5 = inputField5.text;
        string text6 = inputField6.text;
        string text7 = inputField7.text;
        string text8 = inputField8.text;
        string text9 = inputField9.text;
        string text10 = inputField10.text;
        string text11 = inputField11.text;
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
        if (int.TryParse(text6, out int number6))
        {
            GameManager.Instance.currentLevelResults.Q6 = number6;
        }
        if (int.TryParse(text7, out int number7))
        {
            GameManager.Instance.currentLevelResults.Q7 = number7;
        }
        if (int.TryParse(text8, out int number8))
        {
            GameManager.Instance.currentLevelResults.Q8 = number8;
        }
        if (int.TryParse(text9, out int number9))
        {
            GameManager.Instance.currentLevelResults.Q9 = number9;
        }
        if (int.TryParse(text10, out int number10))
        {
            GameManager.Instance.currentLevelResults.Q10 = number10;
        }
        if (int.TryParse(text11, out int number11))
        {
            GameManager.Instance.currentLevelResults.Q11 = number11;
        }
    }
}
