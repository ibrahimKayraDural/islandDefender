using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TowerDefence
{
    public class TDCanvasManager : MonoBehaviour
    {
        [SerializeField] Transform CanvasParent;
        [SerializeField] List<Transform> EditModeCanvasObjects;
        [SerializeField] List<Transform> PlayModeCanvasObjects;
        [SerializeField] List<Transform> IdleModeCanvasObjects;

        public void SetCanvas(TowerDefenceGameplayMode? mode)
        {
            List<Transform> objects;

            switch (mode)
            {
                case TowerDefenceGameplayMode.Idle: objects = IdleModeCanvasObjects; break;
                case TowerDefenceGameplayMode.Play: objects = PlayModeCanvasObjects; break;
                case TowerDefenceGameplayMode.Edit: objects = EditModeCanvasObjects; break;
                default: objects = new List<Transform>(); break; //if the value is null
            }

            var tempList = CanvasParent.Cast<Transform>().ToList();
            foreach (var child in tempList)
            {
                child.gameObject.SetActive(objects.Contains(child));
            }
        }
    }
}
