using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstance : MonoBehaviour
{
    public static PlayerInstance Instance { get; private set; } = null;

    public bool PlayerIsLocked { get; private set; } = false;
    public PlayerController PlayerController_Ref => _PlayerController;
    public PlayerInteractor PlayerInteractor_Ref => _PlayerInteractor;
    public Inventory Inventory_Ref => _Inventory;
    public PlayerToolController PlayerToolController_Ref => _PlayerToolController;
    public PlayerHealthController PlayerHealthController_Ref => _PlayerHealthController;

    [SerializeField] PlayerController _PlayerController;
    [SerializeField] PlayerInteractor _PlayerInteractor;
    [SerializeField] Inventory _Inventory;
    [SerializeField] PlayerToolController _PlayerToolController;
    [SerializeField] PlayerHealthController _PlayerHealthController;
    [SerializeField] CanvasManager _CanvasManager;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Start()
    {
        Respawn();
    }

    public void Respawn()
    {
        SpawnPoint sp = SpawnPoint.Instance;
        if (sp != null)
        {
            Vector3 target = sp.Position;
            target.y = transform.position.y;
            transform.position = target;
        }
    }

    public void SetPlayerIsLocked(bool setTo)
    {
        PlayerController_Ref.enabled = !setTo;
        PlayerInteractor_Ref.enabled = !setTo;
        PlayerToolController_Ref.enabled = !setTo;

        _CanvasManager.SetEnablity(!setTo);

        PlayerIsLocked = setTo;
    }
}
