namespace Overworld
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tool/Tool Database")]
    public class ToolDatabase : ScriptableObject
    {
        public List<Tool> ToolList => _ToolList;
        [SerializeField] List<Tool> _ToolList = new List<Tool>();

        public Tool GetToolByID(string id) => ToolList.Find(x => x.Data.ID == id);
        public Tool GetToolByDisplayName(string displayName) => ToolList.Find(x => x.Data.DisplayName == displayName);
    }
}