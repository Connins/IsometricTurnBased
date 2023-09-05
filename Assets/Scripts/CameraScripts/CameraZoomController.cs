using UnityEngine;

public class CameraZoomController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 3f;
    private float minSize = 2.0f;
    private float maxSize = 10.0f;
    [SerializeField] private float smoothTime = 0.1f; // Smoothing time

    private float targetSize; // The target size the camera is moving towards
    private float velocity;   // Smoothing velocity

    void Start()
    {
        targetSize = Camera.main.orthographicSize; // Initialize targetSize
    }

    void Update()
    {
        // Get the mouse wheel input
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        // Calculate the new target size based on the scroll input
        targetSize -= scroll * zoomSpeed;

        // Clamp the target size within the specified range
        targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

        // Smoothly interpolate towards the target size
        Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, targetSize, ref velocity, smoothTime);
    }
}
