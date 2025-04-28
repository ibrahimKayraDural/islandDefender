using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbarManager : MonoBehaviour
{
    [SerializeField] EnemyHealthbar _HealthbarPrefab;
    [SerializeField] Transform _CameraPosition;
    [SerializeField] Vector3 _HealthbarOffset = Vector3.up;
    [SerializeField] Vector3 _HealthbarRotation = new(0, -90, 0);

    public void SpawnBar(GameObject enemyGORef, EnemyDifficulty difficulty)
    {
        var enemyRef = enemyGORef.GetComponentInChildren<EnemyBase>();
        if (enemyRef == null) return;

        Instantiate(_HealthbarPrefab.gameObject).GetComponentInChildren
            <EnemyHealthbar>().Initialize(enemyRef, _HealthbarRotation, _HealthbarOffset);
    }
}
