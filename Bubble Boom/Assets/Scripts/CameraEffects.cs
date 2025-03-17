using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    private CameraController camController;
    [SerializeField] private Vector3 inMenuPosition;
    [SerializeField] private Quaternion inMenuRotation;
    [Space]
    [SerializeField] private Vector3 inGamePosition;
    [SerializeField] private Quaternion inGameRotation;

    [Header ("Camera Shake")]
    [Range(0.01f, 5f)]
    [SerializeField] private float shakeMagnitude;
    [Range(0.1f, 3f)]
    [SerializeField] private float shakeDuration;

    private void Awake()
    {
        camController = GetComponent<CameraController>();
    }

    private void  Start()
    {   
        SwitchToMenuView();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SwitchToMenuView();

        if(Input.GetKeyDown(KeyCode.Alpha2))
            SwitchToGameView();

        if(Input.GetKeyDown(KeyCode.V))
            ScreenShake(shakeDuration, shakeMagnitude);
    }

    public void ScreenShake(float newshakeDuration, float newshakeMagnitude)
    {
        StartCoroutine(ScreenShakeVFX(newshakeDuration, newshakeMagnitude));
    }

    public void SwitchToMenuView()
    {
        StartCoroutine(ChangePositionAndRotation(inMenuPosition, inMenuRotation));
        camController.AdjustPitchValue(inMenuRotation.eulerAngles.x);
    }

    public void SwitchToGameView()
    {
        StartCoroutine(ChangePositionAndRotation(inGamePosition, inGameRotation));
        camController.AdjustPitchValue(inGameRotation.eulerAngles.x);
    }

    private IEnumerator ChangePositionAndRotation(Vector3 targetPosition, Quaternion targetRotation, float duration = 3, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        camController.EnableCameraControlls(false);

        float time = 0;

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time/duration);
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, time/duration);

            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
        camController.EnableCameraControlls(true);
    }

    private IEnumerator ScreenShakeVFX(float duration, float magnitude)
    {
        Vector3 originalPosition = camController.transform.position;
        float elapsed = 0;

        while(elapsed < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            camController.transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        camController.transform.position = originalPosition;
    }
}
