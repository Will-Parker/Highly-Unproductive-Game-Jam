using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static Vector2 Vec3ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 Vec2ToVec3(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0f);
    }

    public static T MaxObject<T, U>(this IEnumerable<T> source, Func<T, U> selector)
      where U : IComparable<U>
    {
        if (source == null) throw new ArgumentNullException("source");
        bool first = true;
        T maxObj = default(T);
        U maxKey = default(U);
        foreach (var item in source)
        {
            if (first)
            {
                maxObj = item;
                maxKey = selector(maxObj);
                first = false;
            }
            else
            {
                U currentKey = selector(item);
                if (currentKey.CompareTo(maxKey) > 0)
                {
                    maxKey = currentKey;
                    maxObj = item;
                }
            }
        }
        if (first) throw new InvalidOperationException("Sequence is empty.");
        return maxObj;
    }

    // https://stackoverflow.com/questions/1101841/how-to-perform-max-on-a-property-of-all-objects-in-a-collection-and-return-th
    public static T MinObject<T, U>(this IEnumerable<T> source, Func<T, U> selector)
      where U : IComparable<U>
    {
        if (source == null) throw new ArgumentNullException("source");
        bool first = true;
        T minObj = default(T);
        U minKey = default(U);
        foreach (var item in source)
        {
            if (first)
            {
                minObj = item;
                minKey = selector(minObj);
                first = false;
            }
            else
            {
                U currentKey = selector(item);
                if (currentKey.CompareTo(minKey) < 0)
                {
                    minKey = currentKey;
                    minObj = item;
                }
            }
        }
        if (first) throw new InvalidOperationException("Sequence is empty.");
        return minObj;
    }
}
