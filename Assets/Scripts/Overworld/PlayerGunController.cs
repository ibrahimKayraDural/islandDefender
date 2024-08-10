namespace Overworld
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerGunController : MonoBehaviour
    {
        [SerializeField] Transform _GunPoint;
        [SerializeField] List<Tool> _Guns = new List<Tool>();

        // this is an auto value. do not touch this value. >:(
        int AUTOVALUE_gunIdx = 0;

        // to change the gun, simply change this value. The value will automatically normalize itself to _Guns array.
        int _gunIndex
        {
            get => AUTOVALUE_gunIdx;
            set
            {
                if (_Guns.Count <= 0)
                {
                    Debug.LogError("No guns are in the _Guns list");
                    return;
                }

                _currentGun?.Unequip();

                if (value >= _Guns.Count) AUTOVALUE_gunIdx = 0;
                else if (value < 0) AUTOVALUE_gunIdx = _Guns.Count - 1;
                else AUTOVALUE_gunIdx = value;

                _currentGun.Equip();
            }
        }
        Tool _currentGun
        {
            get
            {
                if (_Guns.Count <= 0) return null;
                try { return _Guns[_gunIndex]; }
                catch { return null; }
            }
        }
        bool _anyGunIsEquipped => _currentGun != null;
        float _changeGun_TargetTime = -1;
        float _changeGun_Cooldown = .2f;

        void Start()
        {
            List<Tool> temp = new List<Tool>();

            foreach (var item in _Guns)
            {
                foreach (var typeItem in temp)
                {
                    if (typeItem.GetType().IsEquivalentTo(item.GetType())) goto Checkpoint1;
                }

                Tool gun = Instantiate(item.gameObject).GetComponent<Tool>();
                gun.Initialize(_GunPoint);
                temp.Add(gun);

            Checkpoint1:;
            }

            _Guns = temp;

            _gunIndex = 0;
        }
        void Update()
        {
            if (Input.GetButtonDown("Fire1")) _currentGun?.StartFiring();
            else if (Input.GetButtonUp("Fire1")) _currentGun?.StopFiring();

            TryChangeGun(Input.GetAxisRaw("ChangeGun"));
        }

        void TryChangeGun(float v)
        {
            if (v == 0) return;
            if (_changeGun_TargetTime >= Time.time) return;

            v = v > 0 ? 1 : -1;
            _gunIndex += (int)v;

            _changeGun_TargetTime = Time.time + _changeGun_Cooldown;
        }
    }
}
