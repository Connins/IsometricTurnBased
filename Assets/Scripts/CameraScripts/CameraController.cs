using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float dampenHorizontalCameraSpeed;
    [SerializeField] private float rotationDuration;
    private float angleOffset = 0.001f;

    bool rotateClockwise;
    bool rotateAntiClockwise;
    private bool coroutineActive;
    // Start is called before the first frame update
    void Start()
    {
        coroutineActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        Move();
        
        if (Input.GetKey(KeyCode.LeftControl))
        {
            Rotate();
        }
        else
        {
            SnapRotate();
        }
    }
    

    private void HandleInput()
    {
        float horInput = Input.GetAxis("Horizontal");
        float verInput = Input.GetAxis("Vertical");
        rotateClockwise = Input.GetKeyDown(KeyCode.Q);
        rotateAntiClockwise = Input.GetKeyDown(KeyCode.E);

        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;

        Vector3 forwardRelative = verInput * camForward;
        Vector3 rightRelative = horInput * camRight * dampenHorizontalCameraSpeed;

        moveDirection = forwardRelative + rightRelative;
    }



    private void Move()
    {
        transform.position += moveDirection * Time.deltaTime * moveSpeed;
    }

    private void SnapRotate()
    {
        if (rotateClockwise && !coroutineActive)
        {
            StartCoroutine(RotateOverTime(transform.rotation, SnapToNearest90Degrees(transform.rotation * Quaternion.Euler(0, 45 + angleOffset, 0)), rotationDuration));
        }
        if (rotateAntiClockwise && !coroutineActive)
        {
            StartCoroutine(RotateOverTime(transform.rotation, SnapToNearest90Degrees(transform.rotation * Quaternion.Euler(0, -45 - angleOffset, 0)), rotationDuration));
        }
    }

    private Quaternion SnapToNearest90Degrees(Quaternion currentRotation)
    {
        Vector3 euler = currentRotation.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90) * 90;
        euler.y = Mathf.Round(euler.y / 90) * 90;
        euler.z = Mathf.Round(euler.z / 90) * 90;
        return Quaternion.Euler(euler);
    }

    private void Rotate()
    {
        // Calculate the rotation amount based on the input and speed
        float rotationAmount = rotationSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, rotationAmount);
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up, -rotationAmount);
        }
    }

    IEnumerator RotateOverTime(Quaternion originalRotation, Quaternion finalRotation, float duration)
    {
        coroutineActive = true;
        if (duration > 0f)
        {
            float startTime = Time.time;
            float endTime = startTime + duration;
            transform.rotation = originalRotation;
            yield return null;
            while (Time.time < endTime)
            {
                float progress = (Time.time - startTime) / duration;
                // progress will equal 0 at startTime, 1 at endTime.
                transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, progress);
                yield return null;
            }
        }
        transform.rotation = finalRotation;
        coroutineActive = false;
    }
}
