using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSlot)), CanEditMultipleObjects]

public class TileSlotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Tile"))
        {
            foreach(var obje in targets)
            {
                ((TileSlot)obje).ButtonCheck();
            }
        }
    }    
}
