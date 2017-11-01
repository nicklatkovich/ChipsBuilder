using UnityEngine;

public class Controller : MonoBehaviour {

    public GameObject realPlane;
    public AndGate andGatePrefab;

    enum State {
        Camera,
        Block,
    }

    State state = State.Camera;
    uint mapWidth = 50;
    uint mapHeight = 50;
    Plane mousePlane;
    Camera mainCam;
    Vector3 mousePos3d;
    Vector2 prevMousePos;
    float mLBPressedTime = 0f;
    bool canDrag = false;
    bool canRotate = false;
    Vector3 lastHitPoint = Vector3.zero;
    Gate standingBlock;

    Gate[ ][ ] gatesMap;
    bool[ ][ ] collisionMap;

    public const float CLICK_TIME = 0.5f;

    void Start( ) {
        mousePlane = new Plane(Vector3.up, Vector3.zero);
        realPlane.transform.position = new Vector3(mapWidth / 2f, 0f, mapHeight / 2f);
        realPlane.transform.localScale = new Vector3(mapWidth / 10f, 1f, mapHeight / 10f);
        Material realPlaneMaterial = realPlane.GetComponent<Renderer>( ).material;
        realPlaneMaterial.mainTextureScale = new Vector2(mapWidth, mapHeight);
        realPlaneMaterial.mainTextureOffset = new Vector2(0.5f, 0.5f);
        // TODO: create height-map for realPlane
        mainCam = Camera.main;
        float cameraDistance = Mathf.Max(mapWidth, mapHeight);
        mainCam.transform.position = realPlane.transform.position - new Vector3(0, 0, cameraDistance);
        mainCam.transform.rotation = Quaternion.identity;
        mainCam.transform.RotateAround(realPlane.transform.position, Vector3.right, 80f);
        gatesMap = Utils.Init2DArray<Gate>(mapWidth, mapHeight, null);
        collisionMap = Utils.Init2DArray(mapWidth, mapHeight, false);
    }

