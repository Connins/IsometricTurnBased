using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform follow;

    [SerializeField] private float moveSpeed = 10f;

    [SerializeField] private float cameraMoveSize = 35;
    private int angle = 0;
    private List<Vector3> targetPositon = new List<Vector3>();

    float horInput;
    float verInput;
    // Start is called before the first frame update
    void Start()
    {
        SetTargetPositoins();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        Move();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(false);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(true);
        }
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

    private void Rotate(bool clockWise)
    {
        Quaternion targetRotation;
        int Rotate;
        float smooth = 5f;
        if (clockWise) 
        {
            angle = angle + 1;
            if (angle == 4)
            {
                angle = 0;
            }
            transform.parent.Translate(targetPositon[angle], Space.World);

            targetRotation = Quaternion.Euler(0,  90, 0);
            Rotate = 90;
        }
        else
        {
            transform.parent.Translate(-targetPositon[angle], Space.World);

            angle = angle - 1;
            if (angle == -1)
            {
                angle = 3;
            }
            targetRotation = Quaternion.Euler(0, -90, 0);
            Rotate = -90;
        }
        
        
        //transform.parent.rotation = Quaternion.Slerp(transform.localToWorldMatrix.rotation, targetRotation, Time.deltaTime * smooth);
        //transform.parent.position = Vector3.Slerp(transform.parent.position, targetPositon[angle], Time.deltaTime * smooth);
        transform.parent.Rotate(0,Rotate,0, Space.World);
    }

    private void SetTargetPositoins()
    {
        targetPositon.Add(new Vector3(-cameraMoveSize, 0, 0));
        targetPositon.Add(new Vector3(0, 0, cameraMoveSize));
        targetPositon.Add(new Vector3(cameraMoveSize, 0, 0));
        targetPositon.Add(new Vector3(0, 0, -cameraMoveSize));
    }
}
