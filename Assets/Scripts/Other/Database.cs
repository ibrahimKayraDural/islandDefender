using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Database<T> : ScriptableObject where T : Data<T>
{
    public List<T> DataList => GetDataAsType();
    [SerializeField] internal List<Data<T>> _DataList = new List<Data<T>>();

    public T GetDataByID(string id)
    {
        if (id == null) return null;

        return DataList.Find(x => x.ID == id);
    }
    public T GetDataByDisplayName(string displayName) => DataList.Find(x => x.DisplayName == displayName);

    List<T> GetDataAsType()
    {
        List<T> cleanList = new List<T>();
        List<Data<T>> dataTrack = new List<Data<T>>();
        foreach (var item in _DataList)
        {
            if(item is T)
            {
                cleanList.Add(item as T);
            }
            else
            {
                dataTrack.Add(item);
            }
        }
        dataTrack.ForEach(x => _DataList.Remove(x));

        return cleanList;
    }
}
