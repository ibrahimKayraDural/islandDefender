using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tool/Tool Data")]
public class ToolData : Data<ToolData>
{
    public Sprite UISprite => _UISprite;
    public string Description => _Description;

    [SerializeField] Sprite _UISprite;
    [SerializeField] string _Description = "A tool for your journeys";
}
