﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public GameObject realPlane;

    uint mapWidth = 10;
    uint mapHeight = 10;
    Plane mousePlane;
    Camera mainCam;
    Vector3 mousePos3d;
    Vector2 prevMousePos;
    bool canDrag = false;
    bool canRotate = false;
    Vector3 lastHitPoint = Vector3.zero;

    void Start( ) {
        mousePlane = new Plane(Vector3.up, Vector3.zero);
        realPlane.transform.position = new Vector3(mapWidth / 2f, 0f, mapHeight / 2f);
        realPlane.transform.localScale = new Vector3(mapWidth / 10f, 1f, mapHeight / 10f);
        Material realPlaneMaterial = realPlane.GetComponent<Renderer>( ).material;
        realPlaneMaterial.mainTextureScale = new Vector2(mapWidth, mapHeight);
        realPlaneMaterial.mainTextureOffset = new Vector2(0.5f, 0.5f);
        mainCam = Camera.main;
        float cameraDistance = Mathf.Max(mapWidth, mapHeight);
        mainCam.transform.position = realPlane.transform.position - new Vector3(0, 0, cameraDistance);
        mainCam.transform.rotation = Quaternion.identity;
        mainCam.transform.RotateAround(realPlane.transform.position, Vector3.right, 80f);
    }

    void Update( ) {
        Vector2 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        float mouseScroolWheelAxis = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(mouseScroolWheelAxis) > 0f) {
            Vector3 mainCamPositionBackup = mainCam.transform.position;
            mainCam.transform.position += mouseScroolWheelAxis * 2f * ray.direction;
            if (mainCam.transform.position.y < 1f) {
                mainCam.transform.position = mainCamPositionBackup;
            }
        }
        float distance;
        if (mousePlane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            int x = Mathf.FloorToInt(hitPoint.x);
            int z = Mathf.FloorToInt(hitPoint.z);
            bool mouseOverMap;
            if (mouseOverMap = x >= 0 && x < mapWidth && z >= 0 && z < mapHeight) {
                lastHitPoint = hitPoint;
            }
            if (Input.GetMouseButtonDown(0) && mouseOverMap) {
                mousePos3d = ray.GetPoint(distance);
                mousePos3d.y = 0;
                canDrag = true;
            } else if (Input.GetMouseButton(0) && canDrag) {
                Vector3 mouse3DTranslation = ray.GetPoint(distance);
                mouse3DTranslation.y = 0;
                mainCam.transform.position -= (mouse3DTranslation - mousePos3d);
            } else {
                canDrag = false;
                if (Input.GetMouseButtonDown(1) && mouseOverMap) {
                    canRotate = true;
                }
                if (Input.GetMouseButton(1) && canRotate) {
                    mainCam.transform.RotateAround(lastHitPoint, Vector3.up, mousePos.x - prevMousePos.x);
                    Vector3 mainCamPositionBackup = mainCam.transform.position;
                    Quaternion mainCamRotationBackup = mainCam.transform.rotation;
                    mainCam.transform.RotateAround(
                        lastHitPoint, Vector3.Cross(Vector3.up, mainCam.transform.rotation * Vector3.up), mousePos.y - prevMousePos.y);
                    if (mainCam.transform.rotation.eulerAngles.x < 30f || mainCam.transform.position.y < 1f) {
                        mainCam.transform.position = mainCamPositionBackup;
                        mainCam.transform.rotation = mainCamRotationBackup;
                    }
                    //mainCam.transform.RotateAround(hitPoint, new Vector3(
                    //    mousePos.y - prevMousePos.y,
                    //    mousePos.x - prevMousePos.x,
                    //    0f), (mousePos - prevMousePos).sqrMagnitude);
                } else {
                    canRotate = false;
                    transform.position = new Vector3(x + 0.5f, 0, z + 0.5f);
                }
            }
        }
        prevMousePos = mousePos;
    }
}
