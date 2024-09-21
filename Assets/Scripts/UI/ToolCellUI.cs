using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolCellUI : UICell
{
    public ToolData CellData => _data;

    ToolData _data;

    public void Initialize(ToolData data, bool isInteractable = true, string ownerID = null)
    {
        _data = data;

        Initialize(_data.UISprite, _data.DisplayName, _data.Description, isInteractable, ownerID);
    }
}
