using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class DoorIngredientCell : ResourceCell
    {
        [SerializeField] Color EnoughColor = Color.green;
        [SerializeField] Color NotEnoughColor = Color.red;

        //int _requiredAmount;

        public void Initialize(ResourceData data, int requiredAmount, int currentAmount, int i, string ownerID = null)
        {
            //_requiredAmount = requiredAmount;

            Initialize(data, currentAmount, i, false, ownerID);

            _CountTM.text = currentAmount + " / " + requiredAmount;
            _BackgroundImage.color = currentAmount >= requiredAmount ? EnoughColor : NotEnoughColor;
        }
    } 
}
