using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
    [SerializeField] GameObject inputField;
    [SerializeField] GameObject formObject;
    [SerializeField] float waitTimeOnEffect;
    void Awake()
    {
        StartCoroutine(OpenForm());
    }

    private IEnumerator OpenForm()
    {
        yield return new WaitForSeconds(waitTimeOnEffect);
        formObject.SetActive(true);
    }
    
    public void SubmitForm()
    {
        string text = inputField.GetComponent<TextMeshProUGUI>().text;
        Debug.Log("Submit: " + text);
        SceneManager.LoadScene("LevelSelect");
    }
}
