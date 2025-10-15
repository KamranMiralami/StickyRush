using DG.Tweening;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    // [SerializeField] Renderer targetRenderer;
    [SerializeField] Animator animator;
    [SerializeField] float stateTime = 2f;
    float currentTime = 0f;
    bool isOpen = true;
    public bool IsOpen()
    {
        return isOpen;
        // if (targetRenderer.material.color == Color.green)
        //     return true;
        // else
        //     return false;
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
            currentTime = 0f;
            if (isOpen)
                animator.SetTrigger("Close");
            else
                animator.SetTrigger("Open");
            isOpen = !isOpen;
        }
    }
}
