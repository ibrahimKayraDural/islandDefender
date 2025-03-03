using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMeshSelector : MonoBehaviour
{
    [SerializeField] Mesh[] _Meshes;
    [SerializeField] MeshFilter _Filter = null;

    bool _isInitialized;

    private void Start()
    {
        if (_isInitialized) return;

        if (_Filter == null) _Filter = GetComponentInChildren<MeshFilter>();
        if (_Filter == null) return;
        if (_Meshes.Length <= 0) return;

        float xPos = transform.localPosition.x - .1f;
        float zPos = transform.localPosition.z - .1f;
        float random = Mathf.Clamp(Mathf.PerlinNoise(xPos, zPos), 0, 1);
        float fraction = (float)(1f / _Meshes.Length);
        float newrand = random / fraction;
        int index = Mathf.FloorToInt(newrand);

        _Filter.sharedMesh = _Meshes[Mathf.Clamp(index, 0, _Meshes.Length - 1)];
        _isInitialized = true;
    }
}
