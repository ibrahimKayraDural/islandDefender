using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOneShot : MonoBehaviour
{
    [SerializeField] AudioSource _asour;
    [SerializeField] AudioClip _clip;

    float lenght;

    void Start()
    {
        lenght = _clip.length + .05f;
        _asour.PlayOneShot(_clip);
        Invoke(nameof(KillSelf), lenght);
    }
    public void SetClip(AudioClip clip)
    {
        _clip = clip;
    }
    void KillSelf()
    {
        Destroy(gameObject);
    }
}
