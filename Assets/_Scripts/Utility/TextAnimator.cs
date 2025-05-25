using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimator : MonoBehaviour
{
    public string CurrentText;

    [SerializeField] TextMeshProUGUI _OutputTM;
    [SerializeField, Min(.01f)] float _DurationPerLetter = .05f;
    [SerializeField] bool _PlayOnAwake;

    void Awake()
    {
        if (_PlayOnAwake) Play();
    }

    [ContextMenu("Play")]
    public void Play()
    {
        StopCoroutine(nameof(IENUMPlay));
        StartCoroutine(nameof(IENUMPlay));
    }

    IEnumerator IENUMPlay()
    {
        _OutputTM.text = "";

        var text = CurrentText;
        foreach (var c in text)
        {
            _OutputTM.text += c;
            yield return new WaitForSeconds(_DurationPerLetter);
        }

        _OutputTM.text = text;
    }
}
