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

}
