using UnityEditor;
using UnityEngine;
using TowerDefence;

[CustomEditor(typeof(GridManager))]
public class Editor_GridManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridManager gm = (GridManager)target;

        GUILayout.Space(30);
        if (GUILayout.Button("Generate")) gm.Generate();
    }
}
