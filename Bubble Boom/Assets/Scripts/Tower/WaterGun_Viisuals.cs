using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGun_Viisuals : MonoBehaviour
{
    private Enemy myEnemy;
    [SerializeField] private LineRenderer attackVisuals;
    [SerializeField] private float attackVisualDuration = .1f;
    
    private void Upadate()
    {
        UpdateAttacksVisuals();
    }

    private void UpdateAttacksVisuals()
    {
        if (attackVisuals.enabled && myEnemy != null)
            attackVisuals.SetPosition(1, myEnemy.CenterPoint());
    }

    public void PlayAttackVisual(Vector3 startPoin, Vector3 endPoin, Enemy newEnemy)
    {
        StartCoroutine(VFXCoroutine(startPoin, endPoin, newEnemy));
    }

    private IEnumerator VFXCoroutine(Vector3 startPoin, Vector3 endPoin, Enemy newEnemy)
    {
        myEnemy = newEnemy;
        attackVisuals.enabled = true;
        attackVisuals.SetPosition(0, startPoin);
        attackVisuals.SetPosition(1,endPoin);

        yield return new WaitForSeconds(attackVisualDuration);
        attackVisuals.enabled = false;
    }

}
