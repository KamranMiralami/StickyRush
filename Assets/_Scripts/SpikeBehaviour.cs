using DG.Tweening;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    // [SerializeField] Renderer targetRenderer;
    [SerializeField] Animator animator;
    [SerializeField] float stateTime = 2f;
    float currentTime = 0f;
    bool isOpen;
    public bool IsOpen()
    {
        // OKAY, I'm going to be honest: I have tried everything in the update function with changing the isOpen
        // and for NO apparent reason, is this the only correct way, if we test it with the visuals.
        // IT IS 2:39 AM RIGHT NOW, I AM LEAVING IT LIKE THIS!
        // Honestly just don't touch it, the best games have the worst code (Shout-Out to Undertale)
        
        return !isOpen;
    }
    // Sequence seq;
    // private void Awake()
    // {
    //     //StartFlipping();
    // }
    // void StartFlipping()
    // {
    //     seq = DOTween.Sequence();
    //     seq.Append(
    //         targetRenderer.material.DOColor(Color.green, 2f)
    //     );
    //     seq.AppendInterval(2f);
    //     seq.Append(
    //         targetRenderer.material.DOColor(Color.red, 2f)
    //     );
    //     seq.AppendInterval(2f);
    //     seq.SetLoops(-1);
    // }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= stateTime)
        {
            Debug.Log("Spike Swapping: Was open = " + isOpen, this);
            currentTime = 0f;
            if (isOpen)
            {
                animator.SetTrigger("Close");
                isOpen = false;
            }
            else
            {
                animator.SetTrigger("Open");
                isOpen = true;
            }
        }
    }
}
