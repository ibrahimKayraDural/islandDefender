using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TowerDefence;

[CustomEditor(typeof(EnemyData))]
public class Editor_EnemyData : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EnemyData ed = (EnemyData)target;
        List<EnemyType> Types = ed.EnemyTypes;

        if (Types.Contains(EnemyType.Explosive) == false)
        {
            ed.ExplosionPrefab = null;
            ed.ExplosionRange = 1;
            ed.ExplosionDamage = 1;
        }

        if (Types.Count > 0)
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Additional Values", EditorStyles.boldLabel);

            if (Types.Contains(EnemyType.Explosive))
            {
                ed.ExplosionPrefab = (GameObject)EditorGUILayout.ObjectField("Exploison prefab", ed.ExplosionPrefab, typeof(GameObject), false);
                ed.ExplosionRange = EditorGUILayout.IntField("Exploison range", ed.ExplosionRange);
                ed.ExplosionDamage = EditorGUILayout.IntField("Exploison damage", ed.ExplosionDamage);
            }
        }
    }
}
