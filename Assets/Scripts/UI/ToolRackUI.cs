namespace GameUI
{
    using Overworld;
    using System.Collections;
    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ToolRackUI : MonoBehaviour, IUserInterface
    {
        public ToolRack CurrentRack { get; private set; } = null;
        public bool IsOpen { get; set; } = false;

        [SerializeField] GameObject _VisualParent;
        [SerializeField] Transform _CellParent;
        [SerializeField] GameObject _CellPrefab;
        [SerializeField] GraphicRaycasterScript _GraphicRaycaster;
        [SerializeField] TextMeshProUGUI _DescriptionTitle;
        [SerializeField] TextMeshProUGUI _DescriptionText;

        List<KeyCode> CloseKeys = new List<KeyCode>() {
            KeyCode.I
        };
        bool _breakUpdate = false;
        ToolCellUI _oldCell;

        List<ToolData> AllToolsDatas = new List<ToolData>();

        void Start()
        {
            ToolDatabase tdb = GLOBAL.GetToolDatabase();
            tdb.ToolList.ForEach(x => AllToolsDatas.Add(x.Data));

            foreach (var data in AllToolsDatas)
            {
                ToolCellUI temp = Instantiate(_CellPrefab, _CellParent).GetComponent<ToolCellUI>();
                temp.Initialize(data);
            }
        }

        public bool TrySetCurrentRack(ToolRack setTo)
        {
            if (CurrentRack == null)//chest is opened
            {
                CurrentRack = setTo;
                StartCoroutine(nameof(UpdateIEnum));
            }
            else if (setTo == null)//chest is closed
            {
                CurrentRack = null;
                _breakUpdate = true;
            }
            else
            {
                return false;
            }

            return true;
        }

        IEnumerator UpdateIEnum()
        {
            yield return new WaitForSeconds(.1f);

            _DescriptionTitle.text = "";
            _DescriptionText.text = "";

            ToolCellUI currentCell = null;

            //Update is the inside of this loop.
            //Code above will run once before the update loop.
            while (_breakUpdate == false)
            {
                //Input start
                foreach (var key in CloseKeys)
                {
                    if (Input.GetKeyDown(key))
                    {
                        Close();
                        goto InputEND;
                    }
                }
                if (Input.GetButtonDown("Interact") || Input.GetButtonDown("Exit"))
                {
                    Close();
                    goto InputEND;
                }
            InputEND:;//input end

                currentCell = null;
                RaycastResult result = _GraphicRaycaster.Raycast().Find(x => x.gameObject.TryGetComponent(out currentCell));

                //done this way to prevent Null Reference Exception
                bool targetIsValid = true;
                if (result.isValid == false || currentCell.CellData == null) targetIsValid = false;

                _DescriptionTitle.text = targetIsValid ? currentCell.CellData.DisplayName : "";
                _DescriptionText.text = targetIsValid ? currentCell.CellData.Description : "";

                if (_oldCell != currentCell)
                {
                    if (_oldCell != null) _oldCell.SetHighlight(false);
                    if (targetIsValid) currentCell.SetHighlight(true);
                }

                if (targetIsValid && Input.GetMouseButtonDown(0))
                {
                    //A CELL HAS BEEN CLICKED
                    //implement cell clicking behaviour here
                    //clicked cell is currentCell

                    Debug.Log(currentCell.CellData.DisplayName + " has been clicked");
                }

                _oldCell = currentCell;

                yield return null;
            }
            //Update is the inside of the loop above.
            //Code below will run once after the update loop has ended.

            if (currentCell != null) currentCell.SetHighlight(false);
            if (_oldCell != null) _oldCell.SetHighlight(false);

            _breakUpdate = false;
        }

        void Close()
        {
            CurrentRack.SetOpennes(false);
        }

        public void OnEnablityChanged(bool changedTo)
        {
            _VisualParent.SetActive(changedTo);
        }
        public void SetEnablityGetter(bool setTo) => (this as IUserInterface).SetEnablity(setTo);
    }

}