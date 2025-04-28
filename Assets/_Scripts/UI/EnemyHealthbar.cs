using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TowerDefence;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] Slider _Healthbar;
    [SerializeField] Image _FillImage;
    [SerializeField] Image _BackgroundImage;
    [SerializeField] SerializedDictionary<EnemyDifficulty, BarOptions> dic_DifficultyBarOptions;

    EnemyBase _enemy;

    [System.Serializable]
    struct BarOptions
    {
        public Color FillColor;
        public Color BackgroundColor;
        public Vector3 Size;
    }

    public void Initialize(EnemyBase enemyRef, Vector3? rotation, Vector3? offset)
    {
        if (enemyRef == null)
        {
            KillSelf();
            return;
        }
        _enemy = enemyRef;

        transform.SetParent(_enemy.transform, false);
        _enemy.OnDeath.AddListener(KillSelf);
        _enemy.OnHealthChanged += RefreshHealthOfBar;

        transform.localPosition = offset.HasValue ? offset.Value : Vector3.zero;
        transform.rotation = Quaternion.Euler(rotation.HasValue ? rotation.Value : Vector3.zero);

        var diff = _enemy.Data.Difficulty;
        if (dic_DifficultyBarOptions.ContainsKey(diff))
        {
            var opts = dic_DifficultyBarOptions[diff];
            _Healthbar.transform.localScale = opts.Size;
            _FillImage.color = opts.FillColor;
            _BackgroundImage.color = opts.BackgroundColor;
        }

        _Healthbar.maxValue = 1;
        _Healthbar.minValue = 0;
        _Healthbar.value = 1;
    }

    public void RefreshHealthOfBar(object sender, float normalizedHealth)
    {
        _Healthbar.value = normalizedHealth;
    }
    public void KillSelf()
    {
        _enemy.OnDeath.RemoveListener(KillSelf);
        _enemy.OnHealthChanged -= RefreshHealthOfBar;

        Destroy(gameObject);
    }
}
