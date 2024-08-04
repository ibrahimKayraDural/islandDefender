using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestScript))]
public class Editor_TestScript : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestScript ts = (TestScript)target;

        GUILayout.Space(30);
        if (GUILayout.Button("Run ''TestMethod''")) ts.TestMethod();
    }
}
