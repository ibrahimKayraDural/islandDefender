using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [SerializeField] string _PlayerTag = "OverworldPlayer";
    [SerializeField] float _Damage = 10;
    [SerializeField] bool _DestroyAfterUsage;
    [SerializeField] List<DamageType> _DamageTypes;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != _PlayerTag) return;

        var player = other.transform.GetComponentInParent<IHealth>();
        player.RemoveHealth(_Damage, _DamageTypes);
        if (_DestroyAfterUsage) Destroy(gameObject);
    }
}
