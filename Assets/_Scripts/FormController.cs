using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class FormController : MonoBehaviour
{
    [SerializeField] GameObject inputField1;
    [SerializeField] GameObject formObject1;
    [SerializeField] GameObject inputField2;
    [SerializeField] GameObject formObject2;
    [SerializeField] GameObject inputField3;
    [SerializeField] GameObject formObject3;
    [SerializeField] float waitTimeOnEffect;
    void Awake()
    {
        StartCoroutine(OpenForm());
    }

    private IEnumerator OpenForm()
    {
        yield return new WaitForSeconds(waitTimeOnEffect);
        formObject1.SetActive(true);
        formObject2.SetActive(true);
        formObject3.SetActive(true);
    }
    
    public void SubmitForm()
    {
        string text1 = inputField1.GetComponent<TextMeshProUGUI>().text;
        string text2 = inputField2.GetComponent<TextMeshProUGUI>().text;
        string text3 = inputField3.GetComponent<TextMeshProUGUI>().text;
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
        GameManager.Instance.SendTelementry();
        SceneManager.LoadScene("LevelSelect");
    }
}
