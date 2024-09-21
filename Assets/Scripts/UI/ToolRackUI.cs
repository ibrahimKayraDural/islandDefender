namespace GameUI
{
    using Overworld;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ToolRackUI : ProximityInteractableUI, IUserInterface
    {
        public ToolRack _currentRack => CurrentPI as ToolRack;

        [SerializeField] GameObject _VisualParent;
        [SerializeField] Transform _RackCellParent;
        [SerializeField] Transform _InventoryCellParent;
        [SerializeField] GameObject _CellPrefab;
        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;

        ToolCellUI _oldCell;

        List<ToolData> _allToolDatas { get => _gameplayManager.UnlockedTools; }
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

        ToolCellUI _currentCell = null;
        internal override void OnPIUpdate_Start()
        {
            _DescriptionTitle.text = "";
            _DescriptionText.text = "";

            _currentCell = null;
        }

        internal override void OnPIUpdate_Loop()
        {
            _currentCell = null;
            RaycastResult result = _GraphicRaycaster.Raycast().Find(x => x.gameObject.TryGetComponent(out _currentCell));

            //done this way to prevent Null Reference Exception
            bool targetIsValid = true;
            if (result.isValid == false || _currentCell.CellData == null) targetIsValid = false;

            _DescriptionTitle.text = "";
            _DescriptionText.text = "";

            if (targetIsValid && _currentCell.IsInteractable)
            {
                _DescriptionTitle.text = _currentCell.CellData.DisplayName;
                _DescriptionText.text = _currentCell.CellData.Description;
            }

            if (_oldCell != _currentCell)
            {
                if (_oldCell != null) _oldCell.SetHighlight(false);
                if (targetIsValid) _currentCell.SetHighlight(true);
            }

            if (targetIsValid && Input.GetMouseButtonDown(0))
            {
                //A CELL HAS BEEN CLICKED
                //implement cell clicking behaviour here
                //clicked cell is currentCell

                if (_currentCell.IsInteractable == false) goto ClickedEnd;

                string toolID = _currentCell.CellData.ID;
                bool refreshNeeded;
                if (_currentCell.OwnerID == INVENTORY_ID)
                {
                    refreshNeeded = _playerToolController.TryDeactivateTool(toolID);
                }
                else
                {
                    refreshNeeded = _playerToolController.TryActivateTool(toolID);
                }

                if (refreshNeeded) Refresh();

                ClickedEnd:;
            }

            _oldCell = _currentCell;
        }

        internal override void OnPIUpdate_End()
        {
            if (_currentCell != null) _currentCell.SetHighlight(false);
            if (_oldCell != null) _oldCell.SetHighlight(false);

            _currentCell = null;
        }
    }

}