using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using LimitedQueue;

public class InfoLogUI : MonoBehaviour
{
    public static InfoLogUI Instance { get; private set; } = null;

    [Header("Debugging")]
    [SerializeField] string _TextToWrite;
    [SerializeField] bool Button_Write;
    [SerializeField] bool Button_OpenForDuration;
    [SerializeField] bool Button_Open;
    [SerializeField] bool Button_Close;

    [Space(10)]
    [Header("Values")]
    [SerializeField] float _ShowDuration = 1f;
    [SerializeField] int _MessageCapacity = 20;
    [SerializeField] Color _DefaultColor = Color.white;

    [Space(10)]
    [Header("References")]
    [SerializeField] Transform _VisualParent;
    [SerializeField] Transform _LogParent;

    Queue<GameObject> _log;
    bool _invalidateDurationDisable = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);

        _log = new Queue<GameObject>();
        CanvasManager.e_OnCurrentInterfaceChanged += OnCanvasManagerInterfaceChanged;
    }

    void OnCanvasManagerInterfaceChanged(object sender, IUserInterface e)
    {
        if (e != null) SetEnablity(false);
    }

    private void OnValidate()
    {
        if(Button_Write == true)
        {
            Button_Write = false;

            if (_TextToWrite == "") return;
            AddToLog(_TextToWrite);
            _TextToWrite = "";
        }

        if (Button_OpenForDuration == true)
        {
            Button_OpenForDuration = false;

            SetEnablity(true, true);
        }

        if(Button_Open)
        {
            Button_Open = false;
            SetEnablity(true);
        }

        if (Button_Close)
        {
            Button_Close = false;
            SetEnablity(false);
        }
    }

    public void AddToLog(string text, Color? color = null)
    {
        GameObject obj = new GameObject("LogMessage", new System.Type[] { typeof(TextMeshProUGUI) });
        TextMeshProUGUI tm = obj.GetComponent<TextMeshProUGUI>();

        obj.transform.SetParent(_LogParent);
        tm.text = text;
        tm.color = color.HasValue ? color.Value : _DefaultColor;

        AddToQueue(obj);

        SetEnablity(true, true);
    }

    public void SetEnablity(bool setTo, bool closeAfterDuration = false)
    {
        if (_log.Count <= 0) setTo = false;
        if (CanvasManager.SomethingIsOpen == true) setTo = false;

        if (setTo == false) closeAfterDuration = false;

        _VisualParent.gameObject.SetActive(setTo);

        if (CloseAfterDuration_Handler != null)
            StopCoroutine(CloseAfterDuration_Handler);

        if (setTo == true && closeAfterDuration == false) _invalidateDurationDisable = true;
        if (setTo == false) _invalidateDurationDisable = false;

        if (closeAfterDuration && _invalidateDurationDisable == false)
        {
            CloseAfterDuration_Handler = CloseAfterDuration();
            StartCoroutine(CloseAfterDuration_Handler);
        }
    }

    IEnumerator CloseAfterDuration_Handler = null;
    IEnumerator CloseAfterDuration()
    {
        yield return new WaitForSeconds(_ShowDuration);
        SetEnablity(false);
        CloseAfterDuration_Handler = null;
    }

    void AddToQueue(GameObject obj)
    {
        if(_log.Count == _MessageCapacity)
        {
            Destroy(_log.Dequeue());
        }

        _log.Enqueue(obj);
    }
}
