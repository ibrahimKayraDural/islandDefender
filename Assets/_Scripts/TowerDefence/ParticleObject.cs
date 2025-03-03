using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    [SerializeField] float LifeTime = 1;
    [SerializeField] AudioClip _Clip;

    void Start()
    {
        if (_Clip != null)
            AudioManager.Instance?.PlayClip("ProjectileExplosion_SFX", _Clip);

        Destroy(gameObject, Mathf.Max(0, LifeTime));
    }
}
