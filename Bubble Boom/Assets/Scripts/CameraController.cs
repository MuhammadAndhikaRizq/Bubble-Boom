using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class CameraController : MonoBehaviour
{
    [Header ("Movement")]
    [SerializeField] private float movementSpeed = 120;
    [SerializeField] private float mousMovementSpeed = 5;

    [Header ("Edge Movement Details")]
    [SerializeField] private float edgeMovementSpeed = 10;
    [SerializeField] private float edgeTreshold = 10;
    private float screenWidth;
    private float screenHeigth;


    [Header ("Rotation")]
    [SerializeField] private Transform focusPoint;
    [SerializeField] private float maxfocusPointDistance = 15;
    [SerializeField] private float roationSpeed = 120;
    [Space]
    private float pitch;
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 85f;


    [Header ("Zoom")]
    [SerializeField] private float zoomSpeed = 10f;
    [SerializeField] private float minZoom = 3;
    [SerializeField] private float maxZoom = 15;

    private float smoothTime = .1f;
    private Vector3 movementVelocity = Vector3.zero;
    private Vector3 zoomVelocity = Vector3.zero;
    private Vector3 edgeMovementVelocity = Vector3.zero;    
    private Vector3 mouseMovementVelocity = Vector3.zero;
    private Vector3 lastMousePosition;
    
    void Start()
    {
        screenWidth = Screen.width;
        screenHeigth = Screen.height;
    }


    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleZoom();
        HandleEdgeMovement();
        HandleMouseMovement();
        HandleMovement();

        focusPoint.position = transform.position + (transform.forward * GetFocusDistance());
       
    }

    private float GetFocusDistance()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxfocusPointDistance))
            return hit.distance;
        
        return maxfocusPointDistance;

    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;
        Vector3 targetPosition = transform.position + zoomDirection;

        if(transform.position.y < minZoom && scroll > 0)
            return;
        
        if(transform.position.y > maxZoom && scroll < 0)
            return;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref zoomVelocity, smoothTime);
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

        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

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

    private void HandleMouseMovement()
    {
        if(Input.GetMouseButtonDown(2))
        {
            lastMousePosition = Input.mousePosition;
        }

        if(Input.GetMouseButton(2))
        {
            Vector3 positionDifference = Input.mousePosition - lastMousePosition;
            Vector3 moveRight = transform.right * (-positionDifference.x) * mousMovementSpeed * Time.deltaTime;
            Vector3 moveForward = transform.forward * (-positionDifference.y) * mousMovementSpeed * Time.deltaTime;

            moveRight.y = 0;
            moveForward.y = 0;

            Vector3 movement = moveRight + moveForward;
            Vector3 targetPosition = transform.position + movement;

            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref mouseMovementVelocity, smoothTime);
            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleEdgeMovement()
    {
        Vector3 targetPosition = transform.position;
        Vector3 mousePosition = Input.mousePosition;
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        if(mousePosition.x > screenWidth - edgeTreshold)
            targetPosition += transform.right * edgeMovementSpeed * Time.deltaTime;
        
        if(mousePosition.x < edgeTreshold)
            targetPosition -= transform.right *edgeMovementSpeed * Time.deltaTime;

        if(mousePosition.y > screenHeigth - edgeTreshold)
            targetPosition += flatForward * edgeMovementSpeed * Time.deltaTime;
        
        if(mousePosition.y < edgeTreshold)
            targetPosition -= flatForward * edgeMovementSpeed * Time.deltaTime;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref edgeMovementVelocity, smoothTime);
    }
}
