namespace GameUI
{
    using Overworld;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ChestUIScript : MonoBehaviour, UserInterface, GridUI
    {
        public ChestScript CurrentChest { get; private set; } = null;
        public bool IsOpen { get; set; }

        [SerializeField] GameObject _VisualParent;
        [SerializeField] Transform _CellParent;
        [SerializeField] GameObject _CellPrefab;

        List<KeyCode> ChestCloseKeys = new List<KeyCode>() {
            KeyCode.Escape
        };

        public bool TrySetCurrentChest(ChestScript setTo)
        {
            if (CurrentChest == null)//chest is opened
            {
                CurrentChest = setTo;
                StartCoroutine(nameof(CheckChestDisable));
            }
            else if (setTo == null)//chest is closed
            {
                CurrentChest = null;
                StopCoroutine(nameof(CheckChestDisable));
            }
            else
            {
                return false;
            }

            return true;
        }
        IEnumerator CheckChestDisable()
        {
            yield return new WaitForSeconds(.1f);

            while (true)
            {
                yield return null;

                foreach (var key in ChestCloseKeys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        CurrentChest.SetOpennes(false);
                        goto EndIEnum;
                    }
                }
                if (Input.GetButtonDown("Interact"))
                {
                    CurrentChest.SetOpennes(false);
                    goto EndIEnum;
                }
            }

        EndIEnum:;
        }
        public void OnEnablityChanged(bool changedTo)
        {
            _VisualParent.SetActive(changedTo);

            if(changedTo == true)
            {
                if (CurrentChest == null) Debug.LogError("Can not refresh Chest UI");
                else (this as GridUI).RefreshInventory(CurrentChest.Slots.ToArray(), _CellParent, _CellPrefab);
            }
        }

        public void SetEnablityGetter(bool setTo) => (this as UserInterface).SetEnablity(setTo);

    }
}