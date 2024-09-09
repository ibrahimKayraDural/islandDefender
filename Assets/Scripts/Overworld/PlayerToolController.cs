namespace Overworld
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class PlayerToolController : MonoBehaviour
    {
        public List<ToolData> ActiveTools
        {
            get
            {
                List<ToolData> temp = new List<ToolData>();
                _activeTools.ForEach(x => temp.Add(x.Data));
                return temp;
            }
        }
        public int MaxToolCount => _MaxToolCount;

        [SerializeField] Transform _ToolPoint;
        [SerializeField] int _MaxToolCount = 3;


        // To change the gun, simply change this value. The value will automatically normalize itself to _ActiveTools array.
        int _ToolIndex
        {
            get => AUTOVALUE_toolIdx;
            set
            {
                if (_activeTools.Count <= 0) return;

                 _currentTool?.Unequip();

                if (value >= _activeTools.Count) AUTOVALUE_toolIdx = 0;
                else if (value < 0) AUTOVALUE_toolIdx = _activeTools.Count - 1;
                else AUTOVALUE_toolIdx = value;

                _currentTool.Equip();
            }
        }
        Tool _currentTool
        {
            get
            {
                if (_activeTools.Count <= 0) return null;
                try { return _activeTools[_ToolIndex]; }
                catch { return null; }
            }
        }

        int AUTOVALUE_toolIdx = 0;

        ToolDatabase _toolDatabase;
        List<Tool> _activeTools = new List<Tool>();
        List<Tool> _tools = new List<Tool>();
        List<KeyCode> _numberKeys = GLOBAL.AlphaNumberKeys;
        bool _anyToolIsEquipped => _currentTool != null;
        float _changeTool_TargetTime = -1;
        float _changeTool_Cooldown = .2f;

        void Start()
        {
            _toolDatabase = GLOBAL.GetToolDatabase();
            InitializeToolList();
            CanvasManager.e_OnCurrentInterfaceChanged += OnCanvasInterfaceChanged;

            _ToolIndex = 0;
        }

        void OnCanvasInterfaceChanged(object sender, IUserInterface e)
        {
            if (e != null) _currentTool?.StopFiring();
        }

        void InitializeToolList()
        {
            List<Tool> toolRefs = _toolDatabase.ToolList;

            foreach (var toolRef in toolRefs)
            {
                _tools.Add(toolRef.InstantiatePrefab(_ToolPoint));
            }
        }

        void Update()
        {
            if (Time.timeScale <= 0) return;

            if (CanvasManager.SomethingIsOpen == false)
            {
                if (Input.GetButtonDown("Fire1")) _currentTool?.StartFiring();
                else if (Input.GetButtonUp("Fire1")) _currentTool?.StopFiring();

                CheckChangeTool(); 
            }
        }

        void RefreshTools() => _ToolIndex = _ToolIndex;

        public void ActivateTool_ViaButton(string idOrName) => TryActivateTool(idOrName);
        public bool TryActivateTool(string idOrName)
        {
            if (_activeTools.Count >= _MaxToolCount) return false;

            Tool tool = FindInAllTools(idOrName);
            if (tool == null) return false;

            if (_activeTools.Contains(tool) == false) _activeTools.Add(tool);

            RefreshTools();

            return true;
        }
        public bool TryDeactivateTool(string idOrName)
        {
            Tool tool = FindInActiveTools(idOrName, out int index);
            if (tool == null) return false;

            //shift index when list shifts
            if (AUTOVALUE_toolIdx >= index) AUTOVALUE_toolIdx = Mathf.Max(0, AUTOVALUE_toolIdx - 1);

            _activeTools[index].Unequip();
            _activeTools.RemoveAt(index);
            RefreshTools();

            return true;
        }
        public bool TrySwapTool(string existingTool_idOrName, string newTool_idOrName)
        {
            Tool newTool = FindInAllTools(newTool_idOrName);
            if (newTool == null) return false;

            Tool exisTool = FindInActiveTools(existingTool_idOrName, out int index);
            if (exisTool == null) return false;;

            _activeTools[index].Unequip();
            _activeTools[index] = newTool;
            return true;
        }

        void CheckChangeTool()
        {
            if (_activeTools.Count <= 1) return;

            float v = Input.GetAxisRaw("ChangeGun");

            int number = -1;

            if(Input.anyKeyDown)
            {
                foreach (var key in _numberKeys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        string keyStr = key.ToString();

                        if (keyStr.Contains("Alpha"))
                        {
                            try { number = int.Parse(key.ToString().Replace("Alpha", "")); }
                            catch { number = -1; };
                        }

                        if (number != -1) break;
                    }
                }
            }

            if (v != 0)
            {
                if (_changeTool_TargetTime >= Time.time) return;

                v = v > 0 ? 1 : -1;
                _ToolIndex += (int)v;

                _changeTool_TargetTime = Time.time + _changeTool_Cooldown;
            }
            else if (number != -1)
            {
                if (number == 0)
                { number = 10; }
                number--;

                if (_ToolIndex == number) return;
                if (number >= _activeTools.Count) return;

                _ToolIndex = number;
            }
        }

        Tool FindInAllTools(string idOrDisplayName) => _tools.Find(x => x.Data.ID == idOrDisplayName || x.Data.DisplayName == idOrDisplayName);
        Tool FindInActiveTools(string idOrDisplayName) => _activeTools.Find(x => x.Data.ID == idOrDisplayName || x.Data.DisplayName == idOrDisplayName);
        Tool FindInAllTools(string idOrDisplayName, out int index)
        {
            index = _tools.FindIndex(x => x.Data.ID == idOrDisplayName || x.Data.DisplayName == idOrDisplayName);
            return index == -1 ? null : _tools[index];
        }
        Tool FindInActiveTools(string idOrDisplayName, out int index)
        {
            index = _activeTools.FindIndex(x => x.Data.ID == idOrDisplayName || x.Data.DisplayName == idOrDisplayName);
            return index == -1 ? null : _activeTools[index];
        }
    }
}
