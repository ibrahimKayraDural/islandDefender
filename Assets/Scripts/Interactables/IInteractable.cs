using UnityEngine;

public interface IInteractable
{
    string InteractDescription { get; set; }
    void OnInteracted(GameObject interactor);
}
