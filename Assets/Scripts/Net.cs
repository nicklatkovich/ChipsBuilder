using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour {

    public Vector2 from;
    public Vector2 to;

    void Start( ) {

    }

    void Update( ) {
        transform.localScale = new Vector3((from - to).magnitude, 1, 1);
        transform.position = ((from + to) / 2f).ToVector3XZ(1f);
        transform.localRotation = Quaternion.Euler(0, -(to - from).GetAtan2Deg( ), 0);
    }
}
