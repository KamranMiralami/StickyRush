using DG.Tweening;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class AgentManager : Agent
{
    //Animations
    SetDirectionalAnimations animationControllerScript;

    public Action<Vector2> OnSwipeDirection;
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float agentDecisionDelay = 1f;
    Grid grid;
    Tween moveTween;
    FixToGrid fixToGrid;
    Vector3 initialPosition;
    GameObject reward;
    Vector3 prevPos;
    protected override void Awake()
    {
        base.Awake();
        OnSwipeDirection += OnPlayerSwipe;
        fixToGrid = GetComponent<FixToGrid>();
        initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
    private void Start()
    {
        animationControllerScript = GetComponent<SetDirectionalAnimations>();
        if (!animationControllerScript)
            Debug.Log("In order to use animations on this, you need a SetDirectionalAnimations script", this);
        grid = FixToGrid.GeneralGrid;
        reward = GameManager.Instance.Reward;
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
            //Debug.LogError("No wall hit");
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
        prevPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        moveTween = transform.DOMove(targetPos, movementSpeed)
            .SetEase(Ease.InSine)
            .SetSpeedBased(true)
            .OnComplete(() =>
            {
                StartCoroutine(GameManager.DoWithDelay(.5f, () =>
                {
                    agentDecisionDelay = 0;
                }));
            });
    }
    private void Update()
    {
        if (agentDecisionDelay > 0)
            agentDecisionDelay -= Time.deltaTime;
        else
        {
            if (moveTween == null || !moveTween.IsActive() || moveTween.IsComplete())
            {
                //Debug.Log("AI should do something now");
                RequestDecision();
                SetReward(-10f);
                agentDecisionDelay = 5f;
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Reward"))
        {
            //Debug.Log("Collided with reward");
            moveTween?.Kill(false);
            transform.position = collision.transform.position;
            transform.DOScale(Vector3.one * 1.2f, 0.1f).SetLoops(-1);
            transform.DOKill();
            SetReward(1000f);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("TinyReward"))
        {
            Debug.Log("Collided with tiny reward");
            collision.gameObject.SetActive(false);
            SetReward(100f);
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            Debug.Log("Collided with spike");
            if (collision.gameObject.TryGetComponent<SpikeBehaviour>(out SpikeBehaviour spike))
            {
                if (spike.IsOpen())
                {
                    collision.gameObject.SetActive(false);
                    SetReward(200f);
                }
                else
                {
                    moveTween?.Kill(false);
                    transform.position = prevPos;
                    collision.gameObject.SetActive(false);
                    SetReward(-200f);
                    moveTween = null;
                }
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
        // Move the agent using the action.
        var dir = ParseActionToVector2(actionBuffers.DiscreteActions);
        //Debug.Log("AI deciding on action : " + actionBuffers.DiscreteActions[0]);
        OnSwipeDirection?.Invoke(dir);
        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-0.1f);
    }
    public override void OnEpisodeBegin()
    {
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
        if (reward != null)
            dist = Vector3.Distance(transform.position, reward.transform.position);

        sensor.AddObservation(futureLeft);
        sensor.AddObservation(futureRight);
        sensor.AddObservation(futureUp);
        sensor.AddObservation(futureDown);
        sensor.AddObservation(new Vector2(transform.position.x, transform.position.y));
        sensor.AddObservation(dist);
    }
}
