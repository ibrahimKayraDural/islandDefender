using UnityEngine;

[System.Serializable]
public class FloatRange
{
    public float Min
    {
        get => _min;
        set => _min = Mathf.Min(value, _max);
    }
    public float Max
    {
        get => _max;
        set => _max = Mathf.Max(value, _min);
    }

    [SerializeField] float _min = 0;
    [SerializeField] float _max = 0;

    public FloatRange(float min, float max)
    {
        _min = Mathf.Min(min, max);
        _max = Mathf.Max(min, max);
    }

    public float GetRandomInclusive() => Random.Range(_min, _max);
}

[System.Serializable]
public class IntRange
{
    public int Min
    {
        get => _min;
        set => _min = Mathf.Min(value, _max);
    }
    public int Max
    {
        get => _max;
        set => _max = Mathf.Max(value, _min);
    }

    [SerializeField] int _min = 0;
    [SerializeField] int _max = 0;

    public IntRange(int min, int max)
    {
        _min = Mathf.Min(min, max);
        _max = Mathf.Max(min, max);
    }

    public int GetRandomInclusive() => Random.Range(_min, _max + 1);
    public int GetRandomExclusive() => Random.Range(_min, _max);
}