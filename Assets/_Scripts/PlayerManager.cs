using DG.Tweening;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class PlayerManager :  Agent
{
    public static PlayerManager Instance { get; protected set; }
    //Animations
    SetDirectionalAnimations animationControllerScript;
    
    public Action<Vector2> OnSwipeDirection;
    [SerializeField] bool isPlayerControlled;
    [SerializeField] float minSwipeDistance = 10f;
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float agentDecisionDelay = 1f;
    Vector2 startPos;
    Grid grid;
    bool isMoving = false;
    bool canMove = true;
    Tween moveTween;
    FixToGrid fixToGrid;
    Vector3 initialPosition;
    GameObject reward;
    protected override void Awake()
    {
        base.Awake();
        Instance = this;
        OnSwipeDirection += OnPlayerSwipe;
        fixToGrid = GetComponent<FixToGrid>();
        initialPosition = new Vector3(transform.position.x,transform.position.y, transform.position.z);
    }
    private void Start()
    {
        animationControllerScript = GetComponent<SetDirectionalAnimations>();
        if (!animationControllerScript)
            Debug.Log("In order to use animations on this, you need a SetDirectionalAnimations script", this);
        grid = FixToGrid.GeneralGrid;
        reward = GameManager.Instance.Reward;
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (!isPlayerControlled) return;
    }
    private Vector2 GetFuturePos(Vector2 dir)
    {
        // cast a ray only against walls
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 100f, LayerMask.GetMask("Wall"));
        if (hit.collider != null)
        {
            // move to cell just before the wall
            Vector3 stopPos = hit.point - dir * GameManager.Instance.GetGridSize() * 0.5f;
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
            agentDecisionDelay = 0;
            return;
        }
        //Animations
        if (animationControllerScript)
            animationControllerScript.SetDirection(dir);
        isMoving = true;
        moveTween = transform.DOMove(targetPos, movementSpeed)
            .SetEase(Ease.InSine)
            .SetSpeedBased(true)
            .OnComplete(() =>
            {
                StartCoroutine(GameManager.DoWithDelay(.5f, () =>
                {
                    isMoving = false;
                    agentDecisionDelay = 0;
                }));
            });
    }
    private void Update()
    {
        if (isPlayerControlled)
        {
            CheckSwipeLogic();
        }
        else
        {
            if(agentDecisionDelay > 0)
                agentDecisionDelay -= Time.deltaTime;
            else
            {
                if (moveTween == null || !moveTween.IsActive() || moveTween.IsComplete())
                {
                    Debug.Log("AI should do something now");
                    RequestDecision();
                    agentDecisionDelay = 5f;
                }
            }
        }
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
            collision.gameObject.SetActive(false);
            canMove = false;
            if (!isPlayerControlled)
            {
                canMove = true;
                transform.DOKill();
                collision.gameObject.SetActive(true);
                SetReward(1000f);
                EndEpisode();
            }
        }
    }
    Vector2 ParseActionToVector2(ActionSegment<int> actions)
    {
        int dir = actions[0]; // 0-left, 1-right, 2-up, 3-down
        Vector2 move = Vector2.zero;
        switch (dir)
        {
            case 0:
                move = Vector2.left;
                break;
            case 1:
                move = Vector2.right;
                break;
            case 2:
                move = Vector2.up;
                break;
            case 3:
                move = Vector2.down;
                break;
            default:
                Debug.LogError("Invalid action");
                break;
        }
        return move;
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (isPlayerControlled) return;
        // Move the agent using the action.
        var dir = ParseActionToVector2(actionBuffers.DiscreteActions);
        Debug.Log("AI deciding on action : " + actionBuffers.DiscreteActions[0]);
        OnSwipeDirection?.Invoke(dir);
        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-0.1f);
    }
    public override void OnEpisodeBegin()
    {
        if (isPlayerControlled) return;
        Debug.Log("Begining Episode");
        transform.position = initialPosition;
        fixToGrid.SnapToGrid();
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        var futureLeft = GetFuturePos(Vector2.left);
        var futureRight = GetFuturePos(Vector2.right);
        var futureUp = GetFuturePos(Vector2.up);
        var futureDown = GetFuturePos(Vector2.down);
        float dist = float.MaxValue;
        if(reward != null)
            dist = Vector3.Distance(transform.position, reward.transform.position);

        sensor.AddObservation(futureLeft);
        sensor.AddObservation(futureRight);
        sensor.AddObservation(futureUp);
        sensor.AddObservation(futureDown);
        sensor.AddObservation(new Vector2(transform.position.x,transform.position.y));
        sensor.AddObservation(dist);
    }
}
