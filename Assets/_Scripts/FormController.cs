using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FormController : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] GameObject InputField;
    
=======
    [SerializeField] TMP_InputField inputField1;
    [SerializeField] GameObject formObject1;
    [SerializeField] TMP_InputField inputField2;
    [SerializeField] GameObject formObject2;
    [SerializeField] TMP_InputField inputField3;
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
>>>>>>> Stashed changes
    
    public void SubmitForm()
    {
        string text = InputField.GetComponent<TextMeshProUGUI>().text;
        Debug.Log("Submit: " + text);
        SceneManager.LoadScene("LevelSelect");
    }
}
