using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSlot)), CanEditMultipleObjects]

public class TileSlotEditor : Editor
{
    private GUIStyle centeredStyle;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        base.OnInspectorGUI();

        centeredStyle = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            fontSize = 12
        };

        float oneButtonWidth = (EditorGUIUtility.currentViewWidth - 25);
        float twoButtonWidth = (EditorGUIUtility.currentViewWidth - 25) / 2;
        float threeButtonWidth = (EditorGUIUtility.currentViewWidth - 25) / 3;

        GUILayout.Label("Tile Option", centeredStyle);
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Field", GUILayout.Width(twoButtonWidth)))
        {
            GameObject newTile = FindFirstObjectByType<TileSlotHolder>().tileField;
            foreach(var targetTile in targets)
            {
                ((TileSlot)targetTile).SwitchTile(newTile);
            }
        }

        if(GUILayout.Button("Road", GUILayout.Width(twoButtonWidth)))
        {
            GameObject newTile = FindFirstObjectByType<TileSlotHolder>().tileRoad;
            foreach(var targetTile in targets)
            {
                ((TileSlot)targetTile).SwitchTile(newTile);
            }
        }

        GUILayout.EndHorizontal();  

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("SideWay", GUILayout.Width(oneButtonWidth)))
        {
            GameObject newTile = FindFirstObjectByType<TileSlotHolder>().tileSideway;
            foreach(var targetTile in targets)
            {
                ((TileSlot)targetTile).SwitchTile(newTile);
            }
        }

        GUILayout.EndHorizontal();


        GUILayout.Label("Corner Option", centeredStyle);

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Inner Corner", GUILayout.Width(twoButtonWidth)))
        {
            GameObject newTile = FindFirstObjectByType<TileSlotHolder>().tileInnerCorner;

            foreach(var targetTile in targets)
            {
                ((TileSlot)targetTile).SwitchTile(newTile);
            }
        }

        if(GUILayout.Button("Outer Corner", GUILayout.Width(twoButtonWidth)))
        {
            GameObject newTile = FindFirstObjectByType<TileSlotHolder>().tileOuterCorner;

            foreach(var targetTile in targets)
            {
                ((TileSlot)targetTile).SwitchTile(newTile);
            }
        }

        GUILayout.EndHorizontal();
    }    
}
