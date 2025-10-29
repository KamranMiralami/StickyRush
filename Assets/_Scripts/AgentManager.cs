using DG.Tweening;
using System;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
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
    [SerializeField] LayerMask notPlayerMask;
    [SerializeField] public bool isRandom;
    [SerializeField] GameObject reward;
    Vector2 levelStartPos;
    HashSet<Vector2> visitedLocations;

    bool canMove = true;
    List<GameObject> collectedRewards;
    protected override void Awake()
    {
        base.Awake();
        levelStartPos = transform.position;
        collectedRewards = new();
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
        if(!canMove)
            return;
        if (agentDecisionDelay > 0)
            agentDecisionDelay -= Time.deltaTime;
        else
        {
            if (moveTween == null || !moveTween.IsActive() || moveTween.IsComplete())
            {
                //Debug.Log("AI should do something now");
                if (isRandom)
                {
                    int random = UnityEngine.Random.Range(0, 4);
                    Vector2 dir = Vector2.zero;
                    switch (random)
                    {
                        case 0:
                            dir = Vector2.left;
                            break;
                        case 1:
                            dir = Vector2.right;
                            break;
                        case 2:
                            dir = Vector2.up;
                            break;
                        case 3:
                            dir = Vector2.down;
                            break;
                    }
                    OnPlayerSwipe(dir);
                }
                else
                {
                    RequestDecision();
                }
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
            AddReward(100f);
            EndEpisode();
            canMove = false;
            Debug.Log("AI finished level");
            GameManager.Instance.FinishLevel(false);
        }
        if (collision.gameObject.CompareTag("TinyReward"))
        {
            //Debug.Log("Collided with tiny reward");
            collision.gameObject.SetActive(false);
            collectedRewards.Add(collision.gameObject);
            AddReward(10f);
        }
        if (collision.gameObject.CompareTag("Spike"))
        {
            //Debug.Log("Collided with spike");
            if (collision.gameObject.TryGetComponent<SpikeBehaviour>(out SpikeBehaviour spike))
            {
                if (spike.IsOpen)
                {
                    collision.gameObject.SetActive(false);
                    AddReward(20f);
                }
                else
                {
                    moveTween?.Kill(false);
                    transform.position = levelStartPos;
                    visitedLocations = new();
                    collision.gameObject.SetActive(false);
                    AddReward(-20f);
                    moveTween = null;
                }
                collectedRewards.Add(collision.gameObject);
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
        //Debug.Log("AI deciding on action : " + dir);
        var visitingLoc = new Vector2(transform.position.x, transform.position.y);
        visitedLocations.Add(visitingLoc);
        StartCoroutine(GameManager.DoWithDelay(3f, () =>
        {
            visitedLocations.Remove(visitingLoc);
        }));
        OnSwipeDirection?.Invoke(dir);
        // Penalty given each step to encourage agent to finish task quickly.
        Vector2 currentPos = new(transform.position.x, transform.position.y);
        var parentT = (Vector2)transform.parent.transform.position;
        currentPos -= parentT;
        var dif = Vector2.Distance(currentPos, (Vector2)reward.transform.position - parentT);
        AddReward(dif * -1f);
    }
    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        var futureLeft = GetFuturePos(Vector2.left);
        var futureRight = GetFuturePos(Vector2.right);
        var futureUp = GetFuturePos(Vector2.up);
        var futureDown = GetFuturePos(Vector2.down);
        var possibleActions = 4;
        if (futureLeft == (Vector2)transform.position)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 0, false); // disable futureLeft
        }
        if (futureRight == (Vector2)transform.position)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 1, false); // disable futureRight
        }
        if (futureUp == (Vector2)transform.position)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 2, false); // disable futureUp
        }
        if (futureDown == (Vector2)transform.position)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 3, false); // disable futureDown
        }
        var sawUp = visitedLocations.Contains(futureUp);
        var sawDown = visitedLocations.Contains(futureDown);
        var sawLeft = visitedLocations.Contains(futureLeft);
        var sawRight = visitedLocations.Contains(futureRight);
        if((sawLeft && sawUp && sawRight && sawDown) || possibleActions <=1)
        {
            // all directions have been visited, do not disable anything
            return;
        }
        if (sawDown && possibleActions > 1)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 3, false); // disable futureDown
        }
        if (sawLeft && possibleActions > 1)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 0, false); // disable futureLeft
        }
        if (sawUp && possibleActions > 1)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 2, false); // disable futureUp
        }
        if (sawRight && possibleActions > 1)
        {
            possibleActions--;
            actionMask.SetActionEnabled(0, 1, false); // disable futureRight
        }
    }
    public override void OnEpisodeBegin()
    {
        //Debug.Log("Begining Episode");
        visitedLocations = new HashSet<Vector2>();
        transform.position = initialPosition;
        fixToGrid.SnapToGrid();
        canMove = true;
        foreach(var obj in collectedRewards)
        {
            obj.SetActive(true);
        }
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        var parentT = (Vector2)transform.parent.transform.position;
        var futureLeft = GetFuturePos(Vector2.left);
        var futureRight = GetFuturePos(Vector2.right);
        var futureUp = GetFuturePos(Vector2.up);
        var futureDown = GetFuturePos(Vector2.down);
        futureDown -= parentT;
        futureUp -= parentT;
        futureRight -= parentT;
        futureLeft -= parentT;
        Vector2 currentPos = new(transform.position.x, transform.position.y);
        currentPos -= parentT;

        //Safety (4)
        sensor.AddObservation(CheckSafetyValue(currentPos, Vector2.left));
        sensor.AddObservation(CheckSafetyValue(currentPos, Vector2.right));
        sensor.AddObservation(CheckSafetyValue(currentPos, Vector2.up));
        sensor.AddObservation(CheckSafetyValue(currentPos, Vector2.down));

        //Positions (6 * 3)
        sensor.AddObservation(futureLeft);
        sensor.AddObservation(futureRight);
        sensor.AddObservation(futureUp);
        sensor.AddObservation(futureDown);
        sensor.AddObservation(currentPos);
        sensor.AddObservation((Vector2)reward.transform.position - parentT);

        //Distance to reward (1)
        sensor.AddObservation(Vector2.Distance(currentPos, (Vector2)reward.transform.position - parentT));
    }

    int CheckSafetyValue(Vector2 currentPos, Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(currentPos, dir, 1000, notPlayerMask);
        if (hit.transform == null)
        {
            return 0;
        }
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Reward"))
            return 100;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("TinyReward"))
            return 10;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Trap") && hit.collider.gameObject.TryGetComponent<SpikeBehaviour>(out SpikeBehaviour spike))
            if (spike.IsOpen)
                return 20;
        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
            return 0;
        return 0;
    }
}
