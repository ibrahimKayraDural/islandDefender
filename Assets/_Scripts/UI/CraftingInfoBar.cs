using TMPro;
using TowerDefence;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CraftingInfoBar : MonoBehaviour
{
    [SerializeField] GameObject _VisualParent;
    [SerializeField] TextMeshProUGUI _CostText;
    [SerializeField] TextMeshProUGUI _TurretTitle;
    [SerializeField] TextMeshProUGUI _TurretDescription;

    public void Refresh(TurretData data, float xPosition = 0)
    {
        transform.position = new Vector3(xPosition, transform.position.y, 0);
        SetData(data);
    }
    public void SetEnablity(bool setTo)
    {
        _VisualParent.SetActive(setTo);
    }
    public void SetData(TurretData data)
    {
        _TurretTitle.text = data?.DisplayName ?? "";
        _TurretDescription.text = data?.Description ?? "";

        _CostText.text = "";
        if (data) foreach (var cost in data.Costs) _CostText.text += cost.Amount + " " + cost.Resource.DisplayName;
    }
}