    void Update( ) {
        if (Input.GetKeyDown(KeyCode.A)) {
            state = State.Block;
            if (standingBlock != null) {
                Destroy(standingBlock.gameObject);
            }
            standingBlock = Instantiate(andGatePrefab);
            //standingBlock.transform.localScale = new Vector3(0.025f, 0.025f, 0.025f);
            foreach (var renderer in standingBlock.GetComponentsInChildren<Renderer>( )) {
                var rendererColor = renderer.material.color;
                rendererColor.a = 0.2f;
                renderer.material.color = rendererColor;
            }
        } else if (Input.GetKeyDown(KeyCode.Escape)) {
            state = State.Camera;
            Destroy(standingBlock.gameObject);
        }
        Vector2 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        bool canStandBlock = false;
        float distance;
        if (mousePlane.Raycast(ray, out distance)) {
            Vector3 hitPoint = ray.GetPoint(distance);
            int x = Mathf.FloorToInt(hitPoint.x);
            int z = Mathf.FloorToInt(hitPoint.z);
            bool mouseOverMap;
            if (mouseOverMap = x >= 0 && x < mapWidth && z >= 0 && z < mapHeight) {
                float mouseScroolWheelAxis = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(mouseScroolWheelAxis) > 0f) {
                    Vector3 mainCamPositionBackup = mainCam.transform.position;
                    mainCam.transform.position += mouseScroolWheelAxis * 10f * ray.direction;
                    if (mainCam.transform.position.y < 3f) {
                        mainCam.transform.position = mainCamPositionBackup;
                    }
                }
                if (state == State.Block && (canStandBlock = mouseOverMap)) {
                    for (uint i = (uint)x - 2, iTo = i + 5; i < iTo; i++) {
                        for (uint j = (uint)z - 2, jTo = j + 5; j < jTo; j++) {
                            if (collisionMap[i][j]) {
                                canStandBlock = false;
                                i = iTo - 1;
                                break;
                            }
                        }
                    }
                }
                if (Input.GetMouseButtonDown(0)) {
                    canDrag = true;
                    mLBPressedTime = Time.time;
                    mousePos3d = ray.GetPoint(distance);
                    mousePos3d.y = 0;
                }
                if (Input.GetMouseButtonUp(0)) {
                    canDrag = false;
                    if (state == State.Block && Time.time - mLBPressedTime <= CLICK_TIME && canStandBlock) {
                        state = State.Camera;
                        foreach (var renderer in standingBlock.GetComponentsInChildren<Renderer>( )) {
                            // TODO: make function for changing color
                            var rendererColor = renderer.material.color;
                            rendererColor.a = 1f;
                            renderer.material.color = rendererColor;
                            // TODO: change render mode
                            //renderer.material.SetFloat("_Mode", 0);
                        }
                        for (uint i = (uint)x - 2, iTo = i + 5; i < iTo; i++) {
                            for (uint j = (uint)z - 2, jTo = j + 5; j < jTo; j++) {
                                gatesMap[i][j] = standingBlock;
                                collisionMap[i][j] = true;
                            }
                            collisionMap[i][z - 3] = collisionMap[i][z + 3] = true;
                        }
                        for (uint j = (uint)z - 2, jTo = j + 5; j < jTo; j++) {
                            collisionMap[x - 3][j] = collisionMap[x + 3][j] = true;
                        }
                        standingBlock = null;
                    }
                }
                if (Input.GetMouseButtonDown(1)) {
                    canRotate = true;
                    lastHitPoint = hitPoint;
                }
                if (Input.GetMouseButtonUp(1)) {
                    canRotate = false;
                }
            }
            if (Input.GetMouseButton(0) && canDrag) {
                Vector3 mouse3DTranslation = ray.GetPoint(distance);
                mouse3DTranslation.y = 0;
                mainCam.transform.position -= (mouse3DTranslation - mousePos3d);
            } else if (state == State.Block) {
                if (canStandBlock) {
                    standingBlock.transform.position = new Vector3(x + 0.5f, 0, z + 0.5f);
                    foreach (var renderer in standingBlock.GetComponentsInChildren<Renderer>( )) {
                        bool colorIsSetted = false;
                        Color color = default(Color);
                        switch (renderer.material.name) {
                            case "BlockMaterial":
                            case "BlockMaterial (Instance)":
                                color = new Color(0f, 0f, 0.8f, 0.5f);
                                colorIsSetted = true;
                                break;
                            case "TextMaterial":
                            case "TextMaterial (Instance)":
                                color = new Color(0.8f, 0f, 0f, 0.5f);
                                colorIsSetted = true;
                                break;
                        }
                        if (colorIsSetted) {
                            renderer.material.color = color;
                        }
                    }
                } else {
                    standingBlock.transform.position = new Vector3(hitPoint.x, 0.6f, hitPoint.z);
                    foreach (var renderer in standingBlock.GetComponentsInChildren<Renderer>( )) {
                        renderer.material.color = new Color(1f, 0f, 0f, 0.5f);
                    }
                }
            }
            if (Input.GetMouseButton(1) && canRotate) {
                mainCam.transform.RotateAround(lastHitPoint, Vector3.up, mousePos.x - prevMousePos.x);
                Vector3 mainCamPositionBackup = mainCam.transform.position;
                Quaternion mainCamRotationBackup = mainCam.transform.rotation;
                mainCam.transform.RotateAround(lastHitPoint,
                    Vector3.Cross(Vector3.up, mainCam.transform.rotation * Vector3.up), prevMousePos.y - mousePos.y);
                if (mainCam.transform.rotation.eulerAngles.x < 30f || mainCam.transform.position.y < 3f) {
                    // TODO: bring camera close to the border
                    mainCam.transform.position = mainCamPositionBackup;
                    mainCam.transform.rotation = mainCamRotationBackup;
                }
            }
        }
        prevMousePos = mousePos;
    }
}
