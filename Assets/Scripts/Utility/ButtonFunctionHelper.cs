using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonFunctionHelper : MonoBehaviour
{
    [SerializeField] UnityEvent _EventToRun;
    [SerializeField] List<AYellowpaper.SerializedCollections.SerializedKeyValuePair<string,UnityEvent>> namedEvent2Run;

    public void InvokeEvents() => _EventToRun?.Invoke();
    public void InvokeEventsWithName(string eventName)
    {
        int temp = namedEvent2Run.FindIndex(x => x.Key == eventName);
        if (temp == -1) { return; }
        namedEvent2Run[temp].Value?.Invoke();
    }

}
