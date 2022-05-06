using UnityEngine;
using UnityEngine.Assertions;


public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int seed = 0;
    [SerializeField] private int scale = 1;


    [SerializeField] private NodeSpawner nodeSpawner;
    [SerializeField] private CameraController mainCamera;

    [SerializeField] private GameObject universePrefab;

    private void Awake()
    {
        Assert.IsNotNull(mainCamera);
        Assert.IsNotNull(nodeSpawner);
        Assert.IsNotNull(universePrefab);
    }

    private void Start()
    {
        if (seed != 0) Random.InitState(seed);

        // generating map "terrain"
        nodeSpawner.GenerateNodes(scale);

        // determining map bounds and setting up background
        (Vector2 mapMidPoint, float mapRadius) = nodeSpawner.GetMapBounds();
        mainCamera.Init(mapMidPoint, mapRadius);

        GameObject universeObject = Instantiate(universePrefab, mapMidPoint, Quaternion.identity);
        universeObject.transform.localScale *= mapRadius;
    }
}