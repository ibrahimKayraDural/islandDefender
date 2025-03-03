using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TowerDefence
{
    public abstract class TurretUnit : MonoBehaviour, IHealth
    {
        public TurretData Data => _data;
        public float MaxHealth => _MaxHealth;
        public float Health => _health;

        [SerializeField] internal List<Renderer> _Renderers;
        [SerializeField] internal float _DeathDuration = .5f;

        internal float _MaxHealth;
        internal TurretData _data;
        internal TowerDefenceTileScript _parentTile;
        internal float _health;
        internal bool _isInitialized;
        internal bool _isDead;

        virtual public void Initialize(TurretData data, TowerDefenceTileScript tile)
        {
            if (_isInitialized) return;

            _data = data;
            _MaxHealth = data.Health;
            _parentTile = tile;
            transform.parent = tile.transform;
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
            SetHealth(_MaxHealth);

            tile.SetOccupied(this);
            OnInitialized();

            _isInitialized = true;
        }

        abstract internal void OnInitialized();

        abstract internal void ActivationMethod();

        virtual public void RemoveHealth(float amount)
        {
            AudioManager.Instance.PlayClip(Data.ID + "_GettingDamaged", Data.GetDamagedSFX);
            PlayDamagedAnim();
            SetHealth(Health - amount);
        }
        virtual public void SetHealth(float setTo)
        {
            _health = Mathf.Clamp(setTo, 0, _MaxHealth);

            if (_health == 0) KillSelf();
        }
        virtual public void KillSelf()
        {
            _isDead = true;
            AudioManager.Instance.PlayClip(Data.ID + "_Death", Data.DeathSFX);
            _parentTile.UnOccupy();
            StartCoroutine(DeathAnim());
        }
        internal virtual IEnumerator DeathAnim()
        {
            const float step = .05f;
            float progress = 0;
            List<Material> mats = _Renderers.Select(x => x.material).ToList();
            foreach (var m in mats) m.SetFloat("_DamageFill", 0);

            while (progress < _DeathDuration)
            {
                yield return new WaitForSeconds(step);
                progress += step;
                foreach (var m in mats) m.SetFloat("_Fill", progress / _DeathDuration);
            }

            foreach (var m in mats) m.SetFloat("_Fill", 1);
            Destroy(gameObject);
        }
        internal virtual void PlayDamagedAnim()
        {
            List<Material> mats = _Renderers.Select(x => x.material).ToList();
            foreach (var m in mats) m.SetFloat("_DamageFill", 0);

            if (HANDLE_GetDamaged != null) StopCoroutine(HANDLE_GetDamaged);
            HANDLE_GetDamaged = IENUM_GetDamaged(mats);
            StartCoroutine(HANDLE_GetDamaged);
        }
        internal IEnumerator HANDLE_GetDamaged = null;
        internal virtual IEnumerator IENUM_GetDamaged(List<Material> mats)
        {
            float step = .2f;
            float speed = 20;
            float progress = 0;

            while (progress < 2)
            {
                progress += step;
                foreach (var m in mats) m.SetFloat("_DamageFill", progress);
                yield return new WaitForSeconds(step / speed);
            }

            foreach (var m in mats) m.SetFloat("_DamageFill", 0);
        }
    }
}
