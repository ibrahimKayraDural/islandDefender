using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameplayElementData<T> : Data<T>
{
    public string Description => _Description;
    public Sprite UISprite => _UISprite;

    [SerializeField, TextArea] string _Description = GLOBAL.UnassignedString;
    [SerializeField] Sprite _UISprite;
}
