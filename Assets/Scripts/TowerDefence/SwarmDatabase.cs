namespace TowerDefence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
    }
}