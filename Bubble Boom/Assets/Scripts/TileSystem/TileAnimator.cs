using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimator : MonoBehaviour
{
    [SerializeField] private float yMovementDuration = .1f;

    [Header ("Build Slow Movement")]
    [SerializeField] private float buildSlotYoffset = .25f;

    public void MoveTile(Transform objectMove, Vector3 targetPosition)
    {
        StartCoroutine(MoveTileCo(objectMove, targetPosition));
    }

    public IEnumerator MoveTileCo(Transform objectToMove, Vector3 targetPosition)
    {
        float time = 0;
        Vector3 startPosition = objectToMove.position;

        while(time < yMovementDuration)
        {
            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, time / yMovementDuration);

            time += Time.deltaTime;
            yield return null;
        }

        objectToMove.position = targetPosition;
    }

    public float GetBuildOffset() => buildSlotYoffset;
    public float GetTravelDuration() => yMovementDuration;
}
