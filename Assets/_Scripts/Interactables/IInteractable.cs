using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string InteractDescription { get; set; }
    List<string> OptionalParameters { get; set; }
    void OnInteracted(GameObject interactor);
}
