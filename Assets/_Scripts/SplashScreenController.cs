using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : MonoBehaviour
{
    [SerializeField] GameObject spaceText;
    [SerializeField] GameObject consentForm;
    private void Start()
    {
        Application.targetFrameRate = 120;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetMouseButtonDown(0) || Input.touchCount>0)
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
