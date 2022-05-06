using UnityEngine;
using UnityEngine.Assertions;


public class SnakeController : MonoBehaviour
{
    private const string NODE_TAG = "Node";

    [SerializeField] private Camera mainCamera;
    private Vector2 forwardVector;

    /// TODO:
    private float jumpRange;


    private void Awake()
    {
        Assert.IsNotNull(mainCamera);
        forwardVector = mainCamera.transform.forward;
    }

    public void Init(float maxNodeDistance)
    {
        jumpRange = maxNodeDistance;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInformation = Physics2D.Raycast(clickPosition, forwardVector);

            if (hitInformation.collider != null && hitInformation.collider.CompareTag(NODE_TAG))
            {
                GameNode touchedNode = hitInformation.transform.gameObject.GetComponent<GameNode>();
                if (touchedNode != null)
                {
                    touchedNode.EnterNode();
                }
            }
        }
    }
}
