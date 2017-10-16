using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public GameObject realPlane;

    uint mapWidth = 10;
    uint mapHeight = 10;
    Plane mousePlane;
    Camera mainCamera;
    float cameraDistance;
    Vector2 cameraPosition;
    Vector2 prevMousePosition = Vector2.zero;

    void Start( ) {
        mousePlane = new Plane(Vector3.up, Vector3.zero);
        realPlane.transform.position = new Vector3(mapWidth / 2f, 0f, mapHeight / 2f);
        realPlane.transform.localScale = new Vector3(mapWidth / 10f, 1f, mapHeight / 10f);
        Material realPlaneMaterial = realPlane.GetComponent<Renderer>( ).material;
        realPlaneMaterial.mainTextureScale = new Vector2(mapWidth, mapHeight);
        realPlaneMaterial.mainTextureOffset = new Vector2(0.5f, 0.5f);
        mainCamera = Camera.main;
        cameraDistance = Mathf.Max(mapWidth, mapHeight);
        cameraPosition = realPlane.transform.position;
    }

    void Update( ) {
        Vector2 mousePosition = Input.mousePosition;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            cameraDistance -= 0.5f;
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            cameraDistance += 0.5f;
        }
        if (Input.GetMouseButton(2)) {
            cameraPosition -= (mousePosition - prevMousePosition) / 512f * cameraDistance;
        }
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (mousePlane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            int x = Mathf.FloorToInt(hitPoint.x);
            int z = Mathf.FloorToInt(hitPoint.z);
            transform.position = new Vector3(x + 0.5f, 0, z + 0.5f);
        }
        Vector3 cameraCenterPosition = new Vector3(cameraPosition.x, 0, cameraPosition.y);
        mainCamera.transform.position = cameraCenterPosition - new Vector3(0, 0, cameraDistance);
        mainCamera.transform.rotation = Quaternion.identity;
        mainCamera.transform.RotateAround(cameraCenterPosition, Vector3.right, 80f);
        prevMousePosition = Input.mousePosition;
    }
}
