using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceCountDispplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI DisText;
    [SerializeField] ResourceData resoureData;

    BaseResourceController controller;
    private void Awake()
    {
        controller = BaseResourceController.Instance;
    }
    public void Refresh()
    {
        if (controller == null) return;
        
        DisText.text = controller.CheckItemCount(resoureData.AsItem()).ToString();
    }
}
