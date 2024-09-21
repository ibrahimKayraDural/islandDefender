using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequirementInteractable : ProximityInteractable
{
    public override string InteractDescription { get => _InteractDescription; set => _InteractDescription = value; }
    [SerializeField] string _InteractDescription = "See requirements";
}
