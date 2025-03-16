using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 120;


    [Header ("Rotation")]
    [SerializeField] private Transform focusPoint;
    [SerializeField] private float maxfocusPointDistance = 15;
    [SerializeField] private float roationSpeed = 120;
    [Space]
    private float pitch;
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 85f;

    private float smoothTime = .1f;
    private Vector3 movementVelocity = Vector3.zero;
    

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleMovement();

        focusPoint.position = transform.position + (transform.forward * GetFocusDistance());
       
    }

    private float GetFocusDistance()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxfocusPointDistance))
            return hit.distance;
        
        return maxfocusPointDistance;

    }

    private void HandleRotation()
    {
        if(Input.GetMouseButton(1))
        {
            float horizontalRotation = Input.GetAxis("Mouse X") * roationSpeed * Time.deltaTime;
            float verticalRotation = Input.GetAxis("Mouse Y") * roationSpeed * Time.deltaTime;

            pitch = Mathf.Clamp(pitch - verticalRotation, minPitch, maxPitch);

            transform.RotateAround(focusPoint.position, Vector3.up, horizontalRotation);
            transform.RotateAround(focusPoint.position, transform.right, pitch - transform.eulerAngles.x);

            transform.LookAt(focusPoint);
        }
    }

    private void HandleMovement()
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
