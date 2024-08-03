using UnityEditor;
using UnityEngine;
using TowerDefence;

[CustomEditor(typeof(SwarmDatabase))]
public class Editor_SwarmDatabase : Editor
{
    int _listIndex = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SwarmDatabase sdb = (SwarmDatabase)target;

        GUILayout.Space(30);
        GUILayout.BeginHorizontal();
        _listIndex = Mathf.Clamp(EditorGUILayout.IntField(_listIndex, new GUILayoutOption[] { GUILayout.Width(50) }), 0, sdb.DataList.Count - 1);
        if (GUILayout.Button("Set CurrentSwarmData to Data at index")) sdb.TrySetCurrentSwarmDataToDataAtIndex(_listIndex);
        GUILayout.EndHorizontal();
    }
}
