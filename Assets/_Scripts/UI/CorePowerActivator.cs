using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CorePowerActivator : MonoBehaviour
{
    [SerializeField] Button _Button;
    [SerializeField] TextMeshProUGUI CountTM;
    [SerializeField] CoreManager _CoreManager;
    [SerializeField] GameObject VisualParent;
    [SerializeField] Image Icon;

    public void Activate()
    {
        if (_CoreManager.TryActivateCorePower())
            Refresh(_CoreManager.SelectedCorePower);
    }
    public void Refresh(CorePower_Base corePower)
    {
        if (corePower == null)
        {
            VisualParent.SetActive(false);
            return;
        }

        VisualParent.SetActive(true);

        Icon.sprite = corePower.Data.UISprite;
        var usagesLeft = corePower.UsagesLeft;
        CountTM.text = usagesLeft.ToString();
        _Button.interactable = usagesLeft > 0;
    }
}
