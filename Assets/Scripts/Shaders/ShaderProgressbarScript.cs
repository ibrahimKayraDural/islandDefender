using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderProgressbarScript : MonoBehaviour
{
    [SerializeField] MeshRenderer _Renderer;

    float _currentFill = 0;
    Material _mat;

    void Awake()
    {
        if (_Renderer == null) _Renderer = GetComponentInChildren<MeshRenderer>();
        if (_Renderer == null)
        {
            this.enabled = false;
            return;
        }

        _mat = _Renderer.material;
    }

    public void SetValue(float setTo)
    {
        setTo = Mathf.Clamp(setTo, 0, 1);

        _currentFill = setTo;
        _mat.SetFloat("_Fill", _currentFill);
    }

    public void AddValue(float addition) => SetValue(addition + _currentFill);
}
