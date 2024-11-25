using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileExplosive : MonoBehaviour
{
    readonly float TILE_SIZE = 1.95f;
    readonly float ROTATION_RESCALE = Mathf.Sqrt(1f / 2f);

    [SerializeField] LayerMask _TargetMask;
    [SerializeField] ParticleObject _ExplosionEffect;
    [SerializeField] float XOffset = .1f;

    public void Activate(int tileRadius, int damage)
    {
        float actualHeight = (tileRadius * TILE_SIZE) + (TILE_SIZE / 2);
        float radius = actualHeight * ROTATION_RESCALE;
        float heigt = actualHeight - radius;
        Vector3 origin = transform.position + Vector3.right * XOffset + Vector3.down * tileRadius * 5;
        Vector3 axis = Vector3.forward * heigt;
        RaycastHit[] hits = Physics.CapsuleCastAll(origin - axis, origin + axis, radius, Vector3.up, tileRadius * 10, _TargetMask);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.TryGetComponent(out IHealth ih)) ih.RemoveHealth(damage);
        }

        Instantiate(_ExplosionEffect.gameObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
