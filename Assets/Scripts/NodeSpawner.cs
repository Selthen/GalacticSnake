using UnityEngine;


public class NodeSpawner : MonoBehaviour
{
    private const float WHOLE_ANGLE = 2 * Mathf.PI;

    [SerializeField] private int nodesPerScalePoint;
    [SerializeField] private float minNodeDistance;
    [SerializeField] private float maxNodeDistance;
    [SerializeField] private float mapBorderOffset;

    [SerializeField] private GameNode gameNodePrefab;

    private int nodesNumber;
    private GameNode[] nodes;
    private Vector2 mapMidPoint = Vector2.zero;

    public GameNode[] Nodes => nodes;

    public void GenerateNodes(int scale)
    {
        nodesNumber = scale * nodesPerScalePoint;
        nodes = new GameNode[nodesNumber];

        Vector2[] nodePositions = new Vector2[nodesNumber];
        nodePositions[0] = Vector2.zero;

        // generate positions
        for (int i = 1; i < nodesNumber; i++)
        {
            // do-while loop in rare case, where every alternative position failed
            do { } while (!TryAddingNewPosition(nodePositions, i));
        }

        mapMidPoint /= nodesNumber;

        // spawn nodes at positions
        for (int i = 0; i < nodesNumber; i++)
        {
            nodes[i] = Instantiate(gameNodePrefab, nodePositions[i], Quaternion.identity);
        }
    }
    
    public (Vector2 mapMidPoint, float mapRadius) GetMapBounds()
    {
        float maxDistToCenter = 0;

        foreach (var node in nodes)
        {
            float potentialMaxDistance = Vector2.Distance(mapMidPoint, node.transform.position);

            if (potentialMaxDistance > maxDistToCenter)
            {
                maxDistToCenter = potentialMaxDistance;
            }
        }

        return (mapMidPoint, maxDistToCenter + mapBorderOffset);
    }

    public float GetMaxNodeDistance()
    {
        return maxNodeDistance;
    }

    private bool TryAddingNewPosition(Vector2[] nodePositions, int i)
    {
        int randomNodeIdx = Random.Range(0, i);
        float distanceToNewNode = Random.Range(minNodeDistance, maxNodeDistance);

        float nextNodeSpawnTryAngle = CalculateAngleOfMaxNumberOfCirclesOnRing(distanceToNewNode);
        int possibleSpawnTries = CalculateMaxNumberOfCirclesOnRing(nextNodeSpawnTryAngle);

        for (int j = 0; j < possibleSpawnTries; j++)
        {
            Vector2 newNodePosition = nodePositions[randomNodeIdx] + OnCircle(distanceToNewNode, j * nextNodeSpawnTryAngle);

            if (CheckPositionAndGetMaxDistance(nodePositions, i, newNodePosition))
            {
                mapMidPoint += newNodePosition;
                nodePositions[i] = newNodePosition;
                return true;
            }
        }

        return false;
    }

    private bool CheckPositionAndGetMaxDistance(Vector2[] nodePositions, int existigPositionsNumber, Vector2 newNodePosition)
    {
        for (int i = 0; i < existigPositionsNumber; i++)
        {
            float distance = Vector2.Distance(nodePositions[i], newNodePosition);
                
            if (distance < minNodeDistance)
            {
                return false;
            }
        }

        return true;
    }

    private Vector2 OnCircle(float radius = 1.0f, float angleOffset = 0f)
    {
        float angle = angleOffset + Random.Range(0f, WHOLE_ANGLE);
        float x = radius * Mathf.Cos(angle);
        float y = radius * Mathf.Sin(angle);

        return new Vector2(x, y);
    }

    private int CalculateMaxNumberOfCirclesOnRing(float maxNumberOfCirclesAngle)
    {
        return Mathf.FloorToInt(WHOLE_ANGLE / maxNumberOfCirclesAngle);
    }

    private float CalculateAngleOfMaxNumberOfCirclesOnRing(float ringRadius)
    {
        return 2 * Mathf.Asin(minNodeDistance / ringRadius);
    }
}
