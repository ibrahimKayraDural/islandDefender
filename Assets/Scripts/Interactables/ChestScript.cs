using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractable
{
    public string InteractDescription { get => "Open Chest"; set { } }

    [SerializeField] Animator _Animator;
    [SerializeField] float _ForgetDistance = 1;

    bool _isOpen = false;
    CanvasManager _canvasManager;
    Transform _currentInteractor = null;

    void Start()
    {
        _canvasManager = CanvasManager.Instance;
    }

    public void OnInteracted(GameObject interactor)
    {
        _currentInteractor = interactor.transform;
        SetOpennes(!_isOpen);
    }

    void SetOpennes(bool setTo)
    {
        _isOpen = setTo;
        _Animator?.SetBool("IsOpen", setTo);
        _canvasManager.SetChestUIEnablity(setTo);

        if (setTo) StartCoroutine(nameof(CheckForget));
        else StopCoroutine(nameof(CheckForget));
    }

    IEnumerator CheckForget()
    {
        Vector3 pos = transform.position;
        while (true)
        {
            if(Vector3.Distance(_currentInteractor.position, pos) >= _ForgetDistance)
            {
                _currentInteractor = null;
                SetOpennes(false);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
