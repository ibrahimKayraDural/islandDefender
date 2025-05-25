using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemySpeedModifier : MonoBehaviour
{
    [SerializeField] float _Modification = 1;
    [SerializeField] string _ID = "enemy-speed-modifier";
    [SerializeField] int _UseCount = -1;

    bool _IsSpent => _currentUseCount >= _UseCount;

    Dictionary<EnemyBase, string> dic_registeredEnemies = new();
    bool _wasSpent = false;
    int _currentUseCount = 0;

    void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void KillSelf()
    {
        Destroy(gameObject);
    }
    void PlaySpentAnim()
    {
        if (_wasSpent) return;

        //play here

        _wasSpent = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_UseCount >= 0 && _IsSpent) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("TowerDefenceEnemy")) return;
        if (other.TryGetComponent(out EnemyBase eb) == false) return;
        if (dic_registeredEnemies.ContainsKey(eb)) return;

        eb.AddSpeedModifier(_ID, _Modification, out string usedID);
        dic_registeredEnemies.Add(eb, usedID);
        _currentUseCount++;

        if (_IsSpent) PlaySpentAnim();
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out EnemyBase eb) == false) return;
        if (dic_registeredEnemies.ContainsKey(eb) == false) return;

        eb.RemoveSpeedModifier(dic_registeredEnemies[eb]);
        dic_registeredEnemies.Remove(eb);

        if (dic_registeredEnemies.Count == 0 && _IsSpent) KillSelf();
    }
}
