using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public BuildSlot seletectedBuildSlot;

    public void SelectBuildSlot(BuildSlot newSlot)
    {
        if(seletectedBuildSlot != null)
            seletectedBuildSlot.UnselectTile();
            
        seletectedBuildSlot = newSlot;
    }
}
