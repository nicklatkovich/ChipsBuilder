using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    public static T[ ][ ] Init2DArray<T>(uint width, uint height, T defaultValue) {
        T[ ][ ] result = new T[width][ ];
        for (uint x = 0; x < width; x++) {
            result[x] = new T[height];
            for (uint y = 0; y < height; y++) {
                result[x][y] = defaultValue;
            }
        }
        return result;
    }

    public static Vector2 ToVector2XZ(this Vector3 v) {
        return new Vector2(v.x, v.z);
    }

    public static Point Round(this Vector2 v) {
        return new Point(
            Mathf.RoundToInt(v.x),
            Mathf.RoundToInt(v.y)
        );
    }

    public static T Set<T>(this T[ ][ ] array, Point position, T value) {
        return array[position.X][position.Y] = value;
    }

    public static T Set<T>(this T[ ][ ] array, Vector2 position, T value) {
        return array.Set(position.Round( ), value);
    }

    public static Vector2 Add(this Vector2 v, float value) {
        return new Vector2(v.x + value, v.y + value);
    }

    public static Vector3 Add(this Vector3 v, float value) {
        return new Vector3(v.x + value, v.y + value, v.z + value);
    }

    public static float GetAtan2(this Vector2 v) {
        return Mathf.Atan2(v.y, v.x);
    }

    public static Vector3 ToVector3XZ(this Vector2 v, float y = 0) {
        return new Vector3(v.x, y, v.y);
    }

    public static float GetAtan2Deg(this Vector2 v) {
        return v.GetAtan2( ) * Mathf.Rad2Deg;
    }

}
