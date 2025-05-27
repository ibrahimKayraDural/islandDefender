using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeEffect : MonoBehaviour
{
    [SerializeField] CanvasGroup _CanvasGroup;
    [SerializeField] AnimationCurve _Curve = new(new Keyframe[] { new(0, 0), new(1, 1) });
    [Tooltip("Enables/Disables if the canvas group will be intearactable and raycast blocking at the end")]
    [SerializeField] bool _SetCanvasGroupSolidity = true;
    [Space(15)]
    [SerializeField] UnityEvent _OnFullyFadedIn;
    [SerializeField] UnityEvent _OnFullyFadedOut;

    void Awake()
    {
        if (_CanvasGroup == null) _CanvasGroup = GetComponent<CanvasGroup>();
        if (_CanvasGroup == null)
        {
            Debug.LogError("Canvas group was not assigned and is not existent on this object." +
                " You have to make sure one of them is done in order for this script to work.");
            enabled = false;
        }
    }

    public void SetFade(bool fadeIn, float durationSeconds)
    {
        if (IENUMFade_Handle != null) StopCoroutine(IENUMFade_Handle);
        IENUMFade_Handle = IENUMFade(fadeIn, durationSeconds);
        StartCoroutine(IENUMFade_Handle);
    }
    IEnumerator IENUMFade_Handle = null;
    IEnumerator IENUMFade(bool fadeIn, float durationSeconds)
    {
        float sample = fadeIn ? 0 : 1;
        _CanvasGroup.alpha = sample;

        const float stepDuration = 0.05f;
        int stepCount = Mathf.FloorToInt(durationSeconds / stepDuration);
        float stepFillAmnt = (1f / ((float)stepCount)) * (fadeIn ? 1 : -1);

        for (int i = 0; i < stepCount; i++)
        {
            sample += stepFillAmnt;
            _CanvasGroup.alpha = _Curve.Evaluate(sample);
            yield return new WaitForSeconds(stepDuration);
        }

        _CanvasGroup.alpha = fadeIn ? 1 : 0;
        if (_SetCanvasGroupSolidity)
        {
            _CanvasGroup.blocksRaycasts = fadeIn;
            _CanvasGroup.interactable = fadeIn;
        }

        if (fadeIn) _OnFullyFadedIn?.Invoke();
        else _OnFullyFadedOut?.Invoke();
    }
}
