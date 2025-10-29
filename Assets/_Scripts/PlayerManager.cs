using DG.Tweening;
using System;
using Unity.Cinemachine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerManager :  SingletonBehaviour<PlayerManager>
{
    //Animations
    SetDirectionalAnimations animationControllerScript;
    
    public Action<Vector2> OnSwipeDirection;
    [SerializeField] float minSwipeDistance = 10f;
    [SerializeField] float movementSpeed = 10f;
    Vector2 levelStartPos;
    Vector2 startPos;
    Grid grid;
    bool isMoving = false;
    bool canMove = true;
    Tween moveTween;
    Vector3 prevPos; 
    private CinemachineBasicMultiChannelPerlin camShake;
    [SerializeField] GameObject tutorialDeathText;
    protected override void Awake()
    {
        base.Awake();
        OnSwipeDirection += OnPlayerSwipe;
        camShake = FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();
        GameManager.Instance.GivePlayerReward(20); // starting reward
    }
    private void Start()
    {
        levelStartPos = transform.position;
        animationControllerScript = GetComponent<SetDirectionalAnimations>();
        if (!animationControllerScript)
            Debug.Log("In order to use animations on this, you need a SetDirectionalAnimations script", this);
        grid = FixToGrid.GeneralGrid;
    }
    private Vector2 GetFuturePos(Vector2 dir)
    {
        // cast a ray only against walls
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 100f, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            // move to cell just before the wall
            Vector3 stopPos = hit.point - 0.5f * GameManager.Instance.GetGridSize() * dir;
            Vector3Int cell = grid.WorldToCell(stopPos);
            //Debug.Log("Wall hit at " + hit.point + " " + GameManager.Instance.GetGridSize() + " " + stopPos + " " + hit.transform.gameObject.name);
            return grid.GetCellCenterWorld(cell);
        }
        else
        {
            // nothing hit → move to max distance
            Debug.LogError("No wall hit");
            return Vector2.zero;
        }
    }
    private void OnPlayerSwipe(Vector2 dir)
    {
        Vector3 targetPos = GetFuturePos(dir);
        if (Vector3.Distance(targetPos, transform.position) < 0.1f)
        {
            return;
        }
        //Animations
        if (animationControllerScript)
            animationControllerScript.SetDirection(dir);
        isMoving = true;
        prevPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameManager.Instance.PlayerMadeAMove?.Invoke();
        moveTween = transform.DOMove(targetPos, movementSpeed)
            .SetEase(Ease.InSine)
            .SetSpeedBased(true)
            .OnComplete(() =>
            {
                isMoving = false;
                camShake.FrequencyGain = 100;
                camShake.AmplitudeGain = .25f;
                StartCoroutine(GameManager.DoWithDelay(.3f, () =>
                {
                    camShake.AmplitudeGain = 0;
                    camShake.FrequencyGain = 0;
                }));
            })
            .OnUpdate(() =>
            {
                GameManager.Instance.GivePlayerReward(-4f * Time.deltaTime);
            });
    }
    private void Update()
    {
        CheckSwipeLogic();
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
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnSwipeDirection?.Invoke(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnSwipeDirection?.Invoke(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnSwipeDirection?.Invoke(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnSwipeDirection?.Invoke(Vector2.right);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Reward"))
        {
            Debug.Log("Collided with reward");
            moveTween?.Kill(false);
            transform.position = collision.transform.position;
           
            //transform.DOScale(Vector3.one * 1.2f, 0.1f).SetLoops(-1);
            collision.gameObject.SetActive(false);
            canMove = false;
            GameManager.Instance.GivePlayerReward(20);
            GameManager.Instance.FinishLevel(true);
        }
        if (collision.gameObject.CompareTag("TinyReward"))
        {
            var animator = collision.GetComponentInChildren<Animator>();
            
            Debug.Log("Collided with tiny reward");
            animator.SetTrigger("PickUpReward");
            Destroy(collision.gameObject, 1f);
            GameManager.Instance.OnTinyReward?.Invoke();
            GameManager.Instance.GivePlayerReward(1);
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            Debug.Log("Collided with spike");
            if(collision.gameObject.TryGetComponent<SpikeBehaviour>(out SpikeBehaviour spike))
            {
                Debug.Log(spike.IsOpen);
                if (spike.IsOpen)
                {
                    collision.gameObject.SetActive(false);
                    GameManager.Instance.OnHitSpike?.Invoke(true);
                    GameManager.Instance.GivePlayerReward(2);
                }
                else
                {
                    moveTween?.Kill(false);
                    transform.position = levelStartPos;
                    // Only used in tutorial but easiest to add here
                    if (tutorialDeathText)
                        tutorialDeathText.SetActive(true);
                    GameManager.Instance.OnHitSpike?.Invoke(false);
                    isMoving = false;
                }
            }
        }
    }
}
