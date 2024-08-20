using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractable
{
    public List<InventoryItem> Slots => _slots;

    public string InteractDescription { get => "Open Chest"; set { } }

    [SerializeField, Min(1)] int _Capacity = 14;
    [SerializeField] float _ForgetDistance = 1;
    [SerializeField] Animator _Animator;

    List<InventoryItem> _slots;
    bool _isOpen = false;
    CanvasManager _canvasManager;
    Transform _currentInteractor = null;

    void Start()
    {
        _canvasManager = CanvasManager.Instance;

        _slots = new List<InventoryItem>(new InventoryItem[_Capacity]);
    }

    public void OnInteracted(GameObject interactor)
    {
        _currentInteractor = interactor.transform;

        if (_canvasManager.TrySetCurrentChestOfChestUI(this))
        {
            SetOpennes(true);
        }
    }
    public void SetOpennes(bool setTo)
    {
        if (_isOpen == setTo) return;

        _isOpen = setTo;
        _Animator.SetBool("IsOpen", setTo);
        _canvasManager.SetChestUIEnablity(setTo);

        if (setTo)
        {
            StartCoroutine(nameof(CheckForget));
        }
        else
        {
            StopCoroutine(nameof(CheckForget));
            _canvasManager.TrySetCurrentChestOfChestUI(null);
        }
    }

    IEnumerator CheckForget()
    {
        Vector3 pos = transform.position;
        while (true)
        {
            if (Vector3.Distance(_currentInteractor.position, pos) >= _ForgetDistance)
            {
                _currentInteractor = null;
                SetOpennes(false);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    bool IsNull(InventoryItem item) => GLOBAL.IsNull(item);
}
