using Overworld;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreCell : UICell
{
    public CoreItem ItemData => _item;

    CoreItem _item = null;

    public virtual void Initialize(CoreData data, bool isInteractable = true, string ownerID = null)
    {
        _item = data?.AsItem();
        if (_item == null)
        {
            base.Initialize();
            return;
        }

        base.Initialize(data.UISprite, data.DisplayName, data.Description, isInteractable, ownerID);
    }
}
