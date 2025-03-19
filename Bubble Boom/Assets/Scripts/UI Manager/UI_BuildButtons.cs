using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BuildButtons : MonoBehaviour
{
    [SerializeField] private float yPositionOffset;

    private bool isActive;
    private UI_Animator uiAnim;

    private void Awake()
    {
        uiAnim = GetComponentInParent<UI_Animator>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B))
            ShowBuildButtons();
    }

    private void ShowBuildButtons()
    {
        isActive = !isActive;

        float yOffset = isActive ? yPositionOffset : -yPositionOffset;
        Vector3 offset = new Vector3(0, yOffset);

        uiAnim.ChangePosition(transform, offset);
    }
}
