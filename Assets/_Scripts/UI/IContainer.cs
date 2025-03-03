using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContainer<T>
{
    List<T> Items { get; }

    T AddWithSpill(T item);
    int CheckEmptySpaceFor(T item);
    bool TryAddItemFully(T item);
    void RemoveAtIndex(int index);
    void Clean();
    void SetItemAtIndex(int index, T setTo);
}
