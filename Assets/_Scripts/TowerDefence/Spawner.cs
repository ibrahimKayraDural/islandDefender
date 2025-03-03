using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Vector3 Position => transform.position;

    [SerializeField] List<EnemyIndicator> _Indicators;
    
    /// <summary>
    /// Sets enemy indicators. Enter NULL to clean indicators
    /// </summary>
    /// <param name="enemyDatas"> Datas of the enemies to display. Enter NULL to clean </param>
    public void SetEnemyIndicators(EnemyData[] enemyDatas)
    {
        if(enemyDatas == null)
        {
            foreach (var indicator in _Indicators)
            {
                indicator.SetIndicator(null);
            }

            return;
        }

        List<EnemyData> uniqueDatas = new List<EnemyData>();

        foreach (var data in enemyDatas)
        {
            if(uniqueDatas.Contains(data) == false)
            {
                uniqueDatas.Add(data);
            }
        }

        for (int i = 0; i < _Indicators.Count; i++)
        {
            EnemyIndicator ei = _Indicators[i];

            if(i < uniqueDatas.Count)
            {
                ei.SetIndicator(uniqueDatas[i]);
                continue;
            }

            ei.SetIndicator(null);
        }
    }
}
