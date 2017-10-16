using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    uint mapWidth = 10u;
    uint mapHeight = 10u;
    Plane plane;

    void Start( ) {
        plane = new Plane(Vector3.up, new Vector3(mapWidth / 2f, 0, mapHeight / 2f));
    }

    void Update( ) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            int x = Mathf.FloorToInt(hitPoint.x);
            int z = Mathf.FloorToInt(hitPoint.z);
            transform.position = new Vector3(x + 0.5f, 0.25f, z + 0.5f);
        }
    }
}
