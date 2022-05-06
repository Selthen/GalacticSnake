using UnityEngine;

public class SnakeState : INodeState
{
    private Color color = new Color32(0xFF, 0x0, 0x00, 0xFF);
    public static event System.Action<GameNode> OnEnter;

    public void EnterNode(GameNode node)
    {
        OnEnter?.Invoke(node);
    }

    public void InitState(GameNode node)
    {
        node.ChangeColor(color);
    }
}