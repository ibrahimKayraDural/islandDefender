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
        public List<Data<SwarmData>> DataListAccess
        {
            get => _DataList;
            set => _DataList = value;
        }
    }
}