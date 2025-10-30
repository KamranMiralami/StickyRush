using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
    [SerializeField] GameObject InputField;
    
    
    public void SubmitForm()
    {
        string text = InputField.GetComponent<TextMeshProUGUI>().text;
        Debug.Log("Submit: " + text);
        SceneManager.LoadScene("LevelSelect");
    }
}
