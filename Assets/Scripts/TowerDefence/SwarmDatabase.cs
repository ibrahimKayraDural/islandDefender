namespace TowerDefence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tower Defence/Swarm Database")]
    public class SwarmDatabase : Database<SwarmData>
    {
        public event EventHandler<SwarmData> e_CurrentSDHasChanged;

        public List<Data<SwarmData>> DataListAccess
        {
            get => _DataList;
            set => _DataList = value;
        }
        public SwarmData CurrentSwarmData
        {
            get => _CurrentSwarmData;
            set
            {
                _CurrentSwarmData.SetSwarmValues(value);
                e_CurrentSDHasChanged?.Invoke(this, _CurrentSwarmData);
            }
        }

        [SerializeField] SwarmData _CurrentSwarmData;

#if UNITY_EDITOR
        /// <summary>
        /// Tries to set values of Current Swarm Data to the Swarm Data at index
        /// </summary>
        /// <returns>If the values are set successfully</returns>
        public bool TrySetCurrentSwarmDataToDataAtIndex(int index)
        {
            if (_DataList.Count <= 0) return false;
            if (index < 0 || index >= _DataList.Count) return false;

            EditorUtility.SetDirty(_CurrentSwarmData);

            SwarmData data = DataList[index];
            _CurrentSwarmData.SetSwarmValues(data);
            return true;
        }
#endif
    }
}