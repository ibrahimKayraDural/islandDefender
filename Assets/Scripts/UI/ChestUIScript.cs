namespace GameUI
{
    using Overworld;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ChestUIScript : MonoBehaviour, UserInterface
    {
        [SerializeField] GameObject _VisualParent;

        public bool IsOpen { get; set; }

        public void OnEnablityChanged(bool changedTo)
        {
            _VisualParent.SetActive(changedTo);

            //if (setTo)
            //{
            //    PlayerToolController.ToolDisallowers?.Add(this);
            //}
            //else
            //{
            //    PlayerToolController.ToolDisallowers?.Remove(this);
            //}
        }

        public void SetEnablityGetter(bool setTo)
        {
            UserInterface ui = this as UserInterface;
            ui.SetEnablity(setTo);
        }
    }

}