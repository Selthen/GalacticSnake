using UnityEngine;

public class FreeState : INodeState
{
    private Color color = new Color32(0xEE, 0xFF, 0x94, 0xFF);
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