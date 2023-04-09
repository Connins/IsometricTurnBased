using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private Vector3 moveDirection;
    [SerializeField] private float dampenHorizontalCameraSpeed;
    [SerializeField] private float rotationDuration;

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
        Rotate();
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

    private void Rotate()
    {
        if (rotateClockwise && !coroutineActive)
        {
            StartCoroutine(RotateOverTime(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y + 90, 0), rotationDuration));
        }
        if (rotateAntiClockwise && !coroutineActive)
        {
            StartCoroutine(RotateOverTime(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y - 90, 0), rotationDuration));
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
