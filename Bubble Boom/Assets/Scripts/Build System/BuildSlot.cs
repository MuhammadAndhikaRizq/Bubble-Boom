using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private TileAnimator tileAnimator;
    private Vector3 defaultPosition;

    private void Awake()
    {
        tileAnimator = FindFirstObjectByType<TileAnimator>();
        defaultPosition = transform.position;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Tile was selected");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveTileUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MoveToDefaultPosition();
    }

    private void MoveTileUp()
    {
        Vector3 targetPosition = transform.position + new Vector3(0, tileAnimator.GetBuildOffset(), 0);
        tileAnimator.MoveTile(transform, targetPosition);
    }

    private void MoveToDefaultPosition()
    {
        tileAnimator.MoveTile(transform, defaultPosition);
    }

}
