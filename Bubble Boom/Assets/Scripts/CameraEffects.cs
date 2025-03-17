using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private Vector3 inMenuPosition;
    [SerializeField] private Quaternion inMenuRotation;
    [Space]
    [SerializeField] private Vector3 inGamePosition;
    [SerializeField] private Quaternion inGameRotation;


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
    }

    public void SwitchToMenuView()
    {
        StartCoroutine(ChangePositionAndRotation(inMenuPosition, inMenuRotation));
    }

    public void SwitchToGameView()
    {
        StartCoroutine(ChangePositionAndRotation(inGamePosition, inGameRotation));
    }

    private IEnumerator ChangePositionAndRotation(Vector3 targetPosition, Quaternion targetRotation, float duration = 3, float delay = 0)
    {
        yield return new WaitForSeconds(delay);

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
    }
}
