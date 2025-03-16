using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 120;

    private float smoothTime = .1f;
    private Vector3 movementVelocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    public void HandleMovement()
    {
        Vector3 targetPosition = transform.position;

        float  vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up). normalized;

        if(vInput > 0)
            targetPosition += flatForward * movementSpeed * Time.deltaTime;
        if(vInput < 0)
            targetPosition -= flatForward * movementSpeed * Time.deltaTime;

        if(hInput > 0)
            targetPosition += transform.right * movementSpeed * Time.deltaTime;
        if(hInput < 0)
            targetPosition -= transform.right * movementSpeed * Time.deltaTime;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref movementVelocity,smoothTime);
    }
}
