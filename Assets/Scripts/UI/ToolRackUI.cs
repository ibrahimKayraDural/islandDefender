namespace GameUI
{
    using Overworld;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;

    public class ToolRackUI : ProximityInteractableUI, IUserInterface, IUICellOwner
    {
        public ToolRack _currentRack => CurrentPI as ToolRack;

        [SerializeField] GameObject _VisualParent;
        [SerializeField] Transform _RackCellParent;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;

        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;

        List<ToolData> _allToolDatas { get => _gameplayManager.UnlockedTools; }

        public UICell OldCell { get; set; }
        public UICell CurrentCell { get; set; }
        GraphicRaycasterScript IUICellOwner.GraphicRaycasterS => _GraphicRaycaster;
        TextMeshProUGUI IUICellOwner.DescriptionTitle => _DescriptionTitle;
        TextMeshProUGUI IUICellOwner.DescriptionText => _DescriptionText;

        List<ToolData> _playerActiveTools = new List<ToolData>();
        PlayerToolController _playerToolController = null;
        GameplayManager _gameplayManager = null;

        const string INVENTORY_ID = "inventory";
        const string RACK_ID = "rack";

        void Start()
        {
            StartCoroutine(DelayedStart(.2f));
        }
        IEnumerator DelayedStart(float delay)
        {
            yield return new WaitForSeconds(delay);

            _playerToolController = PlayerInstance.Instance.PlayerToolController_Ref;
            _gameplayManager = GameplayManager.Instance;

            Refresh();
        }

        void Refresh()
        {
            _playerActiveTools = _playerToolController.ActiveTools;

            foreach (Transform child in _RackCellParent) Destroy(child.gameObject);
            foreach (Transform child in _InventoryCellParent) Destroy(child.gameObject);

            foreach (var data in _allToolDatas)
            {
                bool activateCell = _playerActiveTools.Find(x => x.ID == data.ID) == null;
                InstantiateCell(data, _RackCellParent, activateCell, RACK_ID);
            }

            for (int i = 0; i < _playerToolController.MaxToolCount; i++)
            {
                if(i < _playerActiveTools.Count)
                {
                    InstantiateCell(_playerActiveTools[i], _InventoryCellParent, true, INVENTORY_ID);
                }
                else
                {
                    InstantiateCell(null, _InventoryCellParent);
                }
            }

            void InstantiateCell(ToolData data, Transform parent, bool activateCell = true, string ownerID = null)
            {
                ToolCellUI temp = Instantiate(_CellPrefab, parent).GetComponent<ToolCellUI>();

                if (data == null)
                    temp.Initialize();
                else
                    temp.Initialize(data, activateCell, ownerID);
            }
        }

        void Close()
        {
            _currentRack.SetOpennes(false);
        }

        public override void OnEnablityChanged(bool changedTo)
        {
            _VisualParent.SetActive(changedTo);
        }

        internal override void OnPIUpdate_Start()
        {
            (this as IUICellOwner).OnStart();
        }

        internal override void OnPIUpdate_Loop()
        {
            (this as IUICellOwner).OnLoop();
        }

        internal override void OnPIUpdate_End()
        {
            (this as IUICellOwner).OnEnd();
        }

        void IUICellOwner.OnCellClicked(UICell cell)
        {
            if (CurrentCell.IsInteractable == false) return;

            string toolID = (cell as ToolCellUI).CellData.ID;
            bool refreshNeeded;
            if (CurrentCell.OwnerID == INVENTORY_ID)
            {
                refreshNeeded = _playerToolController.TryDeactivateTool(toolID);
            }
            else
            {
                refreshNeeded = _playerToolController.TryActivateTool(toolID);
            }

            if (refreshNeeded) Refresh();
        }

        bool IUICellOwner.CellIsValid(UICell cell)
        {
            ToolCellUI newCell = cell as ToolCellUI;
            return newCell.CellData != null;
        }
    }

}