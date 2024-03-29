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

    public static Vector2 Vec2IntToVec2(Vector2Int v)
    {
        return new Vector2(v.x + 0.5f, v.y + 0.5f);
    }

    public static Vector2Int Vec2ToVec2Int(Vector2 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x - 0.5f), Mathf.RoundToInt(v.y - 0.5f));
    }

    public static Vector2Int Vec3ToVec2Int(Vector3 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x - 0.5f), Mathf.RoundToInt(v.y - 0.5f));
    }

    public static Vector3Int Vec3ToVec3Int(Vector3 v)
    {
        return new Vector3Int(Mathf.RoundToInt(v.x - 0.5f), Mathf.RoundToInt(v.y - 0.5f));
    }
    public static Vector3Int Vec2ToVec3Int(Vector2 v)
    {
        return new Vector3Int(Mathf.RoundToInt(v.x - 0.5f), Mathf.RoundToInt(v.y - 0.5f));
    }

    public static Vector3 Vec2IntToVec3(Vector2Int v)
    {
        return new Vector3(v.x + 0.5f, v.y + 0.5f, 0f);
    }

    public static Vector2Int Vec3IntToVec2Int(Vector3Int v)
    {
        return new Vector2Int(v.x, v.y);
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

    public static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }

    public static int ManhattanDistance(Vector2Int a, Vector2Int b)
    {
        checked
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }

}
