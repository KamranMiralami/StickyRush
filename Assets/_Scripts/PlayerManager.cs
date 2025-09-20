using DG.Tweening;
using System;
using UnityEngine;

public class PlayerManager : SingletonBehaviour<PlayerManager>
{
    //Animations
    SetDirectionalAnimations animationControllerScript;
    
    public Action<Vector2> OnSwipeDirection;
    [SerializeField] float minSwipeDistance = 10f;
    [SerializeField] float movementSpeed = 10f;
    Vector2 startPos;
    Grid grid;
    bool isMoving = false;
    bool canMove = true;
    Tween moveTween;

    private void Start()
    {
        animationControllerScript = GetComponent<SetDirectionalAnimations>();
        if (!animationControllerScript)
            Debug.Log("In order to use animations on this, you need a SetDirectionalAnimations script", this);
        grid = FixToGrid.GeneralGrid;
    }
    
    protected override void Awake()
    {
        base.Awake();

        OnSwipeDirection += OnPlayerSwipe;
    }
    private void Update()
    {
        CheckSwipeLogic();
    }
    
    private void OnPlayerSwipe(Vector2 dir)
    {
        //Animations
        if (animationControllerScript)
            animationControllerScript.SetDirection(dir);
        
        // cast a ray only against walls
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 100f, LayerMask.GetMask("Wall"));

        Vector3 targetPos;

        if (hit.collider != null)
        {
            // move to cell just before the wall
            Vector3 stopPos = hit.point - dir * GameManager.Instance.GetGridSize() * 0.5f;
            Vector3Int cell = grid.WorldToCell(stopPos);
            Debug.Log("Wall hit at "+hit.point +" "+ GameManager.Instance.GetGridSize()+" " + stopPos + " "+hit.transform.gameObject.name);
            targetPos = grid.GetCellCenterWorld(cell);
        }
        else
        {
            // nothing hit → move to max distance
            Debug.LogError("No wall hit");
            return;
        }
        isMoving = true;
        moveTween = transform.DOMove(targetPos, movementSpeed)
            .SetEase(Ease.InSine)
            .SetSpeedBased(true)
            .OnComplete(() =>
            {
                StartCoroutine(GameManager.DoWithDelay(.5f, () =>
                {
                    isMoving = false;
                }));
            });
    }
    private void CheckSwipeLogic()
    {
        if (!canMove) return;
        if (isMoving) return;
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
        }
        //Debug.Log(((Vector2)Input.mousePosition - startPos).magnitude);
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 endPos = Input.mousePosition;
            Vector2 delta = endPos - startPos;

            if (delta.magnitude < minSwipeDistance) return; // too small, ignore

            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0)
                {
                    OnSwipeDirection?.Invoke(Vector2.right);
                    Debug.Log("Swipe right");
                }
                else
                {
                    OnSwipeDirection?.Invoke(Vector2.left);
                    Debug.Log("Swipe left");
                }
            }
            else
            {
                if (delta.y > 0)
                {
                    OnSwipeDirection?.Invoke(Vector2.up);
                    Debug.Log("Swipe up");
                }
                else
                {
                    OnSwipeDirection?.Invoke(Vector2.down);
                    Debug.Log("Swipe down");
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Reward"))
        {
            Debug.Log("Collided with reward");
            moveTween?.Kill(false);
            transform.position = collision.transform.position;
            transform.DOScale(Vector3.one * 1.2f, 0.1f).SetLoops(-1);
            Destroy(collision.gameObject);
            canMove = false;
        }
    }
}
