using UnityEngine;

public class AppleState : INodeState
{
    private Color color = new Color32(0x0, 0xFF, 0x00, 0xFF);
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