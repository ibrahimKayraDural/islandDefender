using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefence
{
    [CreateAssetMenu(menuName = "Tower Defence/Wave Database", fileName = "WaveDatabase")]

    public class TD_WaveDatabase : Database<TD_Wave>
    {
        public List<Data<TD_Wave>> DataListAccess
        {
            get => _DataList;
            set => _DataList = value;
        }
    }
}
