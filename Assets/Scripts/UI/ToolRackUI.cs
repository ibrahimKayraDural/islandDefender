namespace GameUI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ToolRackUI : MonoBehaviour, IUserInterface
    {
        public ToolRack CurrentRack { get; private set; } = null;
        public bool IsOpen { get; set; } = false;

        [SerializeField] GameObject _VisualParent;

        List<KeyCode> CloseKeys = new List<KeyCode>() {
            KeyCode.I
        };
        bool _breakUpdate = false;


        public bool TrySetCurrentRack(ToolRack setTo)
        {
            if (CurrentRack == null)//chest is opened
            {
                CurrentRack = setTo;
                StartCoroutine(nameof(UpdateIEnum));
            }
            else if (setTo == null)//chest is closed
            {
                CurrentRack = null;
                _breakUpdate = true;
            }
            else
            {
                return false;
            }

            return true;
        }

        IEnumerator UpdateIEnum()
        {
            yield return new WaitForSeconds(.1f);

            //Update is the inside of this loop.
            //Code above will run once before the update loop.
            while (_breakUpdate == false)
            {
                //Input start
                foreach (var key in CloseKeys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        Close();
                        goto InputEND;
                    }
                }
                if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Exit"))
                {
                    Close();
                    goto InputEND;
                }
            InputEND:;//input end

                yield return null;
            }
            //Update is the inside of the loop above.
            //Code below will run once after the update loop has ended.

            _breakUpdate = false;
        }

        void Close()
        {
            CurrentRack.SetOpennes(false);
        }

        public void OnEnablityChanged(bool changedTo)
        {
            _VisualParent.SetActive(changedTo);
        }
        public void SetEnablityGetter(bool setTo) => (this as IUserInterface).SetEnablity(setTo);
    }

}