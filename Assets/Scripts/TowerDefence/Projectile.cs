using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Tooltip("Default damage of projectile. Can be overriden by data")] float _Damage = 1;
    [SerializeField] float _IterationDuration = .05f;
    [SerializeField] float _LifeTime = 10f;
    [SerializeField] Transform _VisualParent;
    [SerializeField] AudioClip OnProjectileSpent;
    [SerializeField] string[] IgnoreTags = new string[0];
    [SerializeField] bool _IgnoreTriggers = true;

    int _penetrationCount = 0;
    bool _isInitialized;
    bool _breakUpdate;
    float _speed;
    Vector3 _dir;
    Rigidbody _rb;

    public void Initialize(Vector3 direction, float? damage = null, float speedMultiplier = 1, int penetrationCount = 0)
    {
        if (_isInitialized) return;

        _dir = direction.normalized;
        if (damage.HasValue) _Damage = damage.Value;
        _speed = GLOBAL.BaseProjectileSpeed * speedMultiplier;
        _penetrationCount = Mathf.Max(penetrationCount, 0);

        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.angularDrag = 0;
        _rb.drag = 0;
        transform.forward = direction;

        StartCoroutine(nameof(UpdateLoop));
        StartCoroutine(LifeTimeIEnum());

        _isInitialized = true;
    }
    public void Initialize(Vector3 direction, TowerDefence.TurretData data)
        => Initialize(direction, data.Damage, data.ProjectileSpeedMultiplier, data.PenetrationCount);

    IEnumerator UpdateLoop()
    {
        while (_breakUpdate == false)
        {
            Vector3 targetPos = transform.position + _dir * _speed * _IterationDuration;
            _rb.MovePosition(targetPos);

            yield return new WaitForSeconds(_IterationDuration);
        }
    }
    IEnumerator LifeTimeIEnum()
    {
        yield return new WaitForSeconds(_LifeTime);
        DestroyProjectile(false);
    }

    void DestroyProjectile(bool playSFX = true)
    {
        _breakUpdate = true;
        StopAllCoroutines();

        if (playSFX)
            AudioManager.Instance?.PlayClip(this + "_IsSpentSFX", OnProjectileSpent);

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (_IgnoreTriggers && other.isTrigger) return;

        foreach (var tag in IgnoreTags)
        {
            if (other.gameObject.tag == tag) return;
        }

        if (other.TryGetComponent(out IHealth ih))
        {
            ih.RemoveHealth(_Damage);
        }

        _penetrationCount--;

        if (_penetrationCount <= 0) DestroyProjectile();
    }
}
