using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteScreenController : MonoBehaviour
{
    [SerializeField] Animator _Animator;
    [SerializeField] float _FullscreenHoldDuration = .5f;
    [SerializeField] AudioClip WooshSFX;
    [SerializeField] AudioClip PhoneUpSFX;
    [SerializeField] AudioClip PhoneDownSFX;
    [SerializeField] AudioClip UIOpenSFX;
    AudioManager _audioManager => AudioManager.Instance;

    readonly string PhoneUp_ID = "Phone_Up_Screen_Controller";
    readonly string PhoneDown_ID = "Phone_Down_Screen_Controller";
    readonly string UIOpen_ID = "UIOpen_Screen_Controller";
    BattleManager _battleManager
    {
        get
        {
            if (AUTO_battleManager == null)
                AUTO_battleManager = BattleManager.Instance;
            return AUTO_battleManager;
        }
    }
    BattleManager AUTO_battleManager = null;

    bool _isSmallScreenOpen = false;
    bool _inputNeedsReset = false;
    float _buttonDownTimer = float.MaxValue;

    void Awake()
    {
        CanvasManager.e_OnCurrentInterfaceChanged += OnInterfaceChanged;
    }
    void Update()
    {
        bool timerIsDone = _buttonDownTimer <= 0;
        if (timerIsDone || Input.GetButtonUp("RemoteScreen"))
        {
            if (timerIsDone)
            {
                _audioManager.PlayClip(UIOpen_ID, UIOpenSFX);
                _isSmallScreenOpen = false;
                _Animator.SetInteger("Status", 2);

                _buttonDownTimer = float.MaxValue;
                _inputNeedsReset = true;
            }
            else if (_inputNeedsReset == false) SetSmallScreenEnablity(!_isSmallScreenOpen);
        }

        if (Input.GetButtonDown("RemoteScreen"))
        {
            _buttonDownTimer = _FullscreenHoldDuration;
            _inputNeedsReset = false;
            _audioManager.PlayClip(PhoneUp_ID, PhoneUpSFX);
        }
        else if (Input.GetButton("RemoteScreen") && _inputNeedsReset == false) _buttonDownTimer -= Time.deltaTime;
    }

    void OnInterfaceChanged(object sender, IUserInterface e)
    {
        if (e != null) SetSmallScreenEnablity(false);
    }

    public void SetSmallScreenEnablity(bool setTo)
    {
        if (CanvasManager.SomethingIsOpen) setTo = false;


        _Animator.SetInteger("Status", setTo ? 1 : 0);
        _isSmallScreenOpen = setTo;
    }

    public void EnterBattleScreen()
    {
        _battleManager.EnterBattle(TowerDefence.TowerDefenceControlMode.Remote);
    }
}
