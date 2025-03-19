using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BuildButtons : MonoBehaviour
{
    [SerializeField] private float yPositionOffset;
    [SerializeField] private float openAnimationDuration = .1f;

    private UI_BuildButtonOnEffect[] buildButtons;

    private bool isBuildMenuActive;
    private UI_Animator uiAnim;

    private void Awake()
    {
        uiAnim = GetComponentInParent<UI_Animator>();
        buildButtons = GetComponentsInChildren<UI_BuildButtonOnEffect>();
    }

   
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
            ShowBuildButtons();
    }


    private void ShowBuildButtons()
    {
        isBuildMenuActive = !isBuildMenuActive;

        float yOffset = isBuildMenuActive ? yPositionOffset : -yPositionOffset;
        float methodDelay = isBuildMenuActive ? openAnimationDuration : 0;

        Vector3 offset = new Vector3(0, yOffset);

        uiAnim.ChangePosition(transform, new Vector3(0, yOffset), openAnimationDuration);
        Invoke(nameof(ToggleButtonMovement), methodDelay);
    }

    private void ToggleButtonMovement()
    {
        foreach(var button in buildButtons)
        {
            button.ToggleMovement(isBuildMenuActive);
        }
    }
}
