using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileAnimator : MonoBehaviour
{
    [SerializeField] private float yMovementDuration = .1f;

    private float yTestOffset = .25f;
    public Transform testObject;

    [ContextMenu("Move Tile")]
    public void TestMovementObject()
    {
        Vector3 targetPosition = testObject.position + new Vector3(0, yTestOffset);
        MoveTile(testObject, targetPosition);
    }

    public void MoveTile(Transform objectMove, Vector3 targetPosition)
    {
        StartCoroutine(MoveTileCo(objectMove, targetPosition));
    }

    private IEnumerator MoveTileCo(Transform objectToMove, Vector3 targetPosition)
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
}
