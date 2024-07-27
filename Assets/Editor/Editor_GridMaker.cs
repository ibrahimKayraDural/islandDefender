using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMaker))]
public class Editor_GridMaker : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GridMaker gm = (GridMaker)target;

        GUILayout.Space(30);
        if (GUILayout.Button("Generate")) gm.Generate();
    }
}
