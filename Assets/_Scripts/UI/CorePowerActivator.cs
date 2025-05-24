using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CorePowers;
using TMPro;
using TowerDefence;
using System;
using UnityEngine.ProBuilder.MeshOperations;
using Unity.Burst.CompilerServices;
using System.Drawing;

public class CorePowerActivator : MonoBehaviour
{
    [SerializeField] Button _Button;
    [SerializeField] TextMeshProUGUI CountTM;
    [SerializeField] CoreManager _CoreManager;
    [SerializeField] TowerDefenceGridManager _GridManager;
    [SerializeField] GameObject VisualParent;
    [SerializeField] Image Icon;
    [SerializeField] LayerMask _TileMask = 1 << 6;

    Transform _mouseTracker = null;

    void Start()
    {
        _mouseTracker = TDPlayerController.Instance.MouseTracker;
    }

    public void Activate()
    {
        CorePower_Base power = _CoreManager.SelectedCorePower;
        if (power == null) return;

        if (power as CorePower_AtRandomTiles != null)
        {
            var temp = power as CorePower_AtRandomTiles;
            List<TowerDefenceTileScript> tiles = _GridManager.GetRandomFreeTiles(temp.TileCount);

            if (power.TryActivate(new[] { tiles as object })) Refresh(power);
        }
        else if (power as CorePower_OnPoint != null && _mouseTracker != null)
        {
            SetButtonEnablity(false);
            AwaitMouseClick(OnClicked: OnClickCell, OnCanceled: EnableButton);

            void OnClickCell()
            {
                Ray ray = new Ray(_mouseTracker.position + Vector3.up, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, 10, _TileMask))
                {
                    //_Animator.SetTrigger("shoot");
                    Vector3 point = hit.collider.transform.position;
                    if (power.TryActivate(new[] { point as object })) Refresh(power);
                }
            }
        }
        else if (power.TryActivate(null)) Refresh(power);
    }
    public void Refresh(CorePower_Base corePower)
    {
        if (corePower == null)
        {
            VisualParent.SetActive(false);
            return;
        }

        VisualParent.SetActive(true);

        Icon.sprite = corePower.Data.UISprite;
        var usagesLeft = corePower.UsagesLeft;
        CountTM.text = usagesLeft.ToString();
        SetButtonEnablity(usagesLeft > 0);
    }

    void EnableButton() => SetButtonEnablity(true);
    void SetButtonEnablity(bool setTo)
    {
        _Button.interactable = setTo;
    }
    void AwaitMouseClick(Action OnClicked = null, Action OnCanceled = null)
    {
        StopCoroutine(nameof(IENUM_AwaitMouseClick));
        StartCoroutine(IENUM_AwaitMouseClick(OnClicked, OnCanceled));
    }
    void CancelMouseClick()
    {
        StopCoroutine(nameof(IENUM_AwaitMouseClick));
        SetButtonEnablity(true);
    }
    IEnumerator IENUM_AwaitMouseClick(Action OnClicked = null, Action OnCanceled = null)
    {
        int index = -1;

        while (index < 0)
        {
            if (Input.GetMouseButtonDown(0)) index = 0;
            else if (Input.GetMouseButtonDown(1)) index = 1;

            yield return null;
        }

        if (index == 0 && OnClicked != null) OnClicked();
        else if (index == 1 && OnCanceled != null) OnCanceled();
    }
}
