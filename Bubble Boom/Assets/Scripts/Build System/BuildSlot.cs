using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private TileAnimator tileAnimator;
    private BuildManager buildManager;
    private Vector3 defaultPosition;
    private bool tileCanBeMoved = true;
    private Coroutine currentMovementCo;

    private void Awake()
    {
        tileAnimator = FindFirstObjectByType<TileAnimator>();
        buildManager = FindFirstObjectByType<BuildManager>();
        defaultPosition = transform.position;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left)
            return;

        buildManager.SelectBuildSlot(this);
        MoveTileUp();

        tileCanBeMoved = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(Input.GetKey(KeyCode.Mouse1) || Input.GetKey(KeyCode.Mouse2))
            return;
            
        if(tileCanBeMoved == false)
            return;

        MoveTileUp();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(tileCanBeMoved == false)
            return;
        
        if(currentMovementCo != null)
        {
            Invoke(nameof(MoveToDefaultPosition), tileAnimator.GetTravelDuration());
        }
        else
            MoveToDefaultPosition();
    }

    public void UnselectTile()
    {
        MoveToDefaultPosition();
        tileCanBeMoved = true;
    }

    private void MoveTileUp()
    {
        Vector3 targetPosition = transform.position + new Vector3(0, tileAnimator.GetBuildOffset(), 0);
        currentMovementCo = StartCoroutine(tileAnimator.MoveTileCo(transform, targetPosition));
    }

    private void MoveToDefaultPosition()
    {
        tileAnimator.MoveTile(transform, defaultPosition);
    }

}
