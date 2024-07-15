using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    public class TowerDefenceTileScript : MonoBehaviour
    {
        Material _material = null;

        void Awake()
        {
            if (TryGetComponent(out MeshRenderer mr))
            {
                _material = mr.sharedMaterial;
            }
        }

        public void GetHighlighted() => SetHighlighted(true);
        public void GetUnhighlighted() => SetHighlighted(false);
        public void SetHighlighted(bool setTo)
        {
            if (_material == null) return;

            _material.SetFloat("_IsHighlighted", setTo ? 1 : 0);
        }
    } 
}
