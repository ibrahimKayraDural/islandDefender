namespace GameUI
{
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class InteractableHelperUI : MonoBehaviour
    {
        [SerializeField] GameObject _VisualsParent;
        [SerializeField] TextMeshProUGUI _HelperText;

        public void SetHelperEnablity(bool setTo)
        {
            _VisualsParent.SetActive(setTo);
        }

        public void SetHelperText(string setTo)
        {
            _HelperText.text = setTo;
        }
    }
}
