using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float _Damage = 1;
    [SerializeField] float _IterationDuration = .05f;
    [SerializeField] float _LifeTime = 10f;
    [SerializeField] Transform _VisualParent;
    [SerializeField] AudioClip HitSfx;
    [SerializeField] string[] IgnoreTags = new string[0];
    [SerializeField] bool _IgnoreTriggers = true;

    bool _isInitialized;
    bool _breakUpdate;
    float _speed;
    Vector3 _dir;
    Rigidbody _rb;

    public void Initialize(Vector3 direction, Vector3? visualDisplacement = null, float speedMultiplier = 1)
    {
        if (_isInitialized) return;

        _dir = direction;
        _speed = GLOBAL.BaseProjectileSpeed * speedMultiplier;
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.angularDrag = 0;
        _rb.drag = 0;
        transform.forward = direction;

        if (visualDisplacement != null)
            _VisualParent.localPosition = visualDisplacement.Value;

        StartCoroutine(nameof(UpdateLoop));
        StartCoroutine(LifeTimeEnum());

        _isInitialized = true;
    }

    IEnumerator UpdateLoop()
    {
        while(_breakUpdate == false)
        {
            Vector3 targetPos = transform.position + _dir * _speed * _IterationDuration;
            _rb.MovePosition(targetPos);

            yield return new WaitForSeconds(_IterationDuration);
        }
    }
    IEnumerator LifeTimeEnum()
    {
        yield return new WaitForSeconds(_LifeTime);
        DestroyProjectile(false);
    }

    void DestroyProjectile(bool playSFX = true)
    {
        _breakUpdate = true;
        StopAllCoroutines();

        if (playSFX)
            AudioManager.Instance?.PlayClip(this + "_hit", HitSfx);

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

        DestroyProjectile();
    }
}
