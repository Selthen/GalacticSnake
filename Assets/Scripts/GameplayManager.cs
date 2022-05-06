using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class GameplayManager : MonoBehaviour
{
    private List<GameNode> freeNodes;
    private Queue<GameNode> snakeNodes;
    private LineRenderer snakeBody;
    private GameNode appleNode;
    private GameNode snakeHead;

    public static event System.Action OnSnakeDeath;

    private void OnEnable()
    {
        FreeState.OnEnter += HandleFreeNodeEnter;
        SnakeState.OnEnter += HandleSnakeNodeEnter;
        AppleState.OnEnter += HandleAppleNodeEnter;
        /// TODO:
        OnSnakeDeath += () => Debug.Log("Snake died");
    }
    private void OnDisable()
    {
        FreeState.OnEnter -= HandleFreeNodeEnter;
        SnakeState.OnEnter -= HandleSnakeNodeEnter;
        AppleState.OnEnter -= HandleAppleNodeEnter;
    }

    public void Init(GameNode[] nodes, float maxDistance)
    {
        freeNodes = new List<GameNode>(nodes);
        snakeNodes = new Queue<GameNode>();
        snakeBody = GetComponent<LineRenderer>();

        // initialize first node
        GameNode newNode = AddRandomSnakeNode(null, maxDistance);

        // initialize another node
        AddRandomSnakeNode(newNode, maxDistance);

        // spawn first apple
        SpawnApple();
        appleNode.ChangeState(appleNode.appleState);
    }

    private GameNode AddRandomSnakeNode(GameNode previousNode, float radiusOfAcceptedNodes)
    {
        int randomFreeNodeIdx = Random.Range(0, freeNodes.Count);
        GameNode newNode = freeNodes[randomFreeNodeIdx];

        if (previousNode != null)
        {
            while (Vector2.Distance(previousNode.transform.position, newNode.transform.position) > radiusOfAcceptedNodes)
            {
                randomFreeNodeIdx = Random.Range(0, freeNodes.Count);
                newNode = freeNodes[randomFreeNodeIdx];
            }
        }

        MoveSnakeHead(newNode);
        return newNode;
    }

    private void MoveSnakeHead(GameNode target)
    {
        snakeNodes.Enqueue(target);
        freeNodes.Remove(target);

        target.ChangeState(target.snakeState);

        UpdateSnakeBody(target);
        snakeHead = target;
    }

    private void MoveSnakeTail()
    {
        GameNode tailNode = snakeNodes.Dequeue();
        freeNodes.Add(tailNode);
        tailNode.ChangeState(tailNode.freeState);
    }

    private void UpdateSnakeBody(GameNode newHeadNode)
    {
        snakeBody.positionCount = snakeNodes.Count;
        int i = 0;
        GameNode previousNode = null;

        foreach (var node in snakeNodes)
        {
            // visual update
            snakeBody.SetPosition(i++, node.transform.position);

            // intersection check
            if (previousNode != null && DoSegmentsIntersect(snakeHead, newHeadNode, previousNode, node) 
                && node != snakeHead && node != newHeadNode)
            {
                OnSnakeDeath?.Invoke();
            }
            previousNode = node;
        }
    }

    private void SpawnApple()
    {
        int randomFreeNodeIdx = Random.Range(0, freeNodes.Count);
        appleNode = freeNodes[randomFreeNodeIdx];
        freeNodes.RemoveAt(randomFreeNodeIdx);

        appleNode.ChangeState(appleNode.appleState);
    }

    private void HandleFreeNodeEnter(GameNode enteredNode)
    {
        MoveSnakeTail();
        MoveSnakeHead(enteredNode);
    }

    private void HandleAppleNodeEnter(GameNode enteredNode)
    {
        MoveSnakeHead(enteredNode);
        SpawnApple();
    }

    private void HandleSnakeNodeEnter(GameNode enteredNode)
    {
        if (enteredNode != snakeHead)
        {
            MoveSnakeTail();
            MoveSnakeHead(enteredNode);

            if (enteredNode != snakeHead) 
            {
                OnSnakeDeath?.Invoke();
            } 
        }
    }

    // checks intersection of segments p1,p2 and q1,q1
    private bool DoSegmentsIntersect(MonoBehaviour p1, MonoBehaviour p2, MonoBehaviour q1, MonoBehaviour q2)
    {
        return Orientation(p1.transform.position, p2.transform.position, q1.transform.position)
                != Orientation(p1.transform.position, p2.transform.position, q2.transform.position)
               && Orientation(q1.transform.position, q2.transform.position, p1.transform.position)
                != Orientation(q1.transform.position, q2.transform.position, p2.transform.position);
    }

    // standard algorithm for 2D orientation
    private int Orientation(Vector2 p, Vector2 q, Vector2 r)
    {
        float value = (q.y - p.y) * (r.x - q.x) -
                (q.x - p.x) * (r.y - q.y);

        if (value > 0) return 1; // clockwise
        else if (value < 0) return -1; // counterclockwise
        else return 0; // collinear
    }
}
