using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private const float ZOOM_SPEED = 0.01f;
    private const int EDITOR_ZOOM_SPEED = 3;

    private Vector3 touchStart;
    private Camera cam;

    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;

    private Vector3 mapMidPoint;
    private float mapRadius;
    private float mapRadiusSqr;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    public void Init(Vector2 mapMidPoint, float mapRadius)
    {
        this.mapMidPoint = mapMidPoint;
        this.mapMidPoint.z = transform.position.z;

        this.mapRadius = mapRadius;
        mapRadiusSqr = Mathf.Pow(mapRadius, 2);
    }

    private void Update()
    {
        // init camera movement
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // if camera movement initialized, move camera
        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = transform.position + direction;

            // camera outside of map circle
            if(Mathf.Pow(newPos.x - mapMidPoint.x, 2) + Mathf.Pow(newPos.y - mapMidPoint.y, 2) > mapRadiusSqr)
            {
                Vector3 relativeVector = newPos - mapMidPoint;
                newPos = mapMidPoint + relativeVector.normalized * mapRadius;
            }

            transform.position = newPos;
        }

        // adjust zoom
#if UNITY_ANDROID
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDistanceSqr = (touchOnePrevPos - touchZeroPrevPos).magnitude;
            float currentTouchDistanceSqr = (touchOne.position - touchZero.position).magnitude;

            float distanceDelta = currentTouchDistanceSqr - prevTouchDistanceSqr;
            Zoom(distanceDelta * ZOOM_SPEED);
        }
#else 
        Zoom(EDITOR_ZOOM_SPEED * Input.GetAxis("Mouse ScrollWheel"));
#endif

    }

    private void Zoom(float zoomDelta)
    {
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomDelta, minZoom, maxZoom);
    }
}