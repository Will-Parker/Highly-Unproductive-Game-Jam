using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers 
{
    public static Vector2 Vec3ToVec2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3 Vec2ToVec3(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0f);
    }
}
