using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    [SerializeField] GameObject spaceText;
    [SerializeField] GameObject consentForm;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceText.SetActive(false);
            consentForm.SetActive(true);
        }
    }

    public void GiveConsent()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
