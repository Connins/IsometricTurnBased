using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform follow;

    [SerializeField] private float moveSpeed = 10f;

    float horInput;
    float verInput;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        Move();
    }
    private void HandleInput()
    {
        horInput = Input.GetAxis("Horizontal");
        verInput = Input.GetAxis("Vertical");
    }

    private void Move()
    {
        Vector3 Vec = transform.localPosition;
        Vec.x += horInput * Time.deltaTime * moveSpeed;
        Vec.y += verInput * Time.deltaTime * moveSpeed;
        transform.localPosition = Vec;
    }
}
