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

        bool containtsExplosive = Types.Contains(EnemyType.Explosive);
        bool containtsSwarm = Types.Contains(EnemyType.Swarm);

        if (containtsExplosive == false)
        {
            ed.ExplosionPrefab = null;
            ed.ExplosionRange = 1;
            ed.ExplosionDamage = 1;
        }
        if (containtsSwarm == false)
        {
            ed.SwarmCount = 1;
            ed.SwarmCooldownRange = new(.5f,1);
        }

        if (Types.Count > 0)
        {
            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("Additional Values", EditorStyles.boldLabel);

            if (containtsExplosive)
            {
                ed.ExplosionPrefab = (GameObject)EditorGUILayout.ObjectField("Exploison prefab", ed.ExplosionPrefab, typeof(GameObject), false);
                ed.ExplosionRange = EditorGUILayout.IntField("Exploison range", ed.ExplosionRange);
                ed.ExplosionDamage = EditorGUILayout.IntField("Exploison damage", ed.ExplosionDamage);
            }
            if (containtsSwarm)
            {
                ed.SwarmCount = EditorGUILayout.IntField("Swarm Count", ed.SwarmCount);
                float min = EditorGUILayout.FloatField("Swarm Cooldown Min", ed.SwarmCooldownRange.Min);
                float max = EditorGUILayout.FloatField("Swarm Cooldown Max", ed.SwarmCooldownRange.Max);
                ed.SwarmCooldownRange = new(min, max);
            }
        }
    }
}
