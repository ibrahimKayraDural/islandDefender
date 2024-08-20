namespace Overworld
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerToolController : MonoBehaviour
    {
        [SerializeField] Transform _ToolPoint;
        [SerializeField] List<Tool> _Tools = new List<Tool>();

        // this is an auto value. do not touch this value. >:(
        int AUTOVALUE_toolIdx = 0;

        // to change the gun, simply change this value. The value will automatically normalize itself to _Guns array.
        int _toolIndex
        {
            get => AUTOVALUE_toolIdx;
            set
            {
                if (_Tools.Count <= 0)
                {
                    Debug.LogError("No guns are in the _Guns list");
                    return;
                }

                _currentTool?.Unequip();

                if (value >= _Tools.Count) AUTOVALUE_toolIdx = 0;
                else if (value < 0) AUTOVALUE_toolIdx = _Tools.Count - 1;
                else AUTOVALUE_toolIdx = value;

                _currentTool.Equip();
            }
        }
        Tool _currentTool
        {
            get
            {
                if (_Tools.Count <= 0) return null;
                try { return _Tools[_toolIndex]; }
                catch { return null; }
            }
        }
        bool _anyToolIsEquipped => _currentTool != null;
        float _changeTool_TargetTime = -1;
        float _changeTool_Cooldown = .2f;

        void Start()
        {
            InitializeToolList();
            CanvasManager.e_OnCurrentInterfaceChanged += OnCanvasInterfaceChanged;
        }

        void OnCanvasInterfaceChanged(object sender, UserInterface e)
        {
            if (e != null) _currentTool?.StopFiring();
        }

        void InitializeToolList()
        {
            List<Tool> temp = new List<Tool>();

            foreach (var item in _Tools)
            {
                foreach (var typeItem in temp)
                {
                    if (typeItem.GetType().IsEquivalentTo(item.GetType())) goto Checkpoint1;
                }

                Tool tool = Instantiate(item.gameObject).GetComponent<Tool>(); 
                tool.Initialize(_ToolPoint);
                temp.Add(tool);

            Checkpoint1:;
            }

            _Tools = temp;

            _toolIndex = 0;
        }

        void Update()
        {
            if (Time.timeScale <= 0) return;

            if (CanvasManager.SomethingIsOpen == false)
            {
                if (Input.GetButtonDown("Fire1")) _currentTool?.StartFiring();
                else if (Input.GetButtonUp("Fire1")) _currentTool?.StopFiring();

                TryChangeTool(Input.GetAxisRaw("ChangeGun")); 
            }
        }

        public bool AddTool(GameObject toolPrefab)
        {
            if (toolPrefab.TryGetComponent(out Tool tool) == false) return false;



            return true;
        }
        public void OnRemoveTool()
        {

        }

        void TryChangeTool(float v)
        {
            if (v == 0) return;
            if (_changeTool_TargetTime >= Time.time) return;

            v = v > 0 ? 1 : -1;
            _toolIndex += (int)v;

            _changeTool_TargetTime = Time.time + _changeTool_Cooldown;
        }
    }
}
