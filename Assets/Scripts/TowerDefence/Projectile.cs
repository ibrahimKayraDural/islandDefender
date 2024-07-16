using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float _Damage = 1;
    [SerializeField] float _IterationDuration = .05f;
    [SerializeField] float _LifeTime = 10f;

    bool _isInitialized;
    bool _breakUpdate;
    float _speed;
    Vector3 _dir;
    Rigidbody _rb;

    public void Initialize(Vector3 direction, float speedMultiplier = 1)
    {
        if (_isInitialized) return;

        _dir = direction;
        _speed = GLOBAL.BaseProjectileSpeed * speedMultiplier;
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.angularDrag = 0;
        _rb.drag = 0;
        transform.forward = direction;

        StartCoroutine(nameof(UpdateLoop));
        Invoke(nameof(DestroyProjectile), _LifeTime);

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

    void DestroyProjectile()
    {
        _breakUpdate = true;
        StopAllCoroutines();

        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IGetShot igs))
        {
            igs.GetShot(_Damage);
        }

        DestroyProjectile();
    }
}
