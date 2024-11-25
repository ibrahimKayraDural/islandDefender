using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    [SerializeField] float LifeTime = 1;

    void Start()
    {
        Destroy(gameObject, Mathf.Max(0, LifeTime));
    }
}
