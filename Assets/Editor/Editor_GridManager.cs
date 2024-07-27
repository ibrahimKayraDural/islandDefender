using UnityEditor;
using UnityEngine;
using TowerDefence;

[CustomEditor(typeof(TowerDefenceGridManager))]
public class Editor_GridManager : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TowerDefenceGridManager gm = (TowerDefenceGridManager)target;

        GUILayout.Space(30);
        if (GUILayout.Button("Generate")) gm.Generate();
    }
}
