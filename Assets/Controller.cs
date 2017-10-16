using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public GameObject realPlane;

    uint mapWidth = 10;
    uint mapHeight = 10;
    Plane mousePlane;
    Camera mainCamera;

    void Start( ) {
        mousePlane = new Plane(Vector3.up, Vector3.zero);
        realPlane.transform.position = new Vector3(mapWidth / 2f, 0f, mapHeight / 2f);
        realPlane.transform.localScale = new Vector3(mapWidth / 10f, 1f, mapHeight / 10f);
        realPlane.GetComponent<Renderer>( ).material.mainTextureScale = new Vector2(mapWidth / 2f, mapHeight / 2f);
        mainCamera = Camera.main;
        mainCamera.transform.position =
            new Ray(realPlane.transform.position, new Vector3(0f, 1f, -1f)).GetPoint(Mathf.Max(mapWidth, mapHeight));
        mainCamera.transform.rotation = Quaternion.Euler(45f, 0f, 0f);
    }

    void Update( ) {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (mousePlane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            int x = Mathf.FloorToInt(hitPoint.x);
            int z = Mathf.FloorToInt(hitPoint.z);
            transform.position = new Vector3(x + 0.5f, 0.25f, z + 0.5f);
        }
    }
}
