using UnityEngine;


[RequireComponent(typeof(SpriteRenderer))]
public class GameNode : MonoBehaviour
{
    private INodeState currentState;
    public FreeState freeState = new FreeState();
    public SnakeState snakeState = new SnakeState();
    public AppleState appleState = new AppleState();

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentState = freeState;
    }

    public void EnterNode()
    {
        currentState.EnterNode(this);
    }

    public void ChangeState(INodeState newState)
    {
        currentState = newState;
        currentState.InitState(this);
    }

    public void ChangeColor(Color newColor)
    {
        spriteRenderer.color = newColor;
    }
}