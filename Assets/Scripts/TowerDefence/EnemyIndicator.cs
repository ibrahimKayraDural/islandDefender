using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;

public class EnemyIndicator : MonoBehaviour
{
    [SerializeField] GameObject _Parent;
    [SerializeField] Image _EnemyImage;

    public void SetIndicator(EnemyData data)
    {
        if (data == null)
        {
            Close();
            return;
        }

        Open(data);
    }

    void Close()
    {
        _Parent.SetActive(false);
        _EnemyImage.sprite = null;
    }

    void Open(EnemyData data)
    {
        _EnemyImage.sprite = data.EnemyUISprite;
        _Parent.SetActive(true);
    }
}
