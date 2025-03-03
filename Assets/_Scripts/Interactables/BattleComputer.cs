using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleComputer : MonoBehaviour, IInteractable
{
    public string InteractDescription { get => "Manage battlefield"; set { } }

    BattleManager _battleManager
    {
        get
        {
            if (AUTO_battleManager == null)
                AUTO_battleManager = BattleManager.Instance;
            return AUTO_battleManager;
        }
    }
    BattleManager AUTO_battleManager = null;

    public void OnInteracted(GameObject interactor)
    {
        _battleManager.EnterBattle(TowerDefence.TowerDefenceControlMode.Full);
    }
}
