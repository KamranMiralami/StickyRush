using DG.Tweening;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
    [SerializeField] Renderer targetRenderer;
    public bool IsOpen()
    {
        if (targetRenderer.material.color == Color.green)
            return true;
        else
            return false;
    }
    Sequence seq;
    private void Awake()
    {
        StartFlipping();
    }
    void StartFlipping()
    {
        seq = DOTween.Sequence();
        seq.Append(
            targetRenderer.material.DOColor(Color.green, 2f)
        );
        seq.AppendInterval(2f);
        seq.Append(
            targetRenderer.material.DOColor(Color.red, 2f)
        );
        seq.AppendInterval(2f);
        seq.SetLoops(-1);
    }
}
