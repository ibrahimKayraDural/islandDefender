using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TowerDefence;

[CustomEditor(typeof(TurretData))]
public class Editor_TurretData : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TurretData td = (TurretData)target;
        List<TurretAttribute> attributes = td.Attributes;

        if (attributes.Contains(TurretAttribute.Penetrater) == false) td.PenetrationCount = 0;
        if (attributes.Contains(TurretAttribute.AreaOfEffect) == false) td.AOE_Radius = 0;

        if(attributes.Count > 0)
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Additional Values", EditorStyles.boldLabel);

            if (attributes.Contains(TurretAttribute.Penetrater))
            {
                td.PenetrationCount = EditorGUILayout.IntField("Penetration count", td.PenetrationCount);
            }
            if (attributes.Contains(TurretAttribute.AreaOfEffect))
            {
                td.AOE_Radius = EditorGUILayout.IntField("Area of Effect (tile reach)", td.AOE_Radius);
            }
        }
    }
}
