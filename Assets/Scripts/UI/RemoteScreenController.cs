using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteScreenController : MonoBehaviour
{
    [SerializeField] Animator _Animator;

    bool _isOpen = false;

    void Awake()
    {
        CanvasManager.e_OnCurrentInterfaceChanged += OnInterfaceChanged;
    }
    void Update()
    {
        if (Input.GetButtonDown("RemoteScreen")) SetScreenEnablity(!_isOpen);
    }

    void OnInterfaceChanged(object sender, IUserInterface e)
    {
        if (e != null) SetScreenEnablity(false);
    }

    public void SetScreenEnablity(bool setTo)
    {
        if (CanvasManager.SomethingIsOpen) setTo = false;

        _Animator.SetBool("Open", setTo);
        _isOpen = setTo;
    }
}
