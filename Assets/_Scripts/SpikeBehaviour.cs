using DG.Tweening;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    // [SerializeField] Renderer targetRenderer;
    [SerializeField] Animator animator;
    [SerializeField] float stateTime = 2f;
    [SerializeField] float currentTime = 0f;
    bool isOpen;
    public bool IsOpen => !isOpen;
    void Update()
    {
        currentTime += Time.deltaTime; 
        if (currentTime >= stateTime)
        {
            //Debug.Log("Spike Swapping: Was open = " + isOpen, this);
            currentTime = 0f;
            if (isOpen)
            {
                animator.SetTrigger("Close");
                StartCoroutine(GameManager.DoWithDelay(0.3f, () =>
                {
                    isOpen = false;
                }));
                stateTime = 2f;
            } 
            else
            {
                animator.SetTrigger("Open");
                isOpen = true;
                stateTime = 1f;
            }
        }
    }
}
