using System;
using Random = UnityEngine.Random;

[Serializable]
public class Range
{
    public float min;
    public float max;

    public Range(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float ValorAleatorio => Random.Range(min, max);
}
