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

}
