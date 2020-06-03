using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public static class EnumerableExtensions
{
    public static T RandomElement<T>(this IEnumerable<T> collection)
    {
        var array = collection.ToArray();

        var count = array.Length;

        if (count == 0)
        {
            return default;
        }

        var randomIndex = Random.Range(0, count);

        return array.ToArray()[randomIndex];
    }
}
