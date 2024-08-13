namespace Overworld
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tool/Tool Database")]
    public class ToolDatabase : ScriptableObject
    {
        public List<GameObject> ToolList => _ToolList;
        [SerializeField] List<GameObject> _ToolList = new List<GameObject>();

        private void OnValidate()
        {
            foreach (var item in _ToolList)
            {
                if (item.TryGetComponent(out Tool _) == false)
                    _ToolList.Remove(item);
            }
        }

        public GameObject GetToolByID(string id)
        {
            return ToolList.Find(x =>
            {
                if (x.TryGetComponent(out Tool tool) == false) return false;

                return tool.Data.ID == id;
            });
        }
        public GameObject GetToolByDisplayName(string displayName)
        {
            return ToolList.Find(x =>
            {
                if (x.TryGetComponent(out Tool tool) == false) return false;

                return tool.Data.DisplayName == displayName;
            });
        }
    }
}